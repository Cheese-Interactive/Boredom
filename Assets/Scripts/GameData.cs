using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {

    [Header("Quiz")]
    [SerializeField] private Quiz quiz;

    [Header("Drag Quiz")]
    [SerializeField] private DragQuiz dragQuiz;

    private void Start() {

        quiz.Initialize();

    }

    public Quiz GetQuiz() { return quiz; }

    public DragQuiz GetDragQuiz() { return dragQuiz; }

}
