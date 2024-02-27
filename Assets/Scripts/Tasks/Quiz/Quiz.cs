using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quiz {

    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;
    private QuestionUI[] questionUIs;
    private int lastIndex; // for tracking the last question index in order to add new one

    [Header("Checking")]
    [SerializeField] private Color correctColor;
    [SerializeField] private string correctMarker;
    [SerializeField] private Color incorrectColor;
    [SerializeField] private string incorrectMarker;
    [SerializeField] private float markerFadeDuration;

    public void Initialize() {

        questionUIs = new QuestionUI[questions.Length];
        lastIndex = 0;

        foreach (QuizQuestion question in questions)
            question.Initialize();

    }

    public QuizQuestion[] GetQuestions() { return questions; }

    public void ValidateAnswers() {

        for (int i = 0; i < questions.Length; i++)
            if (questions[i].IsCorrect(questionUIs[i].GetSelectedIndex()))
                questionUIs[i].SetScoreMarker(correctMarker, correctColor, markerFadeDuration);
            else
                questionUIs[i].SetScoreMarker(incorrectMarker, incorrectColor, markerFadeDuration);

    }

    public void AddQuestionUI(QuestionUI questionUI) { questionUIs[lastIndex++] = questionUI; }

}
