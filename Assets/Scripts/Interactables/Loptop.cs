using UnityEngine;

public class Loptop : TaskInteractable {

    [Header("References")]
    private UIController uiController;

    [Header("Quiz")]
    [SerializeField] private Quiz[] quizzes;

    private void Start() {

        taskManager = FindObjectOfType<TaskManager>();
        uiController = FindObjectOfType<UIController>();

        foreach (Quiz quiz in quizzes)
            quiz.Initialize();

    }

    public override void Interact() {

        taskManager.AssignTask(tasks[Random.Range(0, tasks.Count)]);
        uiController.OpenQuiz(quizzes[Random.Range(0, quizzes.Length)]);

    }
}
