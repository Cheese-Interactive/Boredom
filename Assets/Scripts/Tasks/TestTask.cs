using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(menuName = "Tasks/Test Task")]
public class TestTask : Task {

    public override void OnTaskComplete() {

        Debug.Log("done");

    }
}
