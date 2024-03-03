using DG.Tweening;
using UnityEngine;

public class Trash : Interactable {

    [Header("References")]
    private MeshRenderer meshRenderer;

    [Header("Color")]
    [SerializeField] private float fadeDuration;
    private bool beingPickedUp;

    private void Start() {

        meshRenderer = GetComponent<MeshRenderer>();

    }

    public override void Interact() {

        if (beingPickedUp) return;

        beingPickedUp = true;

        meshRenderer.material.DOFade(0f, fadeDuration).OnComplete(() => {

            taskManager.OnTrashPickup();
            Destroy(gameObject);

        });
    }
}
