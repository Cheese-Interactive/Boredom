using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private TaskManager taskManager;
    private GameData gameData;
    private Animator animator;

    [Header("Tasks")]
    [SerializeField] private CanvasGroup mainHUD;
    [SerializeField] private Transform taskInfo;
    [SerializeField] private TMP_Text taskHeaderText;
    [SerializeField] private TMP_Text taskNameText;
    [SerializeField] private TMP_Text taskDescriptionText;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerText; // timer is part of HUD
    [SerializeField] private float flashThreshold;
    [SerializeField] private float flashWaitDuration;
    [SerializeField] private Color flashColor;
    private Coroutine flashTimerCoroutine;
    private Coroutine timerCoroutine;

    [Header("Quiz")]
    [SerializeField] private GameObject homework;
    [SerializeField] private float homeworkFadeDuration;
    [SerializeField] private Transform homeworkContent;
    [SerializeField] private HomeworkQuestionUI homeworkQuestionPrefab;
    [SerializeField] private Button submitButtonPrefab;
    [SerializeField] private float homeworkCompleteWaitDuration;
    private List<HomeworkQuestionUI> questionPrefabs;
    private bool homeworkOpen;

    [Header("Drag Quiz")]
    [SerializeField] private CanvasGroup dragQuiz;
    [SerializeField] private float dragQuizFadeDuration;
    [SerializeField] private float dragQuizCompleteWaitDuration;
    [SerializeField] private List<TVRepairQuestionUI> dragQuizQuestionObjs;
    [SerializeField] private List<DragQuizAnswerUI> dragQuizAnswerObjs;
    [SerializeField] private GameObject checkmark;
    private TVRepairQuestion[] dragQuizQuestions;
    private List<string> orderedAnswers;
    private bool dragQuizOpen;

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

        homework.gameObject.SetActive(false);
        questionPrefabs = new List<HomeworkQuestionUI>();
        //quizCloseButton.onClick.AddListener(() => StartCoroutine(CloseQuiz(false)));

        dragQuiz.gameObject.SetActive(false);
        checkmark.SetActive(false);

        menuButton1.onClick.AddListener(LoadMainMenu);
        menuButton2.onClick.AddListener(LoadMainMenu);

        quitButton1.onClick.AddListener(() => Application.Quit());
        quitButton2.onClick.AddListener(() => Application.Quit());

        /* order drag quiz answers by index */
        List<DragQuizAnswerUI> ordered = new List<DragQuizAnswerUI>(new DragQuizAnswerUI[dragQuizAnswerObjs.Count]);

        for (int i = 0; i < dragQuizAnswerObjs.Count; i++) {

            dragQuizAnswerObjs[i].Initialize();
            ordered[dragQuizAnswerObjs[i].GetIndex()] = dragQuizAnswerObjs[i];

        }

        dragQuizAnswerObjs = ordered;

        StartCoroutine(RebuildLayout(taskInfo));
        ResetTaskInfo();

    }

    //private void Update() {

    //    /* QUIZ */
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //        StartCoroutine(CloseQuiz(false)); // close quiz

    //}

    #region Timer

    public void StartTimer(int seconds) {

        timerCoroutine = StartCoroutine(HandleTimer(seconds));

    }

    public void StopTimer() {

        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

    }

    private IEnumerator HandleTimer(int seconds) {

        while (seconds > 0) {

            if (seconds <= flashThreshold) {

                if (flashTimerCoroutine == null) // make sure timer isn't already flashing
                    flashTimerCoroutine = StartCoroutine(FlashTimer()); // start flashing

            } else if (flashTimerCoroutine != null) { // timer is already flashing but time is above threshold

                StopCoroutine(flashTimerCoroutine); // stop flashing

            }

            timerText.text = string.Format("{0:0}:{1:00}", seconds / 60, seconds % 60);
            yield return new WaitForSeconds(1f);
            seconds--;

        }

        StopCoroutine(flashTimerCoroutine); // stop flashing
        timerText.text = "0:00";
        taskManager.OnGameLoss();

    }

    private IEnumerator FlashTimer() {

        while (true) {

            timerText.color = Color.clear;
            yield return new WaitForSeconds(flashWaitDuration);
            timerText.color = flashColor;
            yield return new WaitForSeconds(flashWaitDuration);

        }
    }

    #endregion

    #region Tasks

    public void SetTaskInfo(int taskNum, string taskName, string taskDescription) {

        if (taskNum == 0)
            taskHeaderText.text = "No Task";
        else
            taskHeaderText.text = "Task " + taskNum + "/" + taskManager.GetTotalTasks() + ":";

        taskNameText.text = taskName;
        taskDescriptionText.text = taskDescription;
        StartCoroutine(RebuildLayout(taskInfo));

    }

    public void ResetTaskInfo() {

        SetTaskInfo(0, "", "");

    }

    #endregion

    #region Homework

    public void OpenHomework() {

        if (homeworkOpen) return; // homework already open

        playerController.PauseBoredomTick(); // stop ticking boredom
        playerController.SetMechanicStatus(MechanicType.Movement, false);

        foreach (Transform child in homeworkContent)
            Destroy(child.gameObject);

        Homework homework = gameData.GetQuiz();
        HomeworkQuestion[] homeworkQuestions = homework.GetRandomQuestions();

        foreach (HomeworkQuestion question in homeworkQuestions) {

            HomeworkQuestionUI questionObject = Instantiate(homeworkQuestionPrefab, homeworkContent);
            questionObject.SetQuestionText(question.GetQuestionText());
            questionObject.SetOptionTexts(question.GetOptions());
            questionPrefabs.Add(questionObject); // track for validation purposes (added in same order as homework questions)

        }

        Button submitButton = Instantiate(submitButtonPrefab, homeworkContent);
        submitButton.onClick.AddListener(() => {

            submitButton.interactable = false; // prevent multiple submissions
            StartCoroutine(OnHomeworkComplete(homework.ValidateAnswers(questionPrefabs)));

        });

        this.homework.gameObject.SetActive(true);
        StartCoroutine(RebuildLayout(homeworkContent, this.homework.transform)); // rebuild layout AFTER making it visible

        animator.SetTrigger("openHomework");
        homeworkOpen = true;

    }

    private IEnumerator OnHomeworkComplete(bool pass) {

        yield return new WaitForSeconds(homeworkCompleteWaitDuration);
        yield return StartCoroutine(CloseHomework(pass)); // wait for homework to close

    }

    private IEnumerator CloseHomework(bool pass) {

        if (!homeworkOpen) yield break; // homework already closed

        animator.SetTrigger("closeHomework");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (pass)
            taskManager.CompleteCurrentTask(); // complete task
        else
            taskManager.FailCurrentTask(); // fail task

        // clear all homework data
        questionPrefabs.Clear();

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before homework closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        homework.gameObject.SetActive(false);
        homeworkOpen = false;

        if (!taskManager.IsGameComplete())
            playerController.StartBoredomTick(); // start ticking boredom again only if game is still going

    }

    #endregion

    #region Drag Quiz

    public IEnumerator OpenDragQuiz() {

        if (dragQuizOpen) yield break; // quiz already open

        this.dragQuiz.blocksRaycasts = true; // allow interactions

        playerController.PauseBoredomTick(); // stop ticking boredom
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        checkmark.SetActive(false);

        TVRepair dragQuiz = gameData.GetDragQuiz();
        dragQuizQuestions = dragQuiz.GetRandomQuestions();
        List<string> availableAnswers = new List<string>();
        orderedAnswers = new List<string>(new string[dragQuizQuestions.Length]);

        // set questions
        for (int i = 0; i < dragQuizQuestions.Length; i++) {

            dragQuizQuestionObjs[i].SetQuestionText(dragQuizQuestions[i]);
            availableAnswers.Add(dragQuizQuestions[i].GetAnswerText());

        }

        // randomize answers
        int guaranteedRandom = Random.Range(0, dragQuizQuestions.Length); // INDEX OF ANSWER THAT WILL BE INSERTED (FROM AVAILABLE ANSWERS)
        int randIndex; // INDEX OF WHICH POSITION ANSWER IS INSERTED INTO

        if (guaranteedRandom == 0) {

            randIndex = Random.Range(1, dragQuizQuestions.Length); // random index from 1 to length - 1

        } else if (guaranteedRandom == dragQuizQuestions.Length - 1) {

            randIndex = Random.Range(0, dragQuizQuestions.Length - 1); // random index from 0 to length - 2

        } else {

            int[] ranges = { Random.Range(0, guaranteedRandom), Random.Range(guaranteedRandom, dragQuizQuestions.Length) }; // random ranges from 0 to guaranteedRandom - 1 and guaranteedRandom to length - 1
            randIndex = ranges[Random.Range(0, ranges.Length)]; // random index from ranges

        }

        orderedAnswers[randIndex] = availableAnswers[guaranteedRandom]; // insert guaranteed random answer at random index
        dragQuizAnswerObjs[randIndex].SetAnswerText(availableAnswers[guaranteedRandom]); // set answer text (same index as insert index of ordered answers)
        availableAnswers.RemoveAt(guaranteedRandom); // remove guaranteed random answer

        for (int i = 0; i < dragQuizQuestions.Length; i++) { // I IS THE INDEX OF THE ANSWER THAT WILL BE INSERTED (FROM AVAILABLE ANSWERS)

            if (availableAnswers.Count == 0) break; // no more answers to randomize

            do {

                yield return null;
                randIndex = Random.Range(0, dragQuizQuestions.Length); // random index from ranges

            } while (orderedAnswers[randIndex] != null);

            if (i == guaranteedRandom) continue; // skip guaranteed random answer

            orderedAnswers[randIndex] = availableAnswers[availableAnswers.Count - 1]; // insert random answer at random index
            dragQuizAnswerObjs[randIndex].SetAnswerText(availableAnswers[availableAnswers.Count - 1]); // set answer text (same index as insert index of ordered answers)
            availableAnswers.RemoveAt(availableAnswers.Count - 1); // remove random answer

        }

        this.dragQuiz.gameObject.SetActive(true);

        animator.SetTrigger("openTVRepair");
        dragQuizOpen = true;

    }

    private IEnumerator OnDragQuizComplete(bool pass) {

        checkmark.SetActive(true);
        animator.SetTrigger("completeTVRepair");
        dragQuiz.blocksRaycasts = false; // stop interactions
        yield return new WaitForSeconds(dragQuizCompleteWaitDuration);
        yield return StartCoroutine(CloseDragQuiz(pass)); // wait for drag quiz to close

    }

    private IEnumerator CloseDragQuiz(bool pass) {

        if (!dragQuizOpen) yield break; // quiz already closed

        animator.SetTrigger("closeTVRepair");
        yield return new WaitForEndOfFrame(); // wait for animation to start playing

        if (pass)
            taskManager.CompleteCurrentTask(); // complete task
        else
            taskManager.FailCurrentTask(); // fail task

        playerController.SetMechanicStatus(MechanicType.Movement, true); // allow movement before quiz closes fully

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); // wait for animation to finish playing

        dragQuiz.gameObject.SetActive(false);
        dragQuizOpen = false;

        if (!taskManager.IsGameComplete())
            playerController.StartBoredomTick(); // start ticking boredom again only if game is still going

    }

    public void OnDragQuizDrop() {

        /* order drag quiz questions by index */
        List<TVRepairQuestionUI> ordered = new List<TVRepairQuestionUI>(new TVRepairQuestionUI[dragQuizQuestionObjs.Count]);

        for (int i = 0; i < dragQuizQuestionObjs.Count; i++)
            ordered[dragQuizQuestionObjs[i].GetIndex()] = dragQuizQuestionObjs[i];

        if (gameData.GetDragQuiz().ValidateAnswers(ordered, orderedAnswers)) StartCoroutine(OnDragQuizComplete(true));

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

        StartCoroutine(CloseHomework(false));
        StartCoroutine(CloseDragQuiz(false));

        victoryScreen.alpha = 0f;
        victoryScreen.gameObject.SetActive(true);
        mainHUD.DOFade(0f, victoryFadeDuration).SetEase(Ease.InCubic).OnComplete(() => mainHUD.gameObject.SetActive(false));
        victoryScreen.DOFade(1f, victoryFadeDuration).SetEase(Ease.InCubic);

    }

    public void ShowLossScreen() {

        StartCoroutine(CloseHomework(false));
        StartCoroutine(CloseDragQuiz(false));

        lossScreen.alpha = 0f;
        lossScreen.gameObject.SetActive(true);
        mainHUD.DOFade(0f, lossFadeDuration).SetEase(Ease.InCubic).OnComplete(() => mainHUD.gameObject.SetActive(false));
        lossScreen.DOFade(1f, lossFadeDuration).SetEase(Ease.InCubic);

    }

    #endregion

    #region Loading Screen

    private void ShowLoadingScreen() {

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingFadeDuration).SetEase(Ease.InCubic).OnComplete(() => {

            if (sceneLoad != null) sceneLoad.allowSceneActivation = true;

        });
    }

    private void HideLoadingScreen() {

        loadingScreen.alpha = 1f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingFadeDuration).SetEase(Ease.InCubic).OnComplete(() => loadingScreen.gameObject.SetActive(false));

    }

    #endregion

    #region Utility

    private void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayout(Transform layout) { // make sure to maintain order

        layout.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        layout.gameObject.SetActive(true);
        RefreshLayout(layout);

    }

    private IEnumerator RebuildLayout(Transform layout1, Transform layout2) { // make sure to maintain order

        RefreshLayout(layout1);
        yield return new WaitForEndOfFrame();
        RefreshLayout(layout2);

    }

    #endregion
}