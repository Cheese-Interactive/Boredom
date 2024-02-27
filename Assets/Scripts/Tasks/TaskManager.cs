using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("Tasks")]
    [SerializeField] private GameObject trosh;
    [SerializeField] private int trashToSpawn;
    [SerializeField] private Vector2 topLeftTrashSpawnBound;
    [SerializeField] private Vector2 bottomRightTrashSpawnBound;
    private Task currTask;
    private int trashRemaining;

    private void Update() {

        // TODO: if trashRemaining >= 0 (during cleanup) then complete the task and assign a new one

    }

    public bool AssignTask(Task task) {

        if (currTask != null && !currTask.IsComplete()) return false;

        currTask = task;

        if (task is Cleanup)
            SpawnTrash();

        return true;

    }


    public int SpawnTrash() {

        trashRemaining = trashToSpawn;

        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector2(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        return trashRemaining;

    }

    public void ReduceTrashRemaining() { trashRemaining--; }

}
