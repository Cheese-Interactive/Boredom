
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Puddle : Interactable {

    [SerializeField] private Color color;
    [SerializeField] private float alphaIterator;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float alphaToDestroy;
    [SerializeField] private float interactDelay;
    private bool canInteract = true;
    private SpriteRenderer sprite;

    private void Start() {

        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;

    }

    public override void Interact() {

        if (canInteract) {

            StartCoroutine(InteractCooldown());
            sprite.DOColor(new Color(color.r, color.g, color.b, color.a - alphaIterator), fadeDuration);

        }

        if (color.a <= alphaToDestroy) {

            FindObjectOfType<TaskManager>().CompleteCurrentTask();
            Destroy(gameObject);

        }
    }

    private IEnumerator InteractCooldown() {

        canInteract = false;
        yield return new WaitForSeconds(interactDelay);
        canInteract = true;

    }
}