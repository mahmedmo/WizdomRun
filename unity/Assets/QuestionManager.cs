using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance { get; private set; }

    [Header("Question Interface")]
    public GameObject QuestionInterface; // The dialogue panel for questions.
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answer01Text;
    public TextMeshProUGUI answer02Text;
    public TextMeshProUGUI answer03Text;
    public TextMeshProUGUI answer04Text;

    [Header("Answer Buttons")]
    public Button answer01Btn;
    public Button answer02Btn;
    public Button answer03Btn;
    public Button answer04Btn;

    [Header("Submit Button")]
    public Image submitImg;
    public Button submitBtn;

    public GameObject feedback;
    public GameObject correctText;
    public GameObject incorrectText;


    [Header("Question Button Cooldown Overlay")]
    public Image questionButtonCooldownOverlay; // Overlay for cooldown on the question button

    // Colors for button visuals
    private Color defaultAnswerColor = Color.white;
    private Color selectedAnswerColor = Color.gray; // Darkened color for selected answer
    private Color disabledSubmitColor = Color.gray;
    private Color enabledSubmitColor = Color.white;

    // The currently displayed question and the answer selected by the user.
    private Question currentQuestion = null;
    private Answer selectedAnswer = null;

    // For easy management, group answer buttons and their texts.
    private List<Button> answerButtons;
    private List<TextMeshProUGUI> answerTexts;
    private float popDuration = 0.3f; // Duration for pop in/out animations

    private Vector3 questionInterfaceTargetScale = new Vector3(0.033f, 0.033f, 0.033f);
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Build lists of answer buttons and texts.
            answerButtons = new List<Button> { answer01Btn, answer02Btn, answer03Btn, answer04Btn };
            answerTexts = new List<TextMeshProUGUI> { answer01Text, answer02Text, answer03Text, answer04Text };

            // Initially disable the submit button.
            submitBtn.interactable = false;
            submitImg.color = disabledSubmitColor;
            // Hide the question interface.
            feedback.SetActive(false);
            QuestionInterface.SetActive(false);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    /// <summary>
    /// Called (for example, by the question mark button) to display the next unanswered question.
    /// </summary>
    public void ShowNextQuestion()
    {
        if (CampaignManager.Instance.currCampaign == null || CampaignManager.Instance.currCampaign.QuestionList == null)
        {
            Debug.LogError("No campaign or questions available.");
            return;
        }
        GameManager.Instance.FreezeGame();
        // Select the first question with GotCorrect == 0.
        currentQuestion = CampaignManager.Instance.currCampaign.QuestionList.Find(q => q.GotCorrect == 0);
        if (currentQuestion == null)
        {
            Debug.Log("All questions have been answered.");
            return;
        }

        // Set the question text.
        questionText.text = currentQuestion.QuestionStr;

        // Get the list of answers.
        List<Answer> answers = currentQuestion.AnswerList;
        string[] labels = { "<color=purple>A.</color>", "<color=purple>B.</color>", "<color=purple>C.</color>", "<color=purple>D.</color>" };

        // Update answer buttons.
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = $"{labels[i]} {answers[i].AnswerStr}";
                answerButtons[i].image.color = defaultAnswerColor;
                answerButtons[i].onClick.RemoveAllListeners();
                int index = i; // Local copy for closure.
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        // Disable submit button until an answer is selected.
        submitBtn.interactable = false;
        submitImg.color = disabledSubmitColor;
        selectedAnswer = null;
        submitBtn.onClick.RemoveAllListeners();
        submitBtn.onClick.AddListener(OnSubmitAnswer);

        // Show the question interface using a scale pop in.
        QuestionInterface.SetActive(true);
        QuestionInterface.transform.localScale = Vector3.zero;
        QuestionInterface.transform.DOScale(questionInterfaceTargetScale, popDuration).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// Called when an answer button is pressed. Only one answer can be selected at a time.
    /// </summary>
    /// <param name="index">Index of the selected answer button.</param>
    public void OnAnswerSelected(int index)
    {
        if (currentQuestion == null) return;
        List<Answer> answers = currentQuestion.AnswerList;
        if (index < 0 || index >= answers.Count) return;

        selectedAnswer = answers[index];

        // Update visuals: mark selected button with a darker color.
        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < answers.Count)
            {
                answerButtons[i].image.color = (i == index) ? selectedAnswerColor : defaultAnswerColor;
            }
        }

        // Enable the submit button.
        submitBtn.interactable = true;
        submitImg.color = enabledSubmitColor;
    }

    /// <summary>
    /// Called when the submit button is pressed.
    /// Checks if the selected answer is correct, applies effects, then hides the interface.
    /// </summary>
    public void OnSubmitAnswer()
    {
        if (selectedAnswer == null)
        {
            Debug.LogError("No answer selected.");
            return;
        }

        bool isCorrect = selectedAnswer.IsCorrect;
        if (isCorrect)
        {
            Debug.Log("Correct answer!");
            PlayerMonitor.Instance.BoostMana(20);
            currentQuestion.GotCorrect = 1;
        }
        else
        {
            Debug.Log("Wrong answer.");
        }

        // Pop in the feedback.
        feedback.SetActive(true);
        feedback.transform.localScale = Vector3.zero;
        // Activate the appropriate feedback text.
        correctText.SetActive(isCorrect);
        incorrectText.SetActive(!isCorrect);
        feedback.transform.DOScale(1f, popDuration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // After a short delay, pop out the feedback.
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    feedback.transform.DOScale(0f, popDuration).SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            feedback.SetActive(false);
                            // Pop out the question interface.
                            QuestionInterface.transform.DOScale(0f, popDuration).SetEase(Ease.InBack)
                                .OnComplete(() =>
                                {
                                    QuestionInterface.SetActive(false);
                                    GameManager.Instance.FreezeGame();
                                    StartQuestionButtonCooldown(5f); // 5-second cooldown.
                                });
                        });
                });
            });
    }

    /// <summary>
    /// Starts a cooldown on the question button using a cooldown overlay.
    /// </summary>
    /// <param name="duration">Cooldown duration in seconds.</param>
    private void StartQuestionButtonCooldown(float duration)
    {
        if (questionButtonCooldownOverlay == null) return;

        questionButtonCooldownOverlay.gameObject.SetActive(true);
        questionButtonCooldownOverlay.fillAmount = 1f;
        questionButtonCooldownOverlay.DOFillAmount(0f, duration).SetEase(Ease.Linear)
            .OnComplete(() => { questionButtonCooldownOverlay.gameObject.SetActive(false); });
    }
}