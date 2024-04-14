using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private UIController uiController;
    [Header("Tasks")]
    [SerializeField] private int totalTasks;
    private int completedTasks;

    private void Start() {

        uiController = FindObjectOfType<UIController>();

    }

    private void OnDestroy() {

        DOTween.KillAll();

    }

    public void OnTaskComplete() {

        completedTasks++;
        FindObjectOfType<AudioManager>().PlaySound(AudioManager.GameSoundEffectType.TaskComplete);

        if (completedTasks >= totalTasks)
            uiController.ShowVictoryScreen();

    }

    public int GetTotalTasks() { return totalTasks; }

    public int GetCompletedTasks() { return completedTasks; }

}
