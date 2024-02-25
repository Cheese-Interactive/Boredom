using UnityEngine;

public class Loptop : TaskInteractable {

    [Header("Quiz")]
    [SerializeField] private Quiz quiz;

    private void Start() {

        quiz.Initialize();

    }

    public void Interact() {

        taskManager.AssignTask(tasks[Random.Range(0, tasks.Count)]);

    }
}
