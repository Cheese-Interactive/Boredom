using System;
using UnityEngine;

[Serializable]
public class QuizQuestion {

    [Header("Answers")]
    [SerializeField] private string questionText;
    [SerializeField] private QuizAnswer[] answers;

    public void Initialize() {

        // make sure all conditions are met
        if (answers.Length != 4)
            Debug.LogError("QuizQuestion:Initialize - Answers array length must be 4.");

        int correctAnswers = 0;

        foreach (QuizAnswer answer in answers)
            if (answer.IsCorrect())
                correctAnswers++;

        if (correctAnswers == 0)
            Debug.LogError("QuizQuestion:Initialize - There must be a correct answer.");

        if (correctAnswers > 1)
            Debug.LogError("QuizQuestion:Initialize - There must be one correct answer.");

    }

    public string GetQuestionText() { return questionText; }

}
