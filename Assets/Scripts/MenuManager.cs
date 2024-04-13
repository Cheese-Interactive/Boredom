
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    [Header("Scenes")]
    [SerializeField] private int mainSceneIndex;
    [Header("Fade Out Stuff")]
    [SerializeField] private GameObject dark;
    [SerializeField] private float fadeToDarkTime;
    private Image darkSprite;
    [Header("Main")]
    [SerializeField] private List<Object> main = new List<Object>();
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutButton;
    [SerializeField] private Button quitButton;
    [Header("Tutorial")]
    [SerializeField] private List<Object> tutorial = new List<Object>();
    [SerializeField] private Button t_close;

    void Start() {
        //initialize the fade thingy
        dark.transform.localScale = Vector3.zero;
        darkSprite = dark.GetComponent<Image>();
        darkSprite.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0);

        //add all buttons to their respective lists (dont want them to be serialized twice)
        main.Add(playButton);
        main.Add(tutButton);
        main.Add(quitButton);

        tutorial.Add(t_close);

        //initialize the menu
        ChangeState(main, true);
        ChangeState(tutorial, false);


        //make the buttons be buttons
        playButton.onClick.AddListener(LoadMain);
        quitButton.onClick.AddListener(Quit);
        tutButton.onClick.AddListener(OpenTut);

        t_close.onClick.AddListener(CloseTut);
    }


    #region Generic
    private void ChangeState(List<Object> l, bool b) {
        //show/hide (b = true/false) every object in List l
        foreach (Object obj in l) {
            if (obj is Button) {
                Button temp = obj as Button;
                temp.gameObject.SetActive(b);
            }
            else if (obj is GameObject) {
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
    private void LoadMain() {
        StartCoroutine(LoadMainAnimHelper());
    }
    private IEnumerator LoadMainAnimHelper() {
        StartCoroutine(FadeToDark());
        yield return new WaitForSeconds(fadeToDarkTime + 0.5f);
        SceneManager.LoadScene(mainSceneIndex);
    }

    private void Quit() {
        StartCoroutine(QuitAnimHelper());
    }
    private IEnumerator QuitAnimHelper() {
        StartCoroutine(FadeToDark());
        yield return new WaitForSeconds(fadeToDarkTime + 0.5f);
        ChangeState(main, false);
        //todo: quit
    }


    private void OpenTut() {
        ChangeState(main, false);
        ChangeState(tutorial, true);
    }



    #endregion

    #region Tutorial

    private void CloseTut() {
        ChangeState(main, true);
        ChangeState(tutorial, false);
    }

    #endregion
}

