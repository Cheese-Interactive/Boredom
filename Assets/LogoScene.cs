using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LogoScene : MonoBehaviour {
    [SerializeField] private int menuSceneIndex;
    void Start() {
        StartCoroutine(Wait((float)GetComponent<VideoPlayer>().clip.length));
    }

    // Update is called once per frame
    private IEnumerator Wait(float t) {
        yield return new WaitForSeconds(t * 1.1f);
        SceneManager.LoadScene(menuSceneIndex);
    }
}
