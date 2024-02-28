
using System.Collections;
using UnityEngine;

public class Puddle : Interactable {
    [SerializeField] private Color color;
    [SerializeField] private float alphaIterator;
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
            StartCoroutine(interactCooldown());
            color = new Color(color.r, color.g, color.b, color.a - alphaIterator);
            sprite.color = color;
        }
        if (color.a <= alphaToDestroy) {
            FindObjectOfType<TaskManager>().completeCurrentTask();
            Destroy(gameObject);
        }
    }

    private IEnumerator interactCooldown() {
        canInteract = false;
        yield return new WaitForSeconds(interactDelay);
        canInteract = true;
    }
}
