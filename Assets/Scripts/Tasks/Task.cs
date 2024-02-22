using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : ScriptableObject {

    [Header("Information")]
    [SerializeField] private string taskName;
    [SerializeField] private string taskDescription;
    private bool isComplete;

    public abstract void OnTaskComplete();

    public string GetTaskName() { return taskName; }

    public string GetTaskDescription() { return taskDescription; }

    public bool IsComplete() { return isComplete; }

    public void SetComplete(bool complete) { isComplete = complete; }

}
