using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [Header("Level")]
    [SerializeField][Tooltip("Level 1 begins at index 1.")] private int levelNum;
    [SerializeField] private bool isCompleted;
    [SerializeField] private int tasksToComplete;
    [SerializeField][Tooltip("In seconds")] private int timeLimit;
    [SerializeField] private float boredomDecayRate;

    public bool IsCompleted() => isCompleted;

    public void SetCompleted(bool completed) => isCompleted = completed;

    public int GetTasksToComplete() => tasksToComplete;

    public int GetTimeLimit() => timeLimit;

    public float GetBoredomDecayRate() => boredomDecayRate;

}
