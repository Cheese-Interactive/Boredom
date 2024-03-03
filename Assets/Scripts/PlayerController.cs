using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    private Rigidbody rb;
    private Animator animator;
    private UIController uiController;

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
    private float boredom;

    [Header("Phone")]
    private bool hasPhoneOut;
    private bool phoneAnimRight; // if player is going forward or right, animation faces right | if player is going back or left, animation faces left

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
    [SerializeField] private KeyCode phoneKey;

    private void Start() {

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        uiController = FindObjectOfType<UIController>();

        moveSpeed = baseMoveSpeed;

        // default all mechanics to true
        mechanicStatuses = new bool[Enum.GetValues(typeof(MechanicType)).Length];

        foreach (MechanicType mechanicType in Enum.GetValues(typeof(MechanicType)))
            mechanicStatuses[(int) mechanicType] = true;

        boredom = boredomMax * 0.7f;
        StartCoroutine(TickBoredom());

        startColor = interactKeyIcon.color;
        interactKeyIcon.gameObject.SetActive(false);
        interactKeyIcon.color = Color.clear; // set to clear for fade in

    }

    private void Update() {

        /* PHONE */
        if (Input.GetKeyDown(phoneKey)) {

            hasPhoneOut = true;

            ResetAnimations();
            animator.SetBool(phoneAnimRight ? "isPhoneOutRight" : "isPhoneOutLeft", true);

        } else if (Input.GetKeyUp(phoneKey)) {

            hasPhoneOut = false;
            ResetAnimations();

        }

        if (hasPhoneOut) return;

        /* MOVEMENT */
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!hasPhoneOut) {

            // vertical movement gets priority
            if (verticalInput > 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingForward", true);
                phoneAnimRight = true;

            } else if (verticalInput < 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingBack", true);
                phoneAnimRight = false;

            } else if (horizontalInput > 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingRight", true);
                phoneAnimRight = true;

            } else if (horizontalInput < 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingLeft", true);
                phoneAnimRight = false;

            } else {

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

            // show interact key icon and add to detected interactables list
            ShowInteractKeyIcon();

            if (Input.GetKeyDown(interactKey)) // check for interact key press
                interactable.Interact();

        } else {

            HideInteractKeyIcon(); // if no interactables in range, hide interact key icon

        }

        /* QUIZ */
        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(uiController.CloseQuiz(false)); // close quiz

    }

    private void FixedUpdate() {

        if (mechanicStatuses[(int) MechanicType.Movement] && !hasPhoneOut)
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

            //BUG: spamming space (taking phone out) while moving ticks boredom up (op)

            if (hasPhoneOut) { // recover boredom

                print("recover");
                boredom += boredomRecoveryRate;
            } else // decay boredom
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

    public void SetMechanicStatus(MechanicType mechanicType, bool status) { mechanicStatuses[(int) mechanicType] = status; }

}
