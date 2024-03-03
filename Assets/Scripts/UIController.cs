using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private TaskManager taskManager;
    private GameData gameData;
    private Animator animator;

    [Header("Tasks")]
    [SerializeField] private Transform taskHUD;
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

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        taskManager = FindObjectOfType<TaskManager>();
        gameData = FindObjectOfType<GameData>();
        animator = GetComponent<Animator>();

        quizPaper.gameObject.SetActive(false);
        quizCloseButton.onClick.AddListener(() => StartCoroutine(CloseQuiz(false)));

        questionUIs = new List<QuestionUI>();

        ResetTaskInfo();

    }

    public void SetTaskInfo(string taskName, string taskDescription) {

        taskNameText.text = taskName;
        taskDescriptionText.text = taskDescription;
        RefreshLayout(taskHUD);

    }

    public void ResetTaskInfo() {

        SetTaskInfo("None", "");

    }

    public void OpenQuiz() {

        if (quizOpen) return; // quiz already open

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

    public IEnumerator CloseQuiz(bool completed) {

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

    }

    public void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayouts() {

        RefreshLayout(quizContentParent);
        yield return new WaitForEndOfFrame();
        RefreshLayout(quizPaper.transform);

    }
}