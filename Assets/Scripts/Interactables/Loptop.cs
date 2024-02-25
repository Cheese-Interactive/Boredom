using System.Collections.Generic;
using UnityEngine;

public class Loptop : TaskInteractable {

    [Header("Quiz")]
    [SerializeField] private Quiz quiz;

    private void Start() {

        quiz.Initialize();

    }

    public override void Interact() {

        playerController.AssignTask(tasks[Random.Range(0, tasks.Count)]);

    }
}
