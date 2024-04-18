
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [Header("Main Menu")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private Transform mainContent;
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;

    [Header("Level Select")]
    [SerializeField] private CanvasGroup levelMenu;
    [SerializeField] private Level[] levels;
    [SerializeField] private Transform levelButtonsParent;
    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private Button levelsCloseButton;

    [Header("Tutorial")]
    [SerializeField] private CanvasGroup tutorialMenu;
    [SerializeField] private Button tutorialCloseButton;

    [Header("Credits")]
    [SerializeField] private CanvasGroup creditsMenu;
    [SerializeField] private Button creditsCloseButton;

    [Header("Loading Screen")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private float loadingFadeDuration;
    private AsyncOperation sceneLoad;

    private void Start() {

        mainMenu.gameObject.SetActive(true);
        tutorialMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(false);

        HideLoadingScreen();

        playButton.onClick.AddListener(OpenLevelSelector);
        quitButton.onClick.AddListener(() => Application.Quit());
        tutorialButton.onClick.AddListener(OpenTutorial);
        creditsButton.onClick.AddListener(OpenCredits);

        tutorialCloseButton.onClick.AddListener(CloseTut);
        levelsCloseButton.onClick.AddListener(CloseLevelSelector);
        creditsCloseButton.onClick.AddListener(CloseCredits);

        LevelButton button = Instantiate(levelButtonPrefab, levelButtonsParent);
        button.Initialize(1, levels[0]);
        button.onClick.AddListener(() => LoadLevel(button.GetLevel().GetScene()));
        button.interactable = true; // level 1 is always unlocked

        for (int i = 1; i < levels.Length; i++) {

#if UNITY_EDITOR
            EditorUtility.SetDirty(levels[i]);
#endif

            button = Instantiate(levelButtonPrefab, levelButtonsParent);
            button.Initialize(i + 1, levels[i]);
            button.onClick.AddListener(() => LoadLevel(button.GetLevel().GetScene()));
            button.interactable = levels[i - 1].IsCompleted();

        }
    }

    private void OnDestroy() {

        DOTween.KillAll();

    }

    #region Tutorial Menu

    private void OpenTutorial() {

        mainMenu.gameObject.SetActive(false);
        tutorialMenu.gameObject.SetActive(true);

    }

    private void CloseTut() {

        mainMenu.gameObject.SetActive(true);
        tutorialMenu.gameObject.SetActive(false);

    }

    #endregion

    #region Level Menu

    private void OpenLevelSelector() {

        mainMenu.gameObject.SetActive(false);
        levelMenu.gameObject.SetActive(true);
        StartCoroutine(RebuildLayout(mainContent, levelButtonsParent)); // rebuild layout EACH time the level menu is opened, rebuild AFTER making it visible

    }

    private void CloseLevelSelector() {

        mainMenu.gameObject.SetActive(true);
        levelMenu.gameObject.SetActive(false);

    }

    private void LoadLevel(Object scene) {

        sceneLoad = SceneManager.LoadSceneAsync(scene.name);
        sceneLoad.allowSceneActivation = false;
        ShowLoadingScreen();

    }

    #endregion

    #region Credits Menu

    private void OpenCredits() {

        mainMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(true);

    }

    private void CloseCredits() {

        mainMenu.gameObject.SetActive(true);
        creditsMenu.gameObject.SetActive(false);

    }

    #endregion

    #region Loading Screen

    private void ShowLoadingScreen() {

        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(1f, loadingFadeDuration).OnComplete(() => {

            if (sceneLoad != null) sceneLoad.allowSceneActivation = true;

        });
    }

    private void HideLoadingScreen() {

        loadingScreen.alpha = 1f;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.DOFade(0f, loadingFadeDuration).OnComplete(() => loadingScreen.gameObject.SetActive(false));

    }

    #endregion

    #region Utility

    private void RefreshLayout(Transform layout) {

        LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());

    }

    private IEnumerator RebuildLayout(Transform layout1, Transform layout2) { // make sure to maintain order

        RefreshLayout(layout1);
        yield return new WaitForEndOfFrame();
        RefreshLayout(layout2);

    }

    #endregion
}

