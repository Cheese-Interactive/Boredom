using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DragQuiz {

    [Header("Questions")]
    [SerializeField] private int questionsPerQuiz;
    [SerializeField] private DragQuizQuestion[] questions;
    private DragQuizQuestion[] currQuestions;

    public DragQuizQuestion[] GetRandomQuestions() {

        List<DragQuizQuestion> availableQuestions = new List<DragQuizQuestion>(questions);
        currQuestions = new DragQuizQuestion[questionsPerQuiz];

        for (int i = 0; i < questionsPerQuiz; i++) {

            int randIndex = UnityEngine.Random.Range(0, availableQuestions.Count);
            currQuestions[i] = availableQuestions[randIndex];
            availableQuestions.RemoveAt(randIndex);

        }

        return currQuestions;

    }
}
