using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleSize : MonoBehaviour, IDragHandler
{
    //scaled resolution, that created mirror effect. Scaled island size but it didnt scale ui proportionally .

    public Camera cam;

    public float zoomSpeed = 0.01f;

    public float minZoom = 2f;
    public float maxZoom = 20f;

    public void OnDrag(PointerEventData eventData)
    {
        cam.orthographicSize -=
            eventData.delta.x * zoomSpeed;

        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize,
            minZoom,
            maxZoom
        );
    }
}

