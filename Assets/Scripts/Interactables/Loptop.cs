using System.Collections.Generic;
using UnityEngine;

public class Loptop : TaskInteractable {

    public override void Interact() {

        playerController.AssignTask(tasks[Random.Range(0, tasks.Count)]);

    }
}
