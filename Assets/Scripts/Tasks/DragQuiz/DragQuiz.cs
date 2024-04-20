using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DragQuiz {

    [Header("Questions")]
    [SerializeField] private int questionsPerQuiz;
    [SerializeField] private DragQuizQuestion[] questions;
    private DragQuizQuestion[] currQuestions;

    public void Initialize() {

        foreach (DragQuizQuestion question in questions)
            question.Initialize();

        if (questionsPerQuiz > questions.Length)
            Debug.LogError("DragQuiz:Initialize - Question per quiz exceeds total questions.");

    }

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

    public bool ValidateAnswers(List<DragQuizQuestionUI> questions, List<string> answers) {

        // validate answers
        for (int i = 0; i < questions.Count; i++)
            if (!questions[i].GetQuestion().IsCorrect(answers[i]))
                return false;

        return true;

    }
}
