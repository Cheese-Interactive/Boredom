using System.Collections.Generic;
using UnityEngine;

public abstract class TaskInteractable : Interactable {

    [Header("Tasks")]
    [SerializeField] protected List<Task> tasks;

    public override void Interact() {

        taskManager.AssignTask(tasks[Random.Range(0, tasks.Count)]);

    }
}