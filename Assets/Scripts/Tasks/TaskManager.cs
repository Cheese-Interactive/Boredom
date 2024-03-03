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
    [SerializeField] private GameObject puddle;
    private Task currTask;

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();
        uiController = FindObjectOfType<UIController>();

    }

    private void Update() {

        if (currTask is CleanupTask && trashRemaining == 0) // cleanup task finished
            CompleteCurrentTask();

        //idk if this works ngl
        // this could be done 10x more efficiently

    }

    public bool AssignTask(Task task) {

        if (currTask != null) return false;

        currTask = task;

        uiController.SetTaskInfo(currTask.GetTaskName(), currTask.GetTaskDescription());

        if (task is CleanupTask)
            SpawnTrash();

        if (task is MoppingTask)
            Instantiate(puddle, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0.1f, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(90, 0, 0));

        if (task is QuizTask)
            uiController.OpenQuiz();

        return true;

    }

    public int SpawnTrash() {

        trashRemaining = trashToSpawn;

        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        return trashRemaining;

    }

    public void ReduceTrashRemaining() { trashRemaining--; }

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