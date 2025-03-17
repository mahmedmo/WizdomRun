using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleFileBrowser;

public class UserMenuManager : MonoBehaviour
{
    public GameObject homeView;
    public GameObject campaignView;
    public GameObject campaignObjPrefab;
    public GameObject AppLoad;
    public Sprite[] campaignIcons;
    public Transform contentPanel;
    public TextMeshProUGUI greetingText;

    public TextMeshProUGUI importFileName;
    public TextMeshProUGUI importFileError;

    public GameObject TitleInputField;
    public GameObject questBtn;
    public GameObject odysseyBtn;
    public GameObject sagaBtn;

    public Image startCampaignImg;
    public Button startCampaignBtn;

    // Track validation and PDF file path.
    private bool pdfSelectedValid = false;
    private bool campaignLengthSelected = false;
    private CampaignLength campaignLength;
    private string selectedPDFPath = "";
    private bool doneLoading = false;
    public static UserMenuManager Instance { get; private set; }

    // Reference to QuestionService.
    private QuestionService questionService;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        doneLoading = false;
        ResetView();
        // Assume a QuestionService exists in the scene.
        questionService = GetComponent<QuestionService>();
        if (questionService == null)
        {
            Debug.LogError("QuestionService not found in the scene!");
        }
    }

    void ResetView()
    {
        homeView.SetActive(true);
        campaignView.SetActive(false);
        importFileError.gameObject.SetActive(false);
        importFileName.text = "no file selected";
        importFileName.color = Color.white;

        startCampaignImg.color = new Color(161f / 255f, 161f / 255f, 156f / 255f);
        startCampaignBtn.interactable = false;

        TMP_InputField titleInput = TitleInputField.GetComponent<TMP_InputField>();
        titleInput.text = "";

        pdfSelectedValid = false;
        campaignLengthSelected = false;
        selectedPDFPath = "";
        Color defaultLengthColor = new Color(1f, 0.9843f, 0.7608f);
        questBtn.GetComponent<Image>().color = defaultLengthColor;
        odysseyBtn.GetComponent<Image>().color = defaultLengthColor;
        sagaBtn.GetComponent<Image>().color = defaultLengthColor;
        AppLoad.SetActive(false);
    }

    public void OnNewCampaign()
    {
        homeView.SetActive(false);
        campaignView.SetActive(true);
        importFileError.gameObject.SetActive(false);
    }

    public void OnBack()
    {
        ResetView();
    }

    public void CheckStartCampaignButtonAvailability()
    {
        TMP_InputField titleInput = TitleInputField.GetComponent<TMP_InputField>();
        bool titleValid = !string.IsNullOrEmpty(titleInput.text.Trim());

        if (pdfSelectedValid && titleValid && campaignLengthSelected)
        {
            startCampaignBtn.interactable = true;
            startCampaignImg.color = Color.white;
        }
        else
        {
            startCampaignBtn.interactable = false;
            startCampaignImg.color = new Color(161f / 255f, 161f / 255f, 156f / 255f);
        }
    }

    public void OnUploadPDF()
    {
        FileBrowser.ShowLoadDialog(
            (string[] paths) =>
            {
                if (paths.Length > 0)
                {
                    string filePath = paths[0];
                    if (Path.GetExtension(filePath).ToLower() == ".pdf")
                    {
                        importFileError.gameObject.SetActive(false);
                        importFileName.text = Path.GetFileName(filePath);
                        importFileName.color = Color.green;
                        importFileError.text = "";
                        pdfSelectedValid = true;
                        selectedPDFPath = filePath;  // Save the file path for later use.
                    }
                    else
                    {
                        importFileError.gameObject.SetActive(true);
                        importFileError.text = "Error: Selected file is not a PDF.";
                        importFileName.text = "no file selected";
                        importFileName.color = Color.white;
                        pdfSelectedValid = false;
                        selectedPDFPath = "";
                    }
                }
                else
                {
                    pdfSelectedValid = false;
                    selectedPDFPath = "";
                }
                CheckStartCampaignButtonAvailability();
            },
            () =>
            {
                importFileError.gameObject.SetActive(false);
                importFileName.text = "no file selected";
                importFileName.color = Color.white;
                pdfSelectedValid = false;
                selectedPDFPath = "";
                CheckStartCampaignButtonAvailability();
            },
            FileBrowser.PickMode.Files,
            false,
            null,
            "Select a PDF file",
            "Select"
        );
    }

    public void OnCampaignStart()
    {
        // Retrieve title.
        TMP_InputField titleInput = TitleInputField.GetComponent<TMP_InputField>();
        string title = titleInput.text.Trim();
        if (string.IsNullOrEmpty(title))
        {
            Debug.LogError("Title is required to start a campaign.");
            return;
        }

        // Convert campaign length enum to string.
        string campaignLengthStr = campaignLength.ToString().ToLower(); // e.g., "quest"
        Debug.Log("Campaign Length " + campaignLengthStr);
        // Get user info from AuthManager.
        string userID = AuthManager.Instance.GetUserId();
        Debug.Log("User Id:" + userID);

        string firebaseToken = AuthManager.Instance.AuthToken;
        Debug.Log("Firebase Token: " + firebaseToken);
        int startingLevel = 1;
        AppLoad.SetActive(true);
        // Start campaign creation.
        StartCoroutine(CampaignManager.Instance.CreateNewCampaign(userID, title, campaignLengthStr, startingLevel, firebaseToken));

        // Now, chain the creation of questions from the PDF.
        // We assume CampaignManager.Instance.currCampaign is set upon successful campaign creation.
        // We wait a short time for campaign creation (in production, better chain these callbacks).
        StartCoroutine(ChainPopulateQuestions(firebaseToken));
    }
    private IEnumerator ChainPopulateQuestions(string firebaseToken)
    {
        // Wait until the campaign is created.
        while (CampaignManager.Instance.currCampaign == null)
        {
            yield return null;
        }

        int campaignID = CampaignManager.Instance.currCampaign.CampaignID;
        Debug.Log("Now uploading PDF to populate questions for campaign: " + campaignID);

        // Call the question creation endpoint.
        yield return questionService.CreateQuestionsFromPDF(selectedPDFPath, campaignID, firebaseToken,
            () =>
            {
                Debug.Log("PDF processed and questions created successfully.");
                // Fetch the questions to update the campaign.
                StartCoroutine(questionService.GetQuestions(campaignID, firebaseToken, (List<QuestionService.Question> serviceQuestions) =>
                {
                    StartCoroutine(ProcessQuestions(serviceQuestions, firebaseToken));
                },
                (string error) =>
                {
                    Debug.LogError("Error fetching questions: " + error);
                }));
            },
            (string error) =>
            {
                Debug.LogError("Error creating questions from PDF: " + error);
                ResetView();
                importFileError.text = "Error parsing file, please select another file.";
            }
        );
    }

    /// <summary>
    /// Processes service questions: converts each to a global Question and fetches its answers.
    /// </summary>
    private IEnumerator ProcessQuestions(List<QuestionService.Question> serviceQuestions, string firebaseToken)
    {
        List<Question> globalQuestions = new List<Question>();

        foreach (var sq in serviceQuestions)
        {
            Question q = new Question();
            q.QuestionID = sq.questionID;
            q.CampaignID = sq.campaignID;
            // Convert difficulty string to global enum (case-insensitive).
            q.Difficulty = (QuestionDifficulty)System.Enum.Parse(typeof(QuestionDifficulty), sq.difficulty, true);
            q.GotCorrect = sq.gotCorrect;
            q.WrongAttempts = sq.wrongAttempts;
            q.QuestionStr = sq.questionStr;
            q.AnswerList = new List<Answer>();

            // Wait to fetch answers for this question.
            yield return StartCoroutine(GetAnswersForQuestion(sq.questionID, firebaseToken, (List<Answer> answers) =>
            {
                q.AnswerList = answers;
            }));

            globalQuestions.Add(q);
        }

        CampaignManager.Instance.currCampaign.QuestionList = globalQuestions;
        Debug.Log("Campaign populated with " + globalQuestions.Count + " questions.");

        // Debug each question and its answers.
        foreach (var question in globalQuestions)
        {
            Debug.Log($"Question {question.QuestionID}: {question.QuestionStr}");
            Debug.Log($"   Difficulty: {question.Difficulty}, GotCorrect: {question.GotCorrect}, WrongAttempts: {question.WrongAttempts}");
            if (question.AnswerList != null && question.AnswerList.Count > 0)
            {
                foreach (var answer in question.AnswerList)
                {
                    Debug.Log($"      Answer {answer.AnswerID}: {answer.AnswerStr} (IsCorrect: {answer.IsCorrect})");
                }
            }
            else
            {
                Debug.Log("      No answers available.");
            }
        }

        AppLoad.SetActive(false);
        SceneManager.LoadScene("HFLevel");
    }

    /// <summary>
    /// Helper coroutine that uses the GetAnswers endpoint to fetch answers for a specific question.
    /// </summary>
    private IEnumerator GetAnswersForQuestion(int questionID, string firebaseToken, System.Action<List<Answer>> onAnswers)
    {
        List<Answer> globalAnswers = new List<Answer>();

        yield return StartCoroutine(questionService.GetAnswers(questionID, firebaseToken, (List<QuestionService.Answer> serviceAnswers) =>
        {
            foreach (var sa in serviceAnswers)
            {
                Answer a = new Answer();
                a.AnswerID = sa.answerID;
                a.QuestionID = sa.questionID;
                a.AnswerStr = sa.answerStr;
                a.IsCorrect = sa.isCorrect;
                globalAnswers.Add(a);
            }
        },
        (string error) =>
        {
            Debug.LogError("Error fetching answers for question " + questionID + ": " + error);
        }));

        onAnswers?.Invoke(globalAnswers);
    }

    public void OnQuestSelected() { LengthPress(CampaignLength.QUEST); }
    public void OnOdysseySelected() { LengthPress(CampaignLength.ODYSSEY); }
    public void OnSagaSelected() { LengthPress(CampaignLength.SAGA); }

    public void LengthPress(CampaignLength cl)
    {
        Color selectedColor = new Color(197f / 255f, 190f / 255f, 90f / 255f);
        Color defaultLengthColor = new Color(1f, 0.9843f, 0.7608f);
        campaignLength = cl;
        campaignLengthSelected = true;

        questBtn.GetComponent<Image>().color = defaultLengthColor;
        odysseyBtn.GetComponent<Image>().color = defaultLengthColor;
        sagaBtn.GetComponent<Image>().color = defaultLengthColor;

        switch (cl)
        {
            case CampaignLength.QUEST:
                questBtn.GetComponent<Image>().color = selectedColor;
                break;
            case CampaignLength.ODYSSEY:
                odysseyBtn.GetComponent<Image>().color = selectedColor;
                break;
            case CampaignLength.SAGA:
                sagaBtn.GetComponent<Image>().color = selectedColor;
                break;
        }
        CheckStartCampaignButtonAvailability();
    }

    void Update()
    {
        if (!doneLoading && UserManager.Instance.CurrentUser == null)
        {
            AppLoad.SetActive(true);
            return;
        }
        else if (!doneLoading)
        {
            AppLoad.SetActive(false);
            greetingText.text = "Greetings, " + UserManager.Instance.CurrentUser.ScreenName + "!";
            doneLoading = true;
            PopulateList();
        }
    }

    void PopulateList()
    {
        List<Campaign> campaigns = UserManager.Instance.CurrentUser.CampaignList;
        Debug.Log("PopulateList: Found " + campaigns.Count + " campaigns.");

        for (int i = 0; i < campaigns.Count; i++)
        {
            Debug.Log("PopulateList: Instantiating campaign item " + i);
            GameObject newItem = Instantiate(campaignObjPrefab, contentPanel);

            // Log and set DateInfo
            Transform dateInfoTransform = newItem.transform.Find("DateInfo");
            if (dateInfoTransform != null)
            {
                TextMeshProUGUI campaignDate = dateInfoTransform.GetComponent<TextMeshProUGUI>();
                if (campaignDate != null)
                {
                    campaignDate.text = campaigns[i].LastUpdated;
                    Debug.Log("PopulateList: Campaign " + i + " LastUpdated set to: " + campaigns[i].LastUpdated);
                }
                else
                {
                    Debug.LogError("PopulateList: TextMeshProUGUI component not found on 'DateInfo' for campaign " + i);
                }
            }
            else
            {
                Debug.LogError("PopulateList: 'DateInfo' child not found in instantiated prefab for campaign " + i);
            }

            // Log and set LevelInfo
            Transform levelInfoTransform = newItem.transform.Find("LevelInfo");
            if (levelInfoTransform != null)
            {
                TextMeshProUGUI levelInfo = levelInfoTransform.GetComponent<TextMeshProUGUI>();
                if (levelInfo != null)
                {
                    levelInfo.text = campaigns[i].CurrLevel.ToString();
                    Debug.Log("PopulateList: Campaign " + i + " CurrLevel set to: " + campaigns[i].CurrLevel);
                }
                else
                {
                    Debug.LogError("PopulateList: TextMeshProUGUI component not found on 'LevelInfo' for campaign " + i);
                }
            }
            else
            {
                Debug.LogError("PopulateList: 'LevelInfo' child not found in instantiated prefab for campaign " + i);
            }

            // Log and set CampaignIcon
            Transform campaignIconTransform = newItem.transform.Find("CampaignIcon");
            if (campaignIconTransform != null)
            {
                Image campaignIcon = campaignIconTransform.GetComponent<Image>();
                if (campaignIcon != null)
                {
                    campaignIcon.sprite = campaignIcons[Random.Range(0, campaignIcons.Length)];
                    Debug.Log("PopulateList: Campaign " + i + " assigned a random icon.");
                }
                else
                {
                    Debug.LogError("PopulateList: Image component not found on 'CampaignIcon' for campaign " + i);
                }
            }
            else
            {
                Debug.LogError("PopulateList: 'CampaignIcon' child not found in instantiated prefab for campaign " + i);
            }
        }
    }

    public void Logout()
    {
        UserManager.Instance.Logout();
        AuthManager.Instance.Logout();
        SceneManager.LoadScene("MainMenu");
    }
}