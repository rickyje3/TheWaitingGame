using UnityEngine;
using UnityEngine.EventSystems;

public class DragAcrossScreen :
    MonoBehaviour,
    IDragHandler,
    IEndDragHandler
{
    public Transform cameraRig;

    public float dragSpeed = 0.01f;

    public bool isMoveDragging { get; private set; } = false;

    public void OnDrag(PointerEventData eventData)
    {
        isMoveDragging = true;

        Vector3 move = new Vector3(
            -eventData.delta.x * dragSpeed,
            -eventData.delta.y * dragSpeed,
            0f
        );

        cameraRig.position += move;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isMoveDragging = false;
    }
}