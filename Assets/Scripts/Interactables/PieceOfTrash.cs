public class Trash : Interactable {
    // Start is called before the first frame update
    private TaskManager taskManager;

    void Start() {
        taskManager = FindObjectOfType<TaskManager>();
    }

    // Update is called once per frame
    public override void Interact() {
        taskManager.reduceTrashRemaining();
        Destroy(gameObject);
    }

}
