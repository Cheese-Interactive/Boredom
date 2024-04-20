using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DragQuizAnswer {

    [Header("Information")]
    [SerializeField] private string answerText;

    public void Initialize() {

        if (answerText.Length == 0)
            Debug.LogError("DragQuizAnswer:Initialize - Answer text cannot be empty.");

    }

    public string GetAnswerText() { return answerText; }

}
