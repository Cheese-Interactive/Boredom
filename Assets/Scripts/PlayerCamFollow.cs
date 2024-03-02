using System.Collections;
using UnityEngine;

public class PlayerCamFollow : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private float lerpTime;
    private Vector3 currentPos;
    private void Update() {
        currentPos = transform.position;
        StartCoroutine(lerpToTarget(target.transform.position, lerpTime));
        //doesnt work how i wanted it to
        //i stole it from rocketbox (thats prob why)
    }

    private IEnumerator lerpToTarget(Vector3 target, float duration) {
        float t = 0;
        float tEase;
        while (t < duration) {
            tEase = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(currentPos, target, tEase);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
