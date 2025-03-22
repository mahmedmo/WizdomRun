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
    public GameObject noCampaign;

    private bool pdfSelectedValid = false;
    private bool campaignLengthSelected = false;
    private CampaignLength campaignLength;
    private string selectedPDFPath = "";
    public static UserMenuManager Instance { get; private set; }

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
        AppLoad.SetActive(true);
        ResetView();
        questionService = GetComponent<QuestionService>();
        if (questionService == null)
        {
            Debug.LogError("QuestionService not found in the scene!");
        }

        if (UserManager.Instance != null)
            UserManager.Instance.OnCampaignsLoadedEvent += HandleCampaignsLoaded;
    }

    private void HandleCampaignsLoaded()
    {
        greetingText.text = "<color=black>Welcome, </color><color=blue>" + UserManager.Instance.CurrentUser.ScreenName + "</color>!";
        PopulateList();
        StartCoroutine(LoadDelay(1.5f));

    }

    private IEnumerator LoadDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AppLoad.SetActive(false);

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

    // Checks for correct file type and saves file path (using SimpleFileBrowser library)
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
                        selectedPDFPath = filePath;
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
                FileBrowser.HideDialog();
                CheckStartCampaignButtonAvailability();
            },
            () =>
            {
                importFileError.gameObject.SetActive(false);
                importFileName.text = "no file selected";
                importFileName.color = Color.white;
                pdfSelectedValid = false;
                selectedPDFPath = "";
                FileBrowser.HideDialog();
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
        TMP_InputField titleInput = TitleInputField.GetComponent<TMP_InputField>();
        string title = titleInput.text.Trim();
        if (string.IsNullOrEmpty(title))
        {
            Debug.LogError("Title is required to start a campaign.");
            return;
        }

        string campaignLengthStr = campaignLength.ToString().ToLower();
        Debug.Log("Campaign Length " + campaignLengthStr);

        string userID = AuthManager.Instance.UserId;
        Debug.Log("User Id:" + userID);

        string firebaseToken = AuthManager.Instance.AuthToken;
        Debug.Log("Firebase Token: " + firebaseToken);
        int startingLevel = 1;
        AppLoad.SetActive(true);

        StartCoroutine(OnCampaignStartCoroutine(userID, title, campaignLengthStr, startingLevel, firebaseToken));
    }

    private IEnumerator OnCampaignStartCoroutine(string userID, string title, string campaignLengthStr, int startingLevel, string firebaseToken)
    {
        // Wait until the campaign is fully created and loaded.
        yield return StartCoroutine(CampaignManager.Instance.CreateNewCampaign(userID, title, campaignLengthStr, startingLevel));

        // Now that the campaign is loaded, start populating questions.
        yield return StartCoroutine(ChainPopulateQuestions(firebaseToken));
    }
    private IEnumerator ChainPopulateQuestions(string firebaseToken)
    {
        while (CampaignManager.Instance.currCampaign == null)
        {
            yield return null;
        }

        int campaignID = CampaignManager.Instance.currCampaign.CampaignID;
        Debug.Log("Now uploading PDF to populate questions for campaign: " + campaignID);

        bool questionsLoaded = false;
        yield return questionService.CreateQuestionsFromPDF(selectedPDFPath, campaignID, firebaseToken,
            () =>
            {
                Debug.Log("PDF processed and questions created successfully.");

                StartCoroutine(questionService.GetQuestions(campaignID, firebaseToken, (List<QuestionService.Question> serviceQuestions) =>
                {
                    StartCoroutine(ProcessQuestions(serviceQuestions, firebaseToken, () =>
                    {
                        questionsLoaded = true;
                    }));
                },
                (string error) =>
                {
                    Debug.LogError("Error fetching questions: " + error);
                    questionsLoaded = true;
                }));
            },
            (string error) =>
            {
                Debug.LogError("Error creating questions from PDF: " + error);
                ResetView();
                importFileError.text = "Error parsing file, please select another file.";
                questionsLoaded = true;
            }
        );

        yield return new WaitUntil(() => questionsLoaded);

        yield return new WaitUntil(() => CampaignManager.Instance.currCampaign.QuestionList != null &&
                                         CampaignManager.Instance.currCampaign.QuestionList.Count > 0);

        CampaignManager.Instance.LoadMappedLevel();
    }

    private IEnumerator ProcessQuestions(List<QuestionService.Question> serviceQuestions, string firebaseToken, System.Action onComplete)
    {
        List<Question> globalQuestions = new List<Question>();

        foreach (var sq in serviceQuestions)
        {
            Question q = new Question();
            q.QuestionID = sq.questionID;
            q.CampaignID = sq.campaignID;
            q.Difficulty = (QuestionDifficulty)System.Enum.Parse(typeof(QuestionDifficulty), sq.difficulty, true);
            q.GotCorrect = sq.gotCorrect;
            q.WrongAttempts = sq.wrongAttempts;
            q.QuestionStr = sq.questionStr;
            q.AnswerList = new List<Answer>();

            yield return StartCoroutine(GetAnswersForQuestion(sq.questionID, firebaseToken, (List<Answer> answers) =>
            {
                q.AnswerList = answers;
            }));

            globalQuestions.Add(q);
        }

        CampaignManager.Instance.currCampaign.QuestionList = globalQuestions;
        Debug.Log("Campaign populated with " + globalQuestions.Count + " questions.");
        onComplete?.Invoke();
    }

    /// Helper function that uses the GetAnswers endpoint to fetch answers for a specific question.
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
        if (UserManager.Instance.CurrentUser == null)
        {
            AppLoad.SetActive(true);
        }
    }

    void PopulateList()
    {
        List<Campaign> campaigns = UserManager.Instance.CurrentUser.CampaignList;
        Debug.Log("PopulateList: Found " + campaigns.Count + " campaigns.");
        noCampaign.SetActive(campaigns.Count < 1);

        for (int i = 0; i < campaigns.Count; i++)
        {
            GameObject newItem = Instantiate(campaignObjPrefab, contentPanel);
            TextMeshProUGUI titleInfo = newItem.transform.Find("TitleInfo").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateInfo = newItem.transform.Find("DateInfo").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI levelInfo = newItem.transform.Find("LevelInfo").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI triesInfo = newItem.transform.Find("TriesInfo").GetComponent<TextMeshProUGUI>();
            Image campaignIcon = newItem.transform.Find("CampaignIcon").GetComponent<Image>();
            Button campaignBtn = newItem.transform.Find("Clickable").GetComponent<Button>();

            Campaign campaign = campaigns[i];
            titleInfo.text = campaign.Title;
            levelInfo.text = "Level: " + campaign.CurrLevel.ToString();
            Debug.Log("Last Played: " + campaign.LastUpdated);
            dateInfo.text = campaign.LastUpdated;
            triesInfo.text = "Remaining Tries: " + campaign.RemainingTries;
            int iconIndex = i % campaignIcons.Length;
            campaignIcon.sprite = campaignIcons[iconIndex];

            // Set up the button callback to load the campaign
            campaignBtn.onClick.AddListener(() =>
            {
                AppLoad.SetActive(true);
                StartCoroutine(LoadCampaignAndSwitchScene(campaign.CampaignID));
            });
        }
    }

    private IEnumerator LoadCampaignAndSwitchScene(int campaignID)
    {
        string firebaseToken = AuthManager.Instance.AuthToken;

        yield return StartCoroutine(CampaignManager.Instance.GetCampaignAndSetCurrent(campaignID));

        List<QuestionService.Question> serviceQuestions = null;
        yield return StartCoroutine(questionService.GetQuestions(campaignID, firebaseToken,
            (List<QuestionService.Question> questions) =>
            {
                serviceQuestions = questions;
            },
            (string error) =>
            {
                Debug.LogError("Error fetching questions: " + error);
            }
        ));

        List<Question> globalQuestions = new List<Question>();
        foreach (var sq in serviceQuestions)
        {
            Question q = new Question();
            q.QuestionID = sq.questionID;
            q.CampaignID = sq.campaignID;
            q.Difficulty = (QuestionDifficulty)System.Enum.Parse(typeof(QuestionDifficulty), sq.difficulty, true);
            q.GotCorrect = sq.gotCorrect;
            q.WrongAttempts = sq.wrongAttempts;
            q.QuestionStr = sq.questionStr;
            q.AnswerList = new List<Answer>();

            yield return StartCoroutine(GetAnswersForQuestion(sq.questionID, firebaseToken, (List<Answer> answers) =>
            {
                q.AnswerList = answers;
            }));

            globalQuestions.Add(q);
        }

        CampaignManager.Instance.currCampaign.QuestionList = globalQuestions;
        Debug.Log("Campaign populated with " + globalQuestions.Count + " questions.");

        CampaignManager.Instance.LoadMappedLevel();
    }

    public void Logout()
    {
        UserManager.Instance.Logout();
        AuthManager.Instance.Logout();
        SceneManager.LoadScene("MainMenu");
    }
}