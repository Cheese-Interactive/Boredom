using UnityEngine;

public class RandomAhhScript : MonoBehaviour {
    [SerializeField] private float delay;
    [SerializeField] private float moveSpeed;
    private float dir = 1;
    float t = 0;

    private void FixedUpdate() {
        t += Time.deltaTime;
        //transform.Translate(moveSpeed * dir, 0, 0); //doesnt switch directions properly
        transform.position = new Vector3(transform.position.x + moveSpeed * dir,
                                         transform.position.y, transform.position.z);
        if (t >= delay) {
            t = 0;
            transform.Rotate(0, 180, 0);
            dir *= -1;
        }
    }
}
