using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;

    [Header("Tasks")]
    [SerializeField] private int tasksToWin;
    [SerializeField] private List<Loptop> tasks = new List<Loptop>();
    private Loptop currLoptop;
    private int tasksDone;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();

        ShuffleTask();

    }

    private void Update() {

        if (tasksDone >= tasksToWin)
            print("Blud finished his tasks");

    }

    public void FinishTask() {

        tasksDone++;
        playerController.HideInteractKeyIcon();
        ShuffleTask();
        print(tasksDone + " done, " + (tasksToWin - tasksDone) + " remaining");

    }

    private void ShuffleTask() {

        Loptop tempLoptop = currLoptop;

        for (int i = 0; i < tasks.Count; i++)
            tasks[i].SetInteractable(false);

        tasks.Remove(currLoptop);
        currLoptop = tasks[Random.Range(0, tasks.Count)];
        currLoptop.SetInteractable(true);
        tasks.Add(tempLoptop);

    }
}
