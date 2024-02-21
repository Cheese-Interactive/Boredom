using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    [Header("References")]
    protected PlayerController playerController;

    protected bool isInteractable;

    protected void Awake() {

        playerController = FindObjectOfType<PlayerController>();

    }

    public abstract void Interact();

    public void SetPlayerController(PlayerController playerController) { this.playerController = playerController; }

    public bool IsInteractable() { return isInteractable; }

    public void SetInteractable(bool status) { isInteractable = status; }

}
