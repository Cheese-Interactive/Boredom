using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("References")]
    private PlayerController playerController;

    [Header("Tasks")]
    [SerializeField] private int tasksToWin;
    [SerializeField] private List<Laptop> tasks = new List<Laptop>();
    Laptop currentLoptop;
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
        ShuffleTask();
        print(tasksDone + " done, " + (tasksToWin - tasksDone) + " remaining");

    }

    private void ShuffleTask() {

        for (int i = 0; i < tasks.Count; i++)
            tasks[i].SetInteractable(false);
        currentLoptop = tasks[Random.Range(0, tasks.Count)];
        currentLoptop.SetInteractable(true);

    }
}
