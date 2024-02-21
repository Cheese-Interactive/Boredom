using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    private Rigidbody2D rb;

    [Header("Mechanics")]
    private bool[] mechanicStatuses;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed;
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

    [Header("Interactables")]
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask interactMask;
    private List<Interactable> detectedInteractables; // to remove interact key icon when player is not in range

    [Header("Keybinds")]
    [SerializeField] private KeyCode interactKey;

    // Start is called before the first frame update
    void Start() {

        rb = GetComponent<Rigidbody2D>();

        moveSpeed = baseMoveSpeed;

        // default all mechanics to true
        mechanicStatuses = new bool[Enum.GetValues(typeof(MechanicType)).Length];

        foreach (MechanicType mechanicType in Enum.GetValues(typeof(MechanicType)))
            mechanicStatuses[(int) mechanicType] = true;

        boredom = boredomMax * 0.7f;
        StartCoroutine(TickBoredom());

        detectedInteractables = new List<Interactable>();

    }

    // Update is called once per frame
    void Update() {

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

        if (mechanicStatuses[(int) MechanicType.Movement])
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * moveSpeed;

        Interactable interactable = Physics2D.OverlapCircle(transform.position, interactRadius, interactMask)?.GetComponent<Interactable>(); // get interactable

        if (interactable != null) { // if interactable is not null

            // show interact key icon and add to detected interactables list
            interactable.ShowInteractKeyIcon();
            detectedInteractables.Add(interactable);

            if (Input.GetKeyDown(interactKey)) // check for interact key press
                interactable.Interact();

        }

        foreach (Interactable detected in detectedInteractables)
            if (detected != interactable) detected.HideInteractKeyIcon(); // hide interact key icon for all detected interactables except the current interactable

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

}
