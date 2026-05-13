using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleWorldObject : MonoBehaviour, IDragHandler
{
    public Camera cam;
    public float zoomSpeed = 0.01f;

    public float minZoom = .2f;
    public float maxZoom = 2f;

    public void OnDrag(PointerEventData eventData)
    {
        cam.orthographicSize -= eventData.delta.x * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}
