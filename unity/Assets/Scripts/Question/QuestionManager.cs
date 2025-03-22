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
    public GameObject QuestionInterface;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI answer01Text;
    public TextMeshProUGUI answer02Text;
    public TextMeshProUGUI answer03Text;
    public TextMeshProUGUI answer04Text;
    public TextMeshProUGUI questionCount;

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
    public Image questionButtonCooldownOverlay;
    public Button questionBtn;

    private Color defaultAnswerColor = Color.white;
    private Color selectedAnswerColor = Color.gray;
    private Color disabledSubmitColor = Color.gray;
    private Color enabledSubmitColor = Color.white;
    private Question currentQuestion = null;
    private Answer selectedAnswer = null;
    private List<Button> answerButtons;
    private List<TextMeshProUGUI> answerTexts;
    private float popDuration = 0.3f;

    private Vector3 questionInterfaceTargetScale = new Vector3(0.033f, 0.033f, 0.033f);
    private int questionsCorrect = 0;
    private const int maxQuestionsPerLevel = 5;

    private bool answerSubmitted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            answerButtons = new List<Button> { answer01Btn, answer02Btn, answer03Btn, answer04Btn };
            answerTexts = new List<TextMeshProUGUI> { answer01Text, answer02Text, answer03Text, answer04Text };

            submitBtn.interactable = false;
            submitImg.color = disabledSubmitColor;

            feedback.SetActive(false);
            QuestionInterface.SetActive(false);

        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        questionsCorrect = 0;
    }
    void Update()
    {
        if (questionsCorrect >= maxQuestionsPerLevel)
        {
            Debug.Log("Level complete. Maximum correct questions reached.");
            questionBtn.interactable = false;
        }
    }

    // Called by the mana gain spell to display the next unanswered question.
    public void ShowNextQuestion()
    {
        // Do not allow more questions if already reached 5 correct.
        if (questionsCorrect >= maxQuestionsPerLevel)
        {
            Debug.Log("Maximum number of questions answered correctly for this level.");
            questionBtn.interactable = false;
            return;
        }

        if (CampaignManager.Instance.currCampaign == null || CampaignManager.Instance.currCampaign.QuestionList == null)
        {
            Debug.LogError("No campaign or questions available.");
            return;
        }
        GameManager.Instance.FreezeGame();

        // Select an unanswered question, prioritizing easy then medium then hard.
        List<Question> allQuestions = CampaignManager.Instance.currCampaign.QuestionList;
        List<Question> unansweredEasy = allQuestions.FindAll(q => q.GotCorrect == 0 && q.Difficulty == QuestionDifficulty.EASY);
        List<Question> unansweredMedium = allQuestions.FindAll(q => q.GotCorrect == 0 && q.Difficulty == QuestionDifficulty.MEDIUM);
        List<Question> unansweredHard = allQuestions.FindAll(q => q.GotCorrect == 0 && q.Difficulty == QuestionDifficulty.HARD);

        if (unansweredEasy.Count > 0)
            currentQuestion = unansweredEasy[0];
        else if (unansweredMedium.Count > 0)
            currentQuestion = unansweredMedium[0];
        else if (unansweredHard.Count > 0)
            currentQuestion = unansweredHard[0];
        else
        {
            Debug.Log("All questions have been answered.");
            GameManager.Instance.UnFreezeGame();
            return;
        }

        questionText.text = currentQuestion.QuestionStr;

        List<Answer> answers = currentQuestion.AnswerList;
        string[] labels = { "<color=purple>A.</color>", "<color=purple>B.</color>", "<color=purple>C.</color>", "<color=purple>D.</color>" };

        for (int i = 0; i < answerButtons.Count; i++)
        {
            if (i < answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = $"{labels[i]} {answers[i].AnswerStr}";
                answerButtons[i].image.color = defaultAnswerColor;
                answerButtons[i].onClick.RemoveAllListeners();
                int index = i;
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        submitBtn.interactable = false;
        submitImg.color = disabledSubmitColor;
        selectedAnswer = null;
        submitBtn.onClick.RemoveAllListeners();
        submitBtn.onClick.AddListener(OnSubmitAnswer);

        QuestionInterface.SetActive(true);
        QuestionInterface.transform.localScale = Vector3.zero;
        QuestionInterface.transform.DOScale(questionInterfaceTargetScale, popDuration).SetEase(Ease.OutBack);
    }

    public void OnAnswerSelected(int index)
    {
        if (currentQuestion == null) return;
        List<Answer> answers = currentQuestion.AnswerList;
        if (index < 0 || index >= answers.Count) return;

        selectedAnswer = answers[index];

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

    public void OnSubmitAnswer()
    {
        if (answerSubmitted)
        {
            Debug.Log("Answer already submitted for this question.");
            return;
        }
        answerSubmitted = true;

        if (selectedAnswer == null)
        {
            Debug.LogError("No answer selected.");
            answerSubmitted = false;
            return;
        }
        submitBtn.interactable = false;

        Debug.Log("Before increment, questionsCorrect: " + questionsCorrect);

        bool isCorrect = selectedAnswer.IsCorrect;
        if (isCorrect)
        {
            Debug.Log("Correct answer!");
            PlayerMonitor.Instance.BoostMana(50);
            currentQuestion.GotCorrect = 1;
            questionsCorrect++;
            Debug.Log("After increment, questionsCorrect: " + questionsCorrect);
            UpdateQuestionCountText();

            if (questionsCorrect >= maxQuestionsPerLevel)
            {
                Debug.Log("Level complete. Maximum correct questions reached.");
                questionBtn.interactable = false;
            }
        }
        else
        {
            Debug.Log("Wrong answer.");
        }

        feedback.SetActive(true);
        feedback.transform.localScale = Vector3.zero;
        correctText.SetActive(isCorrect);
        incorrectText.SetActive(!isCorrect);
        feedback.transform.DOScale(1f, popDuration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    feedback.transform.DOScale(0f, popDuration).SetEase(Ease.InBack)
                        .OnComplete(() =>
                        {
                            feedback.SetActive(false);
                            QuestionInterface.transform.DOScale(0f, popDuration).SetEase(Ease.InBack)
                                .OnComplete(() =>
                                {
                                    QuestionInterface.SetActive(false);
                                    GameManager.Instance.UnFreezeGame();
                                    StartQuestionButtonCooldown(5f); // 5-second cooldown.
                                    answerSubmitted = false;
                                });
                        });
                });
            });
    }

    private void UpdateQuestionCountText()
    {
        int remaining = maxQuestionsPerLevel - questionsCorrect;
        questionCount.text = remaining.ToString();
    }

    //Starts a cooldown on the question button using a cooldown overlay.
    private void StartQuestionButtonCooldown(float duration)
    {
        if (questionButtonCooldownOverlay == null) return;

        questionButtonCooldownOverlay.gameObject.SetActive(true);
        questionBtn.interactable = false;
        questionButtonCooldownOverlay.fillAmount = 1f;
        questionButtonCooldownOverlay.DOFillAmount(0f, duration).SetEase(Ease.Linear)
            .OnComplete(() => { questionButtonCooldownOverlay.gameObject.SetActive(false); questionBtn.interactable = true; });

    }
}