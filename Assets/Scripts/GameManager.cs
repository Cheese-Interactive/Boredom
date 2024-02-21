using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private int tasksToWin;
    [SerializeField] private List<Laptop> tasks = new List<Laptop>();
    Laptop currentLoptop;
    private int tasksDone;

    // Start is called before the first frame update
    void Start() {
        shuffleTask();
    }

    // Update is called once per frame
    void Update() {
        if (tasksDone >= tasksToWin)
            print("Blud finished his tasks");
    }

    public void finishTask() {
        tasksDone++;
        currentLoptop.HideInteractKeyIcon();
        shuffleTask();
        print(tasksDone + " done, " + (tasksToWin - tasksDone) + " remaining");
    }

    private void shuffleTask() {
        Laptop temp = currentLoptop;
        for (int i = 0; i < tasks.Count; i++)
            tasks[i].ChangeStatus(false);
        tasks.Remove(currentLoptop);
        currentLoptop = tasks[Random.Range(0, tasks.Count)];
        currentLoptop.ChangeStatus(true);
        tasks.Add(temp);
    }
}
