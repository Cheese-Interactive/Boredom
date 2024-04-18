using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Level : ScriptableObject {

    [Header("Level")]
    [SerializeField][Tooltip("Level 1 begins at index 1.")] private int levelNum;
    [SerializeField] private bool isCompleted;
    [SerializeField] private Object scene;

    public bool IsCompleted() => isCompleted;

    public void SetCompleted(bool completed) => isCompleted = completed;

    public Object GetScene() {

        return scene;

    }
}
