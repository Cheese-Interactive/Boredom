using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("References")]
    protected TaskManager taskManager;

    protected void Awake() {
        taskManager = FindObjectOfType<TaskManager>();
    }

    abstract public void Interact();

}
