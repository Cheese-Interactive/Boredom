using UnityEngine;

public class Laptop : Interactable {

    [Header("References")]
    private GameManager gameManager;

    private void Start() {

        gameManager = FindObjectOfType<GameManager>();

    }

    public override void Interact() {

        if (isInteractable)
            gameManager.FinishTask();

    }
}
