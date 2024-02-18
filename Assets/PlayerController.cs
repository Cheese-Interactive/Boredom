using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Stats")]
    [SerializeField] private float speed;
    private float baseSpeed;
    private float boredom;
    [SerializeField] private float boredomMax;
    [SerializeField] private float boredomDecayRate;
    [SerializeField] private float boredomRecoveryRate;
    [SerializeField] private float boredomFatigueThreshold; //ex: if this is 0.3, then when under 30% of boredom, you get fatigued
    [SerializeField] private float fatigueSpeedModifier;
    [SerializeField] private float boredomTps;
    [SerializeField] private TMP_Text tempText;
    [Header("IdkIdkIdkIdk")]
    [SerializeField] private float interactionRadius;
    [SerializeField] private bool canInteract;
    [SerializeField] private bool hasPhoneOut;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        boredom = boredomMax * 0.7f;
        StartCoroutine(TickBoredom());
        baseSpeed = speed;
    }

    // Update is called once per frame
    void Update() {
        tempText.text = " " + (int)boredom;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.Space)) {
            hasPhoneOut = true;
            rb.velocity = Vector2.zero;
        }
        else
            rb.velocity = new Vector2(x, y).normalized * speed;

        if (Input.GetKeyUp(KeyCode.Space))
            hasPhoneOut = false;

        if (boredom > boredomMax)
            boredom = boredomMax;
        if (boredom < 0) //should end game
            boredom = 1;

        if (boredom < boredomMax * boredomFatigueThreshold)
            speed = baseSpeed * fatigueSpeedModifier;
        else
            speed = baseSpeed;

    }

    private IEnumerator TickBoredom() {
        while (true) {
            yield return new WaitForSeconds(1 / boredomTps);
            if (hasPhoneOut)
                boredom += boredomRecoveryRate / boredomTps;
            else
                boredom -= boredomDecayRate / boredomTps;
        }
    }

}
