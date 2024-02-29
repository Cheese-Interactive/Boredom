using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;
    private TaskManager taskManager;
    private GameData gameData;

    [Header("Quiz")]
    [SerializeField] private CanvasGroup quizPaper;
    [SerializeField] private float quizFadeDuration;
    [SerializeField] private Transform quizContentParent;
    [SerializeField] private QuestionUI questionPrefab;
    [SerializeField] private Button submitButtonPrefab;
    [SerializeField] private float quizCompleteWaitDuration;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        taskManager = FindObjectOfType<TaskManager>();
        gameData = FindObjectOfType<GameData>();

        quizPaper.alpha = 0f; // set alpha to 0 for fade
        quizPaper.gameObject.SetActive(false);

    }

    public void OpenQuiz() {

        if (playerController.GetCurrentTask() is QuizTask) return; // prevent opening quiz if player doesn't have task

        playerController.SetMechanicStatus(MechanicType.Movement, false);

        foreach (Transform child in quizContentParent)
            Destroy(child.gameObject);

        Quiz quiz = gameData.GetQuiz();
        QuizQuestion[] quizQuestions = quiz.GetRandomQuestions();

        foreach (QuizQuestion question in quizQuestions) {

            QuestionUI questionObject = Instantiate(questionPrefab, quizContentParent);
            questionObject.SetQuestionText(question.GetQuestionText());
            questionObject.SetOptionTexts(question.GetOptions());
            quiz.AddQuestionUI(questionObject); // add to quiz for validation purposes (will be added in same order as quiz questions)

        }

        Button submitButton = Instantiate(submitButtonPrefab, quizContentParent);
        submitButton.onClick.AddListener(() => {

            if (quiz.ValidateAnswers()) { // quiz passed

                submitButton.interactable = false; // prevent multiple submissions
                StartCoroutine(OnQuizComplete());

            }
        });

        StartCoroutine(RebuildLayouts());

        quizPaper.gameObject.SetActive(true);
        quizPaper.DOFade(1f, quizFadeDuration);

    }

    private IEnumerator OnQuizComplete() {

        yield return new WaitForSeconds(quizCompleteWaitDuration);

        quizPaper.DOFade(0f, quizFadeDuration).OnComplete(() => {

            quizPaper.gameObject.SetActive(false);
            playerController.SetMechanicStatus(MechanicType.Movement, true);
            taskManager.CompleteCurrentTask();

        });
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
