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

    [Header("Victory Screen")]
    [SerializeField] private CanvasGroup victoryScreen;
    [SerializeField] private float victoryFadeDuration;
    [SerializeField] private Button menuButton1;
    [SerializeField] private Button quitButton1;

    [Header("Loss Screen")]
    [SerializeField] private CanvasGroup lossScreen;
    [SerializeField] private float lossFadeDuration;
    [SerializeField] private Button menuButton2;
    [SerializeField] private Button quitButton2;

    [Header("Loading Screen")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingFadeDuration;
    private AsyncOperation sceneLoad;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        taskManager = FindObjectOfType<TaskManager>();
        gameData = FindObjectOfType<GameData>();
        animator = GetComponent<Animator>();

        victoryScreen.gameObject.SetActive(false);
        lossScreen.gameObject.SetActive(false);
        HideLoadingScreen();

        quizPaper.gameObject.SetActive(false);
        questionUIs = new List<QuestionUI>();
        //quizCloseButton.onClick.AddListener(() => StartCoroutine(CloseQuiz(false)));

        menuButton1.onClick.AddListener(LoadMainMenu);
        menuButton2.onClick.AddListener(LoadMainMenu);

        quitButton1.onClick.AddListener(() => Application.Quit());
        quitButton2.onClick.AddListener(() => Application.Quit());

        ResetTaskInfo();

    }

    //private void Update() {

    //    /* QUIZ */
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //        StartCoroutine(CloseQuiz(false)); // close quiz

    //}

    #region Tasks

    public void SetTaskInfo(int taskNum, string taskName, string taskDescription) {

        if (taskNum == 0)
            taskHeaderText.text = "No Task";
        else
            taskHeaderText.text = "Task " + taskNum + "/" + taskManager.GetTotalTasks() + ":";

        taskNameText.text = taskName;
        taskDescriptionText.text = taskDescription;
        RefreshLayout(taskHUD);

    }

    public void ResetTaskInfo() {

        SetTaskInfo(0, "", "");

    }

    #endregion

    #region Quiz

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

            submitButton.interactable = false; // prevent multiple submissions
            StartCoroutine(OnQuizComplete(quiz.ValidateAnswers(questionUIs)));

        });

        quizPaper.gameObject.SetActive(true);
        StartCoroutine(RebuildLayout(quizContentParent, quizPaper.transform)); // rebuild layout AFTER making it visible

        animator.SetTrigger("openQuiz");
        quizOpen = true;

    }

    private IEnumerator OnQuizComplete(bool pass) {

        yield return new WaitForSeconds(quizCompleteWaitDuration);
        yield return StartCoroutine(CloseQuiz(pass)); // wait for quiz to close

    }

    private IEnumerator CloseQuiz(bool pass) {

        if (!quizOpen) yield break; // quiz already closed

        animator.SetTrigger("closeQuiz");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (pass)
            taskManager.CompleteCurrentTask(); // complete task
        else
            taskManager.FailCurrentTask(); // fail task

        // clear all quiz data
        questionUIs.Clear();

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before quiz closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        quizPaper.gameObject.SetActive(false);
        quizOpen = false;

        playerController.StartBoredomTick(); // start ticking boredom again

    }

    #endregion

    #region Main Menu

    private void LoadMainMenu() {

        sceneLoad = SceneManager.LoadSceneAsync(0);
        sceneLoad.allowSceneActivation = false;
        ShowLoadingScreen();

    }

    #endregion

    #region Victory/Loss

    public void ShowVictoryScreen() {

        victoryScreen.alpha = 0f;
        victoryScreen.gameObject.SetActive(true);
        victoryScreen.DOFade(1f, 1f);

    }

    public void ShowLossScreen() {

        lossScreen.alpha = 0f;
        lossScreen.gameObject.SetActive(true);
        lossScreen.DOFade(1f, 1f);

    }

    #endregion

    #region Loading Screen

    private void ShowLoadingScreen() {

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingFadeDuration).OnComplete(() => {

            if (sceneLoad != null) sceneLoad.allowSceneActivation = true;

        });
    }

    private void HideLoadingScreen() {

        loadingScreen.alpha = 1f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false));

    }

    #endregion

    #region Utility

    private void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayout(Transform layout1, Transform layout2) { // make sure to maintain order

        RefreshLayout(layout1);
        yield return new WaitForEndOfFrame();
        RefreshLayout(layout2);

    }

    #endregion
}