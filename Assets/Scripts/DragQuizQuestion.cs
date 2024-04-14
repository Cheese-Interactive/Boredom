using System;
using UnityEngine;

[Serializable]
public class DragQuizQuestion {

    [Header("Answers")]
    [SerializeField] private string questionText;
    [SerializeField] private string answer;

    public string GetQuestionText() { return questionText; }

    public string GetAnswer() { return answer; }

}
