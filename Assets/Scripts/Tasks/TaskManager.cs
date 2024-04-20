using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour {

    [Header("References")]
    [SerializeField] private List<TaskInteractable> destinations = new List<TaskInteractable>();
    [SerializeField] private Level level;
    private PlayerController playerController;
    private UIController uiController;

    [Header("Tasks")]
    [SerializeField] private int totalTasks;
    private int completedTasks;
    private bool taskStarted;
    private bool gameComplete;

    [Header("Cleanup")]
    [SerializeField] private GameObject trosh;
    [SerializeField] private int trashToSpawn;
    [SerializeField] private Vector2 topLeftTrashSpawnBound;
    [SerializeField] private Vector2 bottomRightTrashSpawnBound;
    private int trashRemaining;

    [Header("Mopping")]
    [SerializeField] private GameObject puddlePrefab;
    private Task currTask;

    [Header("Sandwich")]
    [SerializeField] private GameObject[] ingredients;
    private bool[] ingredientStatuses;
    private int currIngredientIdx;

    private void Start() {

        playerController = FindObjectOfType<PlayerController>();
        uiController = FindObjectOfType<UIController>();
        AssignDestination();

        ingredientStatuses = new bool[ingredients.Length];
        ResetIngredients();
    }

    private void OnDestroy() {

        DOTween.KillAll();

    }

    private void ResetIngredients() {
        //i know this is sus but DO I CARE NAH
        int i = 0;
        currIngredientIdx = 0;
        foreach (GameObject g in ingredients) {
            g.SetActive(false);
            ingredientStatuses[i] = false;
            i++;
        }
    }

    public void OnTaskComplete() {

        completedTasks++;
        FindObjectOfType<AudioManager>().PlaySound(AudioManager.GameSoundEffectType.TaskComplete);

        if (completedTasks >= totalTasks)
            OnGameVictory();

    }

    public int GetTotalTasks() { return totalTasks; }

    public int GetCompletedTasks() { return completedTasks; }

    public void OnGameVictory() {

        gameComplete = true;
        playerController.PauseBoredomTick();
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        uiController.ShowVictoryScreen();
        level.SetCompleted(true);

    }

    public void OnGameLoss() {

        gameComplete = true;
        playerController.PauseBoredomTick();
        playerController.SetMechanicStatus(MechanicType.Movement, false);
        uiController.ShowLossScreen();

    }

    public bool StartTask() {

        if (currTask == null || taskStarted) // no assigned task or already started a task
            return false;

        taskStarted = true;

        playerController.SetArrowVisible(false);

        if (currTask is CleanupTask)
            SpawnTrash();

        if (currTask is MoppingTask)
            SpawnPuddle();

        if (currTask is HomeworkTask)
            uiController.OpenHomework();

        if (currTask is DragQuizTask)
            StartCoroutine(uiController.OpenDragQuiz());

        if (currTask is Sandwich)
            BeginSandwich();


        return true;

    }

    public void AssignDestination() {

        if (currTask != null || gameComplete) // already has a task or game already ended
            return;

        playerController.SetArrowVisible(true);

        UpdateTaskInteractables(destinations[Random.Range(0, destinations.Count)]);
        TaskInteractable dest = destinations[Random.Range(0, destinations.Count)];

        UpdateTaskInteractables(dest);
        playerController.PointArrow(dest.gameObject.transform.position);

    }

    private void UpdateTaskInteractables(TaskInteractable destination) {

        for (int i = 0; i < destinations.Count; i++) // set all destinations to not interactable
            destinations[i].SetInteractable(false);

        destination.SetInteractable(true);
        currTask = destination.GetRandomTask();
        uiController.SetTaskInfo(completedTasks + 1, currTask.GetName(), currTask.GetDescription());

    }

    private void SpawnTrash() {

        for (int i = 0; i < trashToSpawn; i++)
            Instantiate(trosh, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(0, 0, Random.Range(0, 360)));

        trashRemaining = trashToSpawn;

    }

    private void SpawnPuddle() {

        Instantiate(puddlePrefab, new Vector3(Random.Range(topLeftTrashSpawnBound.x, bottomRightTrashSpawnBound.x), 0.1f, Random.Range(topLeftTrashSpawnBound.y, bottomRightTrashSpawnBound.y)), Quaternion.Euler(90f, 0f, 0f));

    }

    private void BeginSandwich() {
        ResetIngredients();
        StartCoroutine(SpawnIngredient(currIngredientIdx));
        print(currIngredientIdx);
    }

    private IEnumerator SpawnIngredient(int idx) {
        if (idx >= ingredients.Length) {
            ResetIngredients();
            if (currTask is Sandwich)
                CompleteCurrentTask();
            yield break;
        } else {
            ingredients[idx].SetActive(true);
            playerController.SetArrowVisible(true);
            playerController.PointArrow(ingredients[idx].transform.position);
            while (!ingredientStatuses[idx])
                yield return null;
            ingredients[idx].SetActive(false);
            print("starting next ing");
            currIngredientIdx++;
            StartCoroutine(SpawnIngredient(currIngredientIdx));
        }
    }

    public void OnIngredientPickup() {
        ingredientStatuses[currIngredientIdx] = true;

        //this means it has been selected
    }

    public void OnTrashPickup() {

        trashRemaining--;

        if (currTask is CleanupTask && trashRemaining == 0) // cleanup task finished
            CompleteCurrentTask();

    }


    public bool HasCurrentTask() { return currTask != null; }

    public void CompleteCurrentTask() {

        currTask = null;
        OnTaskComplete();
        uiController.ResetTaskInfo();
        AssignDestination();
        taskStarted = false;

    }

    public void FailCurrentTask() {

        currTask = null;
        uiController.ResetTaskInfo();
        AssignDestination();
        taskStarted = false;

    }

    public bool IsTaskStarted() { return taskStarted; }

    public bool IsGameComplete() { return gameComplete; }

}