using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSlot : MonoBehaviour, IDropHandler {

    [Header("Slot")]
    [SerializeField] private DraggableElement currElement;

    private void Start() {

        currElement.transform.parent = transform;
        currElement.transform.localPosition = Vector2.zero;

    }

    public DraggableElement GetCurrElement() { return currElement; }

    public void SwapElements(DraggableElement newElem, DragSlot origin) {

        DraggableElement oldElem = currElement;
        SetCurrElement(newElem);
        origin.SetCurrElement(oldElem);

    }

    public void SetCurrElement(DraggableElement elem) {

        currElement = elem;
        currElement.SetDragSlot(this);
        currElement.transform.SetParent(transform, false);
        currElement.transform.localPosition = Vector2.zero;

    }

    public void OnDrop(PointerEventData eventData) {

        DraggableElement newElem = eventData.pointerDrag.gameObject.GetComponent<DraggableElement>();

        if (newElem)
            SwapElements(newElem, newElem.GetDragSlot());
        else
            transform.localPosition = Vector2.zero; // send back to drag slot

    }
}
