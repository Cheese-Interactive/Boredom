using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("Tasks:")]
    [Header("Cleanup")]
    [SerializeField] private GameObject trosh;
    [SerializeField] private int trashToSpawn;
    [SerializeField] private Vector2 topLeftTrashSpawnBound;
    [SerializeField] private Vector2 bottomRightTrashSpawnBound;
    private int trashRemaining;
    [Header("Mopping")]
    [SerializeField] private GameObject puddle;
    private Task currTask;

    private void Update() {

        if (currTask is CleanupTask && trashRemaining == 0)
            currTask.SetComplete(true);

        //idk if this works ngl


    }

    public bool AssignTask(Task task) {

        if (currTask != null && !currTask.IsComplete()) return false;

        currTask = task;

        if (task is CleanupTask)
            SpawnTrash();
        if (task is MoppingTask)
            Instantiate(puddle, new Vector2(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));
        return true;

    }


    public int SpawnTrash() {

        trashRemaining = trashToSpawn;

        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector2(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        return trashRemaining;

    }

    public void ReduceTrashRemaining() { trashRemaining--; }

    public void completeCurrentTask() {
        currTask.SetComplete(true);
    }

}
