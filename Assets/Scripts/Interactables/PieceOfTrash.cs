public class Trash : Interactable {

    public override void Interact() {
        taskManager.ReduceTrashRemaining();
        Destroy(gameObject);
    }
}
