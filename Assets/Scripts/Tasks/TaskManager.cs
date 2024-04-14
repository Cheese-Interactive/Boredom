using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private List<TaskInteractable> destinations = new List<TaskInteractable>();
    private PlayerController player;
    private GameManager gameManager;
    private UIController uiController;

    [Header("Cleanup")]
    [SerializeField] private GameObject trosh;
    [SerializeField] private int trashToSpawn;
    [SerializeField] private Vector2 topLeftTrashSpawnBound;
    [SerializeField] private Vector2 bottomRightTrashSpawnBound;
    private int trashRemaining;

    [Header("Mopping")]
    [SerializeField] private GameObject puddlePrefab;
    private Task currTask;

    private void Start() {
        player = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
        uiController = FindObjectOfType<UIController>();
        AssignDestination();
    }

    public bool AssignTask(Task task) {
        player.SetArrowVisible(false);
        if (currTask != null) return false;

        currTask = task;

        if (currTask.GetTaskName().Length == 0) Debug.LogWarning("Current task doesn't have a name! Please set one in the inspector.");

        uiController.SetTaskInfo(gameManager.GetCompletedTasks() + 1, currTask.GetTaskName(), currTask.GetTaskDescription());

        if (task is CleanupTask)
            SpawnTrash();

        if (task is MoppingTask)
            SpawnPuddle();

        if (task is QuizTask)
            uiController.OpenQuiz();

        return true;

    }

    public void AssignDestination() {
        player.SetArrowVisible(true);
        for (int i = 0; i < destinations.Count; i++)
            destinations[i].IsInteractable(false);
        TaskInteractable dest = destinations[Random.Range(0, destinations.Count)];
        dest.IsInteractable(true);
        uiController.SetTaskInfo(gameManager.GetCompletedTasks() + 1, dest.GetName(), dest.GetDescription());
        player.PointArrow(dest.gameObject.transform.position);
    }

    private void SpawnTrash() {

        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        trashRemaining = trashToSpawn;

    }

    private void SpawnPuddle() {

        Instantiate(puddlePrefab, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0.1f, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(90f, 0f, 0f));

    }

    public void OnTrashPickup() {

        trashRemaining--;

        if (currTask is CleanupTask && trashRemaining == 0) // cleanup task finished
            CompleteCurrentTask();

    }

    public bool HasCurrentTask() { return currTask != null; }

    public void CompleteCurrentTask() {

        currTask = null;
        gameManager.OnTaskComplete();
        uiController.ResetTaskInfo();
        AssignDestination();

    }

    public void RemoveCurrentTask() {

        currTask = null;
        uiController.ResetTaskInfo();

    }
}