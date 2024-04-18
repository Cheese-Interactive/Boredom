using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuizQuestion {

    [Header("Answers")]
    [SerializeField] private string questionText;
    [SerializeField] private QuizAnswer[] answers;

    public void Initialize() {

        // make sure all conditions are met
        int correctAnswers = 0;

        foreach (QuizAnswer answer in answers)
            if (answer.IsCorrect())
                correctAnswers++;

        if (correctAnswers == 0)
            Debug.LogError("QuizQuestion:Initialize - There must be a correct answer.\nQuestion: " + questionText);

    }

    public string GetQuestionText() { return questionText; }

    public string[] GetOptions() {

        string[] options = new string[answers.Length];

        for (int i = 0; i < answers.Length; i++)
            options[i] = answers[i].GetAnswerText();

        return options;

    }

    public bool IsCorrect(int answerIndex) { return answers[answerIndex].IsCorrect(); }

    public int GetAnswerCount() { return answers.Length; }

}