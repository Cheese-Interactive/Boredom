using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Tasks")]
    [SerializeField] private int totalTasks;
    private int completedTasks;

    private void OnDestroy() {

        DOTween.KillAll();

    }

    public void OnTaskComplete() {

        completedTasks++;

        if (completedTasks >= totalTasks)
            print("Blud finished his tasks");

    }

    public int GetTotalTasks() { return totalTasks; }

    public int GetCompletedTasks() { return completedTasks; }

}
