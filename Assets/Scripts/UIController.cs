using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private GameManager gameManager;
    private TaskManager taskManager;
    private GameData gameData;
    private Animator animator;

    [Header("Tasks")]
    [SerializeField] private Transform taskHUD;
    [SerializeField] private TMP_Text taskHeaderText;
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private TMP_Text taskDescriptionText;

    [Header("Quiz")]
    [SerializeField] private GameObject quizPaper;
    [SerializeField] private Button quizCloseButton;
    [SerializeField] private float quizFadeDuration;
    [SerializeField] private Transform quizContentParent;
    [SerializeField] private QuestionUI questionPrefab;
    [SerializeField] private Button submitButtonPrefab;
    [SerializeField] private float quizCompleteWaitDuration;
    private List<QuestionUI> questionUIs;
    private bool quizOpen;

    [Header("Drag Quiz")]
    [SerializeField] private GameObject dragQuiz;
    [SerializeField] private Button dragQuizCloseButton;
    [SerializeField] private float dragQuizFadeDuration;
    [SerializeField] private Transform dragQuizContentParent;
    [SerializeField] private QuestionUI dragQuestionPrefab;
    [SerializeField] private float dragQuizCompleteWaitDuration;
    private List<TMP_Text> dragQuestionTexts;
    private List<TMP_Text> dragAnswerTexts;
    private bool dragQuizOpen;

    [Header("Victory Screen")]
    [SerializeField] private CanvasGroup victoryScreen;
    [SerializeField] private Button menuButton1;
    [SerializeField] private Button quitButton1;

    [Header("Loss Screen")]
    [SerializeField] private CanvasGroup lossScreen;
    [SerializeField] private Button menuButton2;
    [SerializeField] private Button quitButton2;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        taskManager = FindObjectOfType<TaskManager>();
        gameData = FindObjectOfType<GameData>();
        animator = GetComponent<Animator>();

        quizPaper.gameObject.SetActive(false);
        quizCloseButton.onClick.AddListener(() => StartCoroutine(CloseQuiz(false)));

        questionUIs = new List<QuestionUI>();

        victoryScreen.gameObject.SetActive(false);
        victoryScreen.alpha = 0f;

        lossScreen.gameObject.SetActive(false);
        lossScreen.alpha = 0f;

        menuButton1.onClick.AddListener(() => SceneManager.LoadScene(0));
        menuButton2.onClick.AddListener(() => SceneManager.LoadScene(0));

        quitButton1.onClick.AddListener(() => Application.Quit());
        quitButton2.onClick.AddListener(() => Application.Quit());

        ResetTaskInfo();

    }

    private void Update() {

        /* QUIZ */
        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(CloseQuiz(false)); // close quiz

    }

    public void SetTaskInfo(int taskNum, string taskName, string taskDescription) {

        if (taskNum == 0)
            taskHeaderText.text = "No Task";
        else
            taskHeaderText.text = "Task " + taskNum + "/" + gameManager.GetTotalTasks() + ":";

        taskNameText.text = taskName;
        taskDescriptionText.text = taskDescription;
        RefreshLayout(taskHUD);

    }

    public void ResetTaskInfo() {

        SetTaskInfo(0, "", "");

    }

    public void OpenQuiz() {

        if (quizOpen) return; // quiz already open

        playerController.PauseBoredomTick(); // stop ticking boredom
        playerController.SetMechanicStatus(MechanicType.Movement, false);

        foreach (Transform child in quizContentParent)
            Destroy(child.gameObject);

        Quiz quiz = gameData.GetQuiz();
        QuizQuestion[] quizQuestions = quiz.GetRandomQuestions();

        foreach (QuizQuestion question in quizQuestions) {

            QuestionUI questionObject = Instantiate(questionPrefab, quizContentParent);
            questionObject.SetQuestionText(question.GetQuestionText());
            questionObject.SetOptionTexts(question.GetOptions());
            questionUIs.Add(questionObject); // track for validation purposes (added in same order as quiz questions)

        }

        Button submitButton = Instantiate(submitButtonPrefab, quizContentParent);
        submitButton.onClick.AddListener(() => {

            if (quiz.ValidateAnswers(questionUIs)) { // quiz passed

                submitButton.interactable = false; // prevent multiple submissions
                StartCoroutine(OnQuizComplete());

            }
        });

        StartCoroutine(RebuildLayouts());

        quizPaper.gameObject.SetActive(true);

        animator.SetTrigger("openQuiz");
        quizOpen = true;

    }

    private IEnumerator OnQuizComplete() {

        yield return new WaitForSeconds(quizCompleteWaitDuration);
        yield return StartCoroutine(CloseQuiz(true)); // wait for quiz to close

    }

    private IEnumerator CloseQuiz(bool completed) {

        if (!quizOpen) yield break; // quiz already closed

        animator.SetTrigger("closeQuiz");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (completed)
            taskManager.CompleteCurrentTask();
        else
            taskManager.RemoveCurrentTask();

        // clear all quiz data
        questionUIs.Clear();

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before quiz closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        quizPaper.gameObject.SetActive(false);
        quizOpen = false;

        playerController.StartBoredomTick(); // start ticking boredom again

    }

    public void OpenDragQuiz() {

        if (dragQuizOpen) return; // quiz already open

        playerController.PauseBoredomTick(); // stop ticking boredom
        playerController.SetMechanicStatus(MechanicType.Movement, false);

        DragQuiz quiz = gameData.GetDragQuiz();
        DragQuizQuestion[] quizQuestions = quiz.GetRandomQuestions();

        for (int i = 0; i < quizQuestions.Length; i++) {

            dragQuestionTexts[i].text = quizQuestions[i].GetQuestionText();
            dragAnswerTexts[i].text = quizQuestions[i].GetAnswer();

        }

        StartCoroutine(RebuildLayouts());

        dragQuiz.gameObject.SetActive(true);

        animator.SetTrigger("openDragQuiz");
        dragQuizOpen = true;

    }

    private IEnumerator CloseDragQuiz(bool completed) {

        if (!dragQuizOpen) yield break; // quiz already closed

        animator.SetTrigger("closeDragQuiz");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (completed)
            taskManager.CompleteCurrentTask();
        else
            taskManager.RemoveCurrentTask();

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before quiz closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        dragQuiz.gameObject.SetActive(false);
        dragQuizOpen = false;

        playerController.StartBoredomTick(); // start ticking boredom again

    }

    public void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayouts() {

        RefreshLayout(quizContentParent);
        yield return new WaitForEndOfFrame();
        RefreshLayout(quizPaper.transform);

    }

    public void ShowVictoryScreen() {

        victoryScreen.gameObject.SetActive(true);
        victoryScreen.DOFade(1f, 1f);

    }

    public void ShowLossScreen() {

        lossScreen.gameObject.SetActive(true);
        lossScreen.DOFade(1f, 1f);

    }
}