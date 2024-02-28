using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Cleanup")]
public class CleanupTask : Task {

    public override void OnTaskComplete() {

        Debug.Log("Cleanup Finished");

    }
}
