
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    [Header("Level Data")]
    [SerializeField] private bool[] levelUnlockStatuses;
    [SerializeField] private int[] levelSceneIndices;
    [SerializeField] private Level[] levels1;

    [Header("Animations")]
    [SerializeField] private GameObject dark;
    [SerializeField] private float fadeToDarkTime;
    private Image darkSprite;

    [Header("Main")]
    [SerializeField] private GameObject mainObj;
    [SerializeField] private List<Object> main;
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialObj;
    [SerializeField] private List<Object> tutorial = new List<Object>();
    [SerializeField] private Button t_close;

    [Header("Credits")]
    [SerializeField] private GameObject creditsObj;
    [SerializeField] private List<Object> credits = new List<Object>();
    [SerializeField] private Button c_close;

    [Header("Level Select")]
    [SerializeField] private GameObject levelsObj;
    [SerializeField] private List<Object> levels = new List<Object>();
    [SerializeField] private Button l_1;
    [SerializeField] private Button l_2;
    [SerializeField] private Button l_3;
    [SerializeField] private Button l_4;
    private List<Button> levelSelectionButtons = new List<Button>();
    [SerializeField] private Button l_back;

    private void Start() {

        //initialize the fade thingy
        dark.transform.localScale = Vector3.zero;
        darkSprite = dark.GetComponent<Image>();
        darkSprite.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0);

        //add all buttons to their respective lists (dont want them to be serialized twice)
        main = new List<Object>();
        main.Add(playButton);
        main.Add(tutButton);
        main.Add(quitButton);
        main.Add(creditsButton);

        tutorial.Add(t_close);

        credits.Add(c_close);

        //initialize the menu
        ChangeState(main, true);
        ChangeState(tutorial, false);
        ChangeState(credits, false);
        ChangeState(levels, false);

        mainObj.SetActive(true);
        tutorialObj.SetActive(false);
        creditsObj.SetActive(false);
        levelsObj.SetActive(false);

        //make the buttons be buttons
        playButton.onClick.AddListener(OpenLevelSelector);
        quitButton.onClick.AddListener(Quit);
        tutButton.onClick.AddListener(OpenTut);
        creditsButton.onClick.AddListener(OpenCredits);

        t_close.onClick.AddListener(CloseTut);

        l_back.onClick.AddListener(CloseLevelSelector);
        l_1.onClick.AddListener(openl1);
        l_2.onClick.AddListener(openl2);
        l_3.onClick.AddListener(openl3);
        l_4.onClick.AddListener(openl4);

        c_close.onClick.AddListener(CloseCredits);

        //load level data (need to read/write files)
        levelSelectionButtons.Add(l_1); levelSelectionButtons.Add(l_2);
        levelSelectionButtons.Add(l_3); levelSelectionButtons.Add(l_4);
        for (int i = 0; i < levelUnlockStatuses.Length; i++)
            if (levelUnlockStatuses[i])
                levelSelectionButtons[i].interactable = true;

    }


    #region Generic
    private void ChangeState(List<Object> l, bool b) {
        //show/hide (b = true/false) every object in List l
        foreach (Object obj in l) {
            if (obj is Button) {
                Button temp = obj as Button;
                temp.gameObject.SetActive(b);
            } else if (obj is GameObject) {
                GameObject temp = obj as GameObject;
                temp.SetActive(b);
            }
        }
    }

    private IEnumerator FadeToDark() {
        //lame ahh anim
        float t = 0;
        Color colorEnd = Color.black;
        Color colorStart = darkSprite.color;
        Vector3 scaleEnd = new Vector3(1.5f, 2.8f, 4.9f);
        Vector3 scaleStart = scaleEnd / 10;
        while (t < fadeToDarkTime) {
            darkSprite.color = Color.Lerp(colorStart, colorEnd, t / fadeToDarkTime);
            dark.transform.localScale = Vector3.Lerp(scaleStart, scaleEnd, t / fadeToDarkTime);
            t += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        darkSprite.color = colorEnd;
        dark.transform.localScale = scaleEnd;

    }
    #endregion

    #region Main

    private void Quit() {
        Application.Quit();
    }

    #endregion

    #region Tutorial

    private void OpenTut() {

        tutorialObj.SetActive(true);
        ChangeState(main, false);
        ChangeState(tutorial, true);

    }

    private void CloseTut() {

        ChangeState(main, true);
        ChangeState(tutorial, false);
        tutorialObj.SetActive(false);

    }

    #endregion

    #region Level Selector
    private void OpenLevelSelector() {
        mainObj.SetActive(false);
        ChangeState(main, false);
        ChangeState(levels, true);
        levelsObj.SetActive(true);
    }

    private void CloseLevelSelector() {
        mainObj.SetActive(true);
        ChangeState(main, true);
        ChangeState(levels, false);
        levelsObj.SetActive(false);

    }

    private void openl1() { SceneManager.LoadScene(levelSceneIndices[0]); }
    private void openl2() { SceneManager.LoadScene(levelSceneIndices[1]); }
    private void openl3() { SceneManager.LoadScene(levelSceneIndices[2]); }
    private void openl4() { SceneManager.LoadScene(levelSceneIndices[3]); }

    #endregion

    #region credits
    private void OpenCredits() {

        creditsObj.SetActive(true);
        ChangeState(main, false);
        ChangeState(credits, true);

    }

    private void CloseCredits() {

        ChangeState(main, true);
        ChangeState(credits, false);
        creditsObj.SetActive(false);

    }
    #endregion
}

