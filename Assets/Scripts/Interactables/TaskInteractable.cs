using System.Collections.Generic;
using UnityEngine;

public abstract class TaskInteractable : Interactable {

    [Header("Tasks")]
    [SerializeField] protected List<Task> tasks;
    protected bool interactability = false;
    public override void Interact() {

        taskManager.AssignTask(tasks[Random.Range(0, tasks.Count)]);
    }

    public void IsInteractable(bool t) { interactability = t; }

    public bool IsInteractable() { return interactability; }

    public abstract string GetName();

    public abstract string GetDescription();

}