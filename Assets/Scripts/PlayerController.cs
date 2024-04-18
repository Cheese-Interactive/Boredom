using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject arrow;
    private Vector3 arrowTarget;
    private AudioManager audioManager;
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
    [SerializeField] private float initialBoredom;
    [SerializeField] private float boredomMax;
    [SerializeField] private float boredomDecayRate;
    [SerializeField] private float boredomRecoveryRate;
    [SerializeField] private float boredomFatigueThreshold; //ex: if this is 0.3, then when under 30% of boredom, you get fatigued
    [SerializeField] private float fatigueSpeedModifier;
    [SerializeField] private float boredomMultiplier;
    [SerializeField] private float flashWaitDuration;
    [SerializeField] private GameObject meterReference;
    [SerializeField] private Gradient meterGradient;
    [SerializeField] private TMP_Text boredomText;
    private Image meter;
    private Coroutine boredomCoroutine;
    private float boredom;
    private bool isAnimatingBoredom;
    private Tweener fillTweener;
    private Coroutine flashMeterCoroutine;

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

        arrowTarget = Vector3.zero;
        SetArrowVisible(true);
        audioManager = FindObjectOfType<AudioManager>();
        taskManager = FindObjectOfType<TaskManager>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        meter = meterReference.GetComponent<Image>();

        moveSpeed = baseMoveSpeed;

        // default all mechanics to true
        mechanicStatuses = new bool[Enum.GetValues(typeof(MechanicType)).Length];

        foreach (MechanicType mechanicType in Enum.GetValues(typeof(MechanicType)))
            mechanicStatuses[(int) mechanicType] = true;

        boredom = initialBoredom;
        boredomText.text = $"{boredom}";
        meter.fillAmount = boredom / 100f;
        meter.color = meterGradient.Evaluate(boredom - boredomFatigueThreshold);
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

        } else if (Input.GetKeyUp(phoneKey)) {

            hasPhoneOut = false;
            ResetAnimations();

        }

        if (hasPhoneOut) {
            audioManager.PlaySound(AudioManager.GameSoundEffectType.PhoneLoop);
            HideInteractKeyIcon();
            return;

        }

        /* MOVEMENT */
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput != 0 || horizontalInput != 0)
            audioManager.PlaySound(AudioManager.GameSoundEffectType.WalkLoop);

        if (!hasPhoneOut) {

            // vertical movement gets priority
            if (verticalInput > 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingForward", true);

            } else if (verticalInput < 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingBack", true);

            } else if (horizontalInput < 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingLeft", true);

            } else if (horizontalInput > 0f) {

                ResetAnimations();
                animator.SetBool("isWalkingRight", true);

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

        if (interactable != null && interactable.IsInteractable()) { // in range and is interactable

            if (Input.GetKeyDown(interactKey)) { // interact key pressed

                if (interactable is TaskInteractable)
                    interactable.SetInteractable(false);

                interactable.Interact();

                interactKeyIcon.transform.DOScale(iconStartScale * iconAnimScaleMultiplier, iconAnimScaleDuration / 2f).OnComplete(() => interactKeyIcon.transform.DOScale(iconStartScale, iconAnimScaleDuration / 2f));

            } else if (interactable is TaskInteractable && interactable.IsInteractable() && !taskManager.IsTaskStarted()) { // task interactable, has current task

                ShowInteractKeyIcon();

            } else if (interactable is not TaskInteractable && interactable.IsInteractable()) { // anything other than task interactable, but key not pressed

                ShowInteractKeyIcon();

            }
        } else {

            HideInteractKeyIcon(); // if no interactables in range, hide interact key icon

        }

        /* ANIMATIONS */
        if (isAnimatingBoredom && !hasPhoneOut)
            isAnimatingBoredom = false;

        /* ARROW */
        /* Vector3 dir = arrowTarget - arrow.transform.position;
         Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
         arrow.transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);*/
        arrow.transform.LookAt(arrowTarget);

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

            float prevBoredom = boredom;

            if (hasPhoneOut) // recover boredom
                boredom++;
            else // decay boredom
                boredom--;

            if (boredom > boredomMax) // clamp boredom
                boredom = boredomMax;

            if (boredom <= 0f)
                taskManager.OnGameLoss();

            if (boredom < boredomMax * boredomFatigueThreshold) { // modify move speed based on boredom

                moveSpeed = baseMoveSpeed * fatigueSpeedModifier;

                if (flashMeterCoroutine == null)
                    flashMeterCoroutine = StartCoroutine(FlashMeter()); // start flashing meter

            } else {

                moveSpeed = baseMoveSpeed;

                if (flashMeterCoroutine != null) {

                    StopCoroutine(flashMeterCoroutine); // stop flashing meter
                    flashMeterCoroutine = null;

                }
            }

            isAnimatingBoredom = true; // flag to prevent spamming space (taking phone out) while moving from ticking boredom up (op)

            if (fillTweener != null && fillTweener.IsActive()) fillTweener.Kill(); // kill previous tween if still active

            float boredomP = boredom / 100f;
            float duration = Math.Abs(prevBoredom - boredom) * boredomMultiplier; // uses difference in boredom to make animation smooth rather than incremental
            boredomText.text = boredom + "";
            meter.fillAmount = boredomP;
            //fillTweener = meter.DOFillAmount(boredomP, duration).OnStepComplete(() => print(meter.fillAmount)); // for smoothing, doesn't work when it hits 100

            if (flashMeterCoroutine == null) // don't change color if flashing
                meter.DOColor(meterGradient.Evaluate(boredomP - boredomFatigueThreshold), duration);

            //BUG: spamming space (taking phone out) while moving ticks boredom up (op)

            if (hasPhoneOut)
                yield return new WaitForSeconds(1f / boredomRecoveryRate);
            else
                yield return new WaitForSeconds(1f / boredomDecayRate);

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

    public void StartBoredomTick() { boredomCoroutine = StartCoroutine(TickBoredom()); }

    public void PauseBoredomTick() { if (boredomCoroutine != null) StopCoroutine(boredomCoroutine); }

    public void SetArrowVisible(bool t) { arrow.SetActive(t); }

    public void PointArrow(Vector3 position) {

        arrowTarget = position;

    }

    private IEnumerator FlashMeter() {

        while (true) {

            meter.color = Color.clear;
            yield return new WaitForSeconds(flashWaitDuration);
            meter.color = meterGradient.Evaluate(boredom / 100f - boredomFatigueThreshold);
            yield return new WaitForSeconds(flashWaitDuration);

        }
    }
}
