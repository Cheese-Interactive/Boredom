using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    [Header("Quiz")]
    [SerializeField] private Homework homework;

    [Header("Drag Quiz")]
    [SerializeField] private DragQuiz dragQuiz;

    private void Start() {

        homework.Initialize();
        dragQuiz.Initialize();

    }

    public Homework GetQuiz() { return homework; }

    public DragQuiz GetDragQuiz() { return dragQuiz; }

}
