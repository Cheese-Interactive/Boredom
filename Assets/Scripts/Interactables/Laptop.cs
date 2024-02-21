using UnityEngine;

public class Laptop : Interactable {

    public override void Interact() {
        if (isInteractable)
            GameObject.FindObjectOfType<GameManager>().finishTask();
    }
}
