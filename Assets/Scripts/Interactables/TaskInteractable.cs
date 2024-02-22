using System.Collections.Generic;
using UnityEngine;

public abstract class TaskInteractable : MonoBehaviour {

    [Header("References")]
    protected PlayerController playerController;

    [Header("Tasks")]
    [SerializeField] protected List<Task> tasks;

    protected void Awake() {

        playerController = FindObjectOfType<PlayerController>();

    }

    public abstract void Interact();

    public void SetPlayerController(PlayerController playerController) { this.playerController = playerController; }

}
