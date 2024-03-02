using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject playerSprite;
    private SpriteRenderer spriteRenderer;
    private Rigidbody rb;
    private Animator animator;
    private bool canIdle = true;

    [Header("Sprites")]
    [SerializeField][Tooltip("Forward: 0 | Backward: 1 | Left: 2 | Right: 3")] private Sprite[] directionSprites;
    [SerializeField][Tooltip("Forward: 0 | Backward: 1 | Left: 2 | Right: 3 (CORRELATES WITH directionSprites")] private string[] animationStates;
    [SerializeField] private float timeToIdle;

    [Header("Mechanics")]
    private bool[] mechanicStatuses;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float flipTime;
    private float horizontalInput;
    private float verticalInput;
    private float moveSpeed;
    private Coroutine flipCoroutine;
    private Coroutine idleCoroutine;


    [Header("Boredom")]
    [SerializeField] private float boredomMax;
    [SerializeField] private float boredomDecayRate;
    [SerializeField] private float boredomRecoveryRate;
    [SerializeField] private float boredomFatigueThreshold; //ex: if this is 0.3, then when under 30% of boredom, you get fatigued
    [SerializeField] private float fatigueSpeedModifier;
    private float boredom;

    [Header("Tasks")]
    private Task currTask;

    [Header("Phone")]
    private bool hasPhoneOut;

    [Header("Interactables")]
    [SerializeField] private SpriteRenderer interactKeyIcon;
    [SerializeField] private float iconFadeDuration;
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactMask;
    private Tweener keyIconTweenIn;
    private Tweener keyIconTweenOut;
    private bool keyIconVisible;
    private Color startColor;

    [Header("Keybinds")]
    [SerializeField] private KeyCode interactKey;

    private TaskManager taskManager;


    private void Start() {

        rb = GetComponent<Rigidbody>();
        spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        animator = playerSprite.GetComponent<Animator>();

        moveSpeed = baseMoveSpeed;

        // default all mechanics to true
        mechanicStatuses = new bool[Enum.GetValues(typeof(MechanicType)).Length];

        foreach (MechanicType mechanicType in Enum.GetValues(typeof(MechanicType)))
            mechanicStatuses[(int)mechanicType] = true;

        boredom = boredomMax * 0.7f;
        StartCoroutine(TickBoredom());

        startColor = interactKeyIcon.color;
        interactKeyIcon.gameObject.SetActive(false);
        interactKeyIcon.color = Color.clear; // set to clear for fade in

        taskManager = FindObjectOfType<GameManager>().GetComponent<TaskManager>();
        StopCoroutine(flipCoroutine = StartCoroutine(DoAFlip(1)));
        canIdle = true;
    }

    private void Update() {

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        animator.SetBool("isWalking", !(horizontalInput == 0f && verticalInput == 0f));

        if (Input.GetKeyDown(KeyCode.A)) {
            canIdle = true;
            StopCoroutine(flipCoroutine);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleRight"))
                animator.Play(animationStates[3]);
            else
                flipCoroutine = StartCoroutine(DoAFlip(1, 3));
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            canIdle = true;
            StopCoroutine(flipCoroutine);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleLeft"))
                animator.Play(animationStates[2]);
            else
                flipCoroutine = StartCoroutine(DoAFlip(-1, 2));
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            canIdle = true;
            StopCoroutine(flipCoroutine);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleBack"))
                animator.Play(animationStates[0]);
            else
                flipCoroutine = StartCoroutine(DoAFlip(1, 0));
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            canIdle = true;
            StopCoroutine(flipCoroutine);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IdleForward"))
                animator.Play(animationStates[1]);
            else
                flipCoroutine = StartCoroutine(DoAFlip(1, 1));
        }
        if (isIdle() && canIdle)
            idleCoroutine = StartCoroutine(GoIdle());

        if (verticalInput > 0f)

            if (Input.GetKey(KeyCode.Space))
                hasPhoneOut = true;

        if (Input.GetKeyUp(KeyCode.Space))
            hasPhoneOut = false;

        if (boredom > boredomMax)
            boredom = boredomMax;

        if (boredom < 0f) //should end game
            boredom = 1f;

        if (boredom < boredomMax * boredomFatigueThreshold)
            moveSpeed = baseMoveSpeed * fatigueSpeedModifier;
        else
            moveSpeed = baseMoveSpeed;

        Interactable interactable = null;
        foreach (Collider obj in Physics.OverlapSphere(transform.position, interactRadius, interactMask)) {
            interactable = obj?.GetComponent<Interactable>();
            break;
        }
        //Physics2D.OverlapCircle(transform.position, interactRadius, interactMask)?.GetComponent<Interactable>(); // get interactable

        if (interactable != null) { // if interactable is not null

            // show interact key icon and add to detected interactables list
            ShowInteractKeyIcon();

            if (Input.GetKeyDown(interactKey)) // check for interact key press
                interactable.Interact();

        }
        else {

            HideInteractKeyIcon(); // if no interactables in range, hide interact key icon

        }
    }

    private IEnumerator GoIdle() {
        print("Idling");
        canIdle = false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkRight"))
            animator.Play("IdleRight");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkLeft"))
            animator.Play("IdleLeft");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkBack"))
            animator.Play("IdleBack");
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("WalkForward"))
            animator.Play("IdleForward");
        float t = 0;
        while (t < timeToIdle) {
            if (!isIdle())
                StopCoroutine(idleCoroutine);
            t += Time.deltaTime;
            yield return null;
        }
        if (isIdle() && !animator.GetCurrentAnimatorStateInfo(0).IsName("IdleForward")) {
            StartCoroutine(DoAFlip(1));
            animator.Play("IdleForward");
        }
    }


    private IEnumerator DoAFlip(float dir) {
        //StopCoroutine(flipCoroutine);
        //dir = dir.normalized;
        dir /= dir; //normalize
        Quaternion startRot = transform.rotation;
        float t = 0;
        while (t < flipTime) {
            playerSprite.transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(0f, dir * 180f, 0f), t / flipTime);
            t += Time.deltaTime;
            yield return null;
        }
        playerSprite.transform.rotation = Quaternion.Euler(0f, dir * 180f, 0f);
    }

    private IEnumerator DoAFlip(float dir, int state) {
        //StopCoroutine(flipCoroutine);
        //dir = dir.normalized;
        dir /= dir; //normalize
        Quaternion startRot = transform.rotation;
        float t = 0;
        bool hasChangedState = false;
        while (t < flipTime) {
            playerSprite.transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(0f, dir * 180f, 0f), t / flipTime);
            if (t >= flipTime / 2f && !hasChangedState) {
                spriteRenderer.sprite = directionSprites[state];
                animator.Play(animationStates[state]);
                hasChangedState = true;
            }
            t += Time.deltaTime;

            yield return null;
        }
        playerSprite.transform.rotation = Quaternion.Euler(0f, dir * 180f, 0f);
    }

    private void FixedUpdate() {

        if (mechanicStatuses[(int)MechanicType.Movement])
            rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

    }

    private IEnumerator TickBoredom() {

        while (true) {

            if (hasPhoneOut)
                boredom += boredomRecoveryRate / 1f;
            else
                boredom -= boredomDecayRate / 1f;

            yield return new WaitForSeconds(1);

        }
    }


    public void ShowInteractKeyIcon() {

        if (keyIconVisible) return;

        if (keyIconTweenOut != null && keyIconTweenOut.IsActive()) keyIconTweenOut.Kill();

        keyIconVisible = true;
        interactKeyIcon.gameObject.SetActive(true);
        interactKeyIcon.DOColor(startColor, iconFadeDuration);

    }

    public void HideInteractKeyIcon() {

        if (!keyIconVisible) return;

        if (keyIconTweenIn != null && keyIconTweenIn.IsActive()) keyIconTweenIn.Kill();

        keyIconVisible = false;
        keyIconTweenOut = interactKeyIcon.DOColor(Color.clear, iconFadeDuration).OnComplete(() => interactKeyIcon.gameObject.SetActive(false));

    }

    public void SetMechanicStatus(MechanicType mechanicType, bool status) {

        mechanicStatuses[(int)mechanicType] = status;

    }

    private bool isIdle() {
        return horizontalInput == 0f && verticalInput == 0f;
    }
}
