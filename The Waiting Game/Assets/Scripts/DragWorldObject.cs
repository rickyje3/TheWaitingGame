using UnityEngine;
using UnityEngine.EventSystems;


public class DragWorldObject : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public Transform target;
    public Camera cam;

    private Vector3 offset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = ScreenToWorld(eventData.position);
        offset = target.position - worldPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = ScreenToWorld(eventData.position);
        target.position = worldPoint + offset;
    }

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 p = cam.ScreenToWorldPoint(new Vector3(
            screenPos.x,
            screenPos.y,
            cam.nearClipPlane
        ));

        // lock Z so it doesn't drift
        p.z = target.position.z;
        return p;
    }
}
