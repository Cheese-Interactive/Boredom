using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("References")]
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

        gameManager = FindObjectOfType<GameManager>();
        uiController = FindObjectOfType<UIController>();

    }

    public bool AssignTask(Task task) {

        if (currTask != null) return false;

        currTask = task;

        if (currTask.GetTaskName().Length == 0) Debug.LogWarning("Current task doesn't have a name! Please set one in the inspector.");

        uiController.SetTaskInfo(currTask.GetTaskName(), currTask.GetTaskDescription());

        if (task is CleanupTask)
            SpawnTrash();

        if (task is MoppingTask)
            SpawnPuddle();

        if (task is QuizTask)
            uiController.OpenQuiz();

        return true;

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

    }

    public void RemoveCurrentTask() {

        currTask = null;
        uiController.ResetTaskInfo();

    }
}