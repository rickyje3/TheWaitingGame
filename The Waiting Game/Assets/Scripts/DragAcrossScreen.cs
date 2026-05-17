using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragAcrossScreen :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler
{
    public Transform target;
    public Camera cam;

    private Vector3 lastWorldPoint;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastWorldPoint = GetWorldPoint(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 currentWorldPoint =
            GetWorldPoint(eventData.position);

        Vector3 delta =
            currentWorldPoint - lastWorldPoint;

        // invert for natural panning
        target.position -= new Vector3(
            delta.x,
            delta.y,
            0f
        );

        lastWorldPoint = currentWorldPoint;
    }

    Vector3 GetWorldPoint(Vector2 screenPos)
    {
        return cam.ScreenToWorldPoint(
            new Vector3(
                screenPos.x,
                screenPos.y,
                Mathf.Abs(cam.transform.position.z)
            )
        );
    }
}