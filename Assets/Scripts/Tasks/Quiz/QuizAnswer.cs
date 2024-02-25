using System;
using UnityEngine;

[Serializable]
public class QuizAnswer {

    [Header("Information")]
    [SerializeField] private string answerText;
    [SerializeField] private bool isCorrect;

    public string GetAnswerText() { return answerText; }

    public bool IsCorrect() { return isCorrect; }


}
