using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Sprites")]
    [SerializeField][Tooltip("Forward: 0 | Backward: 1 | Left: 2 | Right: 3")] private Sprite[] directionSprites;

    [Header("Mechanics")]
    private bool[] mechanicStatuses;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
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

    private void Start() {

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

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

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        animator.SetBool("isWalking", !(horizontalInput == 0f && verticalInput == 0f));

        if (verticalInput > 0f)
            spriteRenderer.sprite = directionSprites[0];
        else if (verticalInput < 0f)
            spriteRenderer.sprite = directionSprites[1];
        else if (horizontalInput < 0f)
            spriteRenderer.sprite = directionSprites[2];
        else if (horizontalInput > 0f)
            spriteRenderer.sprite = directionSprites[3];

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

        TaskInteractable interactable = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask)?.GetComponent<TaskInteractable>(); // get interactable

        if (interactable != null) { // if interactable is not null

            // show interact key icon and add to detected interactables list
            ShowInteractKeyIcon();

            if (Input.GetKeyDown(interactKey)) // check for interact key press
                interactable.Interact();

        } else {

            HideInteractKeyIcon(); // if no interactables in range, hide interact key icon

        }
    }

    private void FixedUpdate() {

        if (mechanicStatuses[(int) MechanicType.Movement])
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

    }

    private IEnumerator TickBoredom() {

        while (true) {

            if (hasPhoneOut)
                boredom += boredomRecoveryRate / 1f;
            else
                boredom -= boredomDecayRate / 1f;

            yield return new WaitForSeconds(1f);

        }
    }

    public bool AssignTask(Task task) {

        if (currTask != null && !currTask.IsComplete()) return false;

        currTask = task;
        return true;

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
}
