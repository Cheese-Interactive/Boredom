using DG.Tweening;
using UnityEngine;

public class MenuPlayer : MonoBehaviour {

    [Header("Movement")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float delay;
    [SerializeField] private float moveDuration;
    private Vector3 startPoint;
    private bool toTarget;
    private Tweener tweener;

    private void Start() {

        startPoint = transform.position;
        toTarget = true;
        transform.Rotate(0f, 180f, 0f); // place this here so it rotates to normal position when the game starts

    }

    private void FixedUpdate() {

        if (tweener != null && tweener.IsActive()) return;

        transform.Rotate(0f, 180f, 0f);

        tweener = transform.DOMove(toTarget ? targetPoint.position : startPoint, moveDuration).SetDelay(delay).OnComplete(() => {

            toTarget = !toTarget;

        }).SetEase(Ease.Linear);
    }
}
