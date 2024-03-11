using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    private TaskManager taskManager;
    private Rigidbody rb;
    private Animator animator;

    [Header("Mechanics")]
    private bool[] mechanicStatuses;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float flipTime;
    private float horizontalInput;
    private float verticalInput;
    private float moveSpeed;

    [Header("Boredom")]
    [SerializeField] private float boredomMax;
    [SerializeField] private float boredomDecayRate;
    [SerializeField] private float boredomRecoveryRate;
    [SerializeField] private float boredomFatigueThreshold; //ex: if this is 0.3, then when under 30% of boredom, you get fatigued
    [SerializeField] private float fatigueSpeedModifier;
    [SerializeField] private GameObject meterReference;
    private Image meter;
    [SerializeField] private Gradient meterGradient;
    private Coroutine boredomCoroutine;
    private float boredom;

    [Header("Phone")]
    private bool hasPhoneOut;

    [Header("Interactables")]
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactMask;
    private Color startColor;

    [Header("Interact Icon")]
    [SerializeField] private SpriteRenderer interactKeyIcon;
    [SerializeField] private float iconFadeDuration;
    [SerializeField] private float iconAnimScaleMultiplier;
    [SerializeField] private float iconAnimScaleDuration;
    private Vector2 iconStartScale;
    private Tweener keyIconTweenIn;
    private Tweener keyIconTweenOut;
    private bool keyIconVisible;

    [Header("Keybinds")]
    [SerializeField] private KeyCode interactKey;
    [SerializeField] private KeyCode phoneKey;

    private void Start() {

        taskManager = FindObjectOfType<TaskManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        meter = meterReference.GetComponent<Image>();

        moveSpeed = baseMoveSpeed;

        // default all mechanics to true
        mechanicStatuses = new bool[Enum.GetValues(typeof(MechanicType)).Length];

        foreach (MechanicType mechanicType in Enum.GetValues(typeof(MechanicType)))
            mechanicStatuses[(int)mechanicType] = true;

        boredom = boredomMax * 0.7f;
        meter.fillAmount = boredom;
        StartBoredomTick();

        startColor = interactKeyIcon.color;
        interactKeyIcon.gameObject.SetActive(false);
        interactKeyIcon.color = Color.clear; // set to clear for fade in

        iconStartScale = interactKeyIcon.transform.localScale;

    }

    private void Update() {

        /* PHONE */
        if (Input.GetKeyDown(phoneKey)) {

            hasPhoneOut = true;

            ResetAnimations();
            animator.SetBool(horizontalInput >= 0f ? "isPhoneOutRight" : "isPhoneOutLeft", true); // moving right or standing still, animation faces right, else left

        }
        else if (Input.GetKeyUp(phoneKey)) {

            hasPhoneOut = false;
            ResetAnimations();

        }

        if (hasPhoneOut) {

            HideInteractKeyIcon();
            return;

        }

        /* MOVEMENT */
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!hasPhoneOut) {

            // vertical movement gets priority
            if (verticalInput > 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingForward", true);

            }
            else if (verticalInput < 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingBack", true);

            }
            else if (horizontalInput < 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingLeft", true);

            }
            else if (horizontalInput > 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingRight", true);

            }
            else {

                ResetAnimations();

            }
        }

        /* INTERACTABLES */
        Interactable interactable = null;

        foreach (Collider obj in Physics.OverlapSphere(transform.position, interactRadius, interactMask)) {

            interactable = obj?.GetComponent<Interactable>();
            break;

        }

        //Physics2D.OverlapCircle(transform.position, interactRadius, interactMask)?.GetComponent<Interactable>(); // get interactable

        if (interactable != null) { // if interactable is not null

            if (interactable is TaskInteractable) {

                if (!taskManager.HasCurrentTask())
                    ShowInteractKeyIcon(); // show interact key icon if no current task and interactable is task interactable
                else
                    HideInteractKeyIcon();

            }
            else {

                ShowInteractKeyIcon();

            }

            if (Input.GetKeyDown(interactKey)) { // interact key pressed

                interactable.Interact();

                interactKeyIcon.transform.DOScale(iconStartScale * iconAnimScaleMultiplier, iconAnimScaleDuration / 2f).OnComplete(() => interactKeyIcon.transform.DOScale(iconStartScale, iconAnimScaleDuration / 2f));

            }
        }
        else {

            HideInteractKeyIcon(); // if no interactables in range, hide interact key icon

        }
    }

    private void FixedUpdate() {

        if (mechanicStatuses[(int)MechanicType.Movement] && !hasPhoneOut)
            rb.velocity = new Vector3(horizontalInput, 0, verticalInput).normalized * moveSpeed;
        else
            rb.velocity = Vector3.zero;

    }

    private void ResetAnimations() {

        animator.SetBool("isWalkingForward", false);
        animator.SetBool("isWalkingBack", false);
        animator.SetBool("isWalkingRight", false);
        animator.SetBool("isWalkingLeft", false);
        animator.SetBool("isPhoneOutLeft", false);
        animator.SetBool("isPhoneOutRight", false);

    }

    private IEnumerator TickBoredom() {

        while (true) {
            float boredomP = boredom / 100;
            meter.fillAmount = boredomP;
            meter.color = meterGradient.Evaluate(boredomP - boredomFatigueThreshold);
            //BUG: spamming space (taking phone out) while moving ticks boredom up (op)

            if (hasPhoneOut) // recover boredom
                boredom += boredomRecoveryRate;
            else // decay boredom
                boredom -= boredomDecayRate;

            if (boredom > boredomMax) // clamp boredom
                boredom = boredomMax;

            if (boredom < 0f) //should end game
                boredom = 1f;

            if (boredom < boredomMax * boredomFatigueThreshold) // modify move speed based on boredom
                moveSpeed = baseMoveSpeed * fatigueSpeedModifier;
            else
                moveSpeed = baseMoveSpeed;

            yield return new WaitForSeconds(1f);

            print(boredom);

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

    public void SetMechanicStatus(MechanicType mechanicType, bool status) { mechanicStatuses[(int)mechanicType] = status; }

    public void StartBoredomTick() { boredomCoroutine = StartCoroutine(TickBoredom()); }

    public void PauseBoredomTick() { if (boredomCoroutine != null) StopCoroutine(boredomCoroutine); }

}
