public class Trash : Interactable {

    public override void Interact() {
        taskManager.OnTrashPickup();
        Destroy(gameObject);
    }
}
