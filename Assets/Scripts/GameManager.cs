using DG.Tweening;
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
        FindObjectOfType<AudioManager>().PlaySound(AudioManager.GameSoundEffectType.TaskComplete);

        if (completedTasks >= totalTasks)
            print("Blud finished his tasks");

    }

    public int GetTotalTasks() { return totalTasks; }

    public int GetCompletedTasks() { return completedTasks; }

}
