using UnityEngine;
using UnityEngine.EventSystems;

public class DragAcrossScreen :
    MonoBehaviour,
    IDragHandler
{
    public Transform cameraRig;

    public float dragSpeed = 0.01f;

    public void OnDrag(PointerEventData eventData)
    {

        Vector3 move = new Vector3(
            -eventData.delta.x * dragSpeed,
            -eventData.delta.y * dragSpeed,
            0f
        );

        cameraRig.position += move;
    }
}