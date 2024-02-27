using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;

    [Header("Quiz")]
    [SerializeField] private CanvasGroup quizPaper;
    [SerializeField] private float quizFadeDuration;
    [SerializeField] private Transform quizContentParent;
    [SerializeField] private QuestionUI questionPrefab;
    [SerializeField] private Button submitButtonPrefab;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();

        quizPaper.alpha = 0f; // set alpha to 0 for fade
        quizPaper.gameObject.SetActive(false);

    }

    public void OpenQuiz(Quiz quiz) {

        playerController.SetMechanicStatus(MechanicType.Movement, false);

        foreach (Transform child in quizContentParent)
            Destroy(child.gameObject);

        foreach (QuizQuestion question in quiz.GetQuestions()) {

            QuestionUI questionObject = Instantiate(questionPrefab, quizContentParent);
            questionObject.SetQuestionText(question.GetQuestionText());
            questionObject.SetOptionTexts(question.GetOptions());
            quiz.AddQuestionUI(questionObject); // add to quiz for validation purposes (will be added in same order as quiz questions)

        }

        Button submitButton = Instantiate(submitButtonPrefab, quizContentParent);
        submitButton.onClick.AddListener(() => {

            quiz.ValidateAnswers();
            submitButton.interactable = false;

        });

        StartCoroutine(RebuildLayouts());

        quizPaper.gameObject.SetActive(true);
        quizPaper.DOFade(1f, quizFadeDuration);

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
