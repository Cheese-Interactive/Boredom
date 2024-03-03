using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Tasks")]
    [SerializeField] private int tasksToWin;
    private int tasksDone;

    private void OnDestroy() {

        DOTween.KillAll();

    }

    public void OnTaskComplete() {

        tasksDone++;

        if (tasksDone >= tasksToWin)
            print("Blud finished his tasks");

    }
}
