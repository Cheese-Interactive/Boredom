using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableElement : MonoBehaviour, IDragHandler {

    [Header("Drag")]
    private DragSlot dragSlot;

    private void Start() {

        dragSlot = GetComponentInParent<DragSlot>();

    }

    public void OnDrag(PointerEventData eventData) {

        transform.parent.SetAsFirstSibling();
        transform.position = new Vector2(transform.position.x, Input.mousePosition.y);

    }

    public DragSlot GetDragSlot() { return dragSlot; }

    public void SetDragSlot(DragSlot slot) { dragSlot = slot; }

}
