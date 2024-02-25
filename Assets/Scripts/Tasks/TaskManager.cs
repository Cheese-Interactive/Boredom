using UnityEngine;

public class TaskManager : MonoBehaviour {
    private Task currTask;

    [Header("Errands")]
    private int trashRemaining;
    [SerializeField] private GameObject trosh;
    [SerializeField] private int trashToSpawn;
    [SerializeField] private Vector2 topLeftTrashSpawnBound;
    [SerializeField] private Vector2 bottomRightTrashSpawnBound;

    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //todo: if trashRemaining >= 0 (during Cleanup) then complete the task and assign a new one
    }

    public bool AssignTask(Task task) {

        if (currTask != null && !currTask.IsComplete()) return false;

        currTask = task;
        if (task is Cleanup)
            spawnTrash();
        return true;


    }


    public int spawnTrash() {
        trashRemaining = trashToSpawn;
        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector2(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x),
                Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)),
                Quaternion.Euler(0, 0, Random.Range(0, 360)));
        return trashRemaining;
    }

    public int reduceTrashRemaining() {
        trashRemaining--;
        return trashRemaining;
    }
}
