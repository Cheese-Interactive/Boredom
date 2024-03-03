using UnityEngine;

public abstract class Task : ScriptableObject {

    [Header("Information")]
    [SerializeField] private string taskName;
    [SerializeField] private string taskDescription;

    public abstract void OnTaskComplete();

    public string GetTaskName() { return taskName; }

    public string GetTaskDescription() { return taskDescription; }

}