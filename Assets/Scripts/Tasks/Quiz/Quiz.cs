using System;
using UnityEngine;

[Serializable]
public class Quiz {

    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;

    public void Initialize() {

        foreach (QuizQuestion question in questions)
            question.Initialize();

    }

}