using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Quiz")]
public class QuizTask : Task {

    public override void OnTaskComplete() {

        Debug.Log("Drag Quiz Finished");

    }
}
