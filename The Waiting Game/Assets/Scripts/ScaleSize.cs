using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleSize : MonoBehaviour, IDragHandler
{
    //scaled resolution, that created mirror effect. Scaled island size but it didnt scale ui proportionally .

    public Camera islandCamera;

    public float zoomSpeed = 0.01f;

    public float minZoom = 2f;
    public float maxZoom = 20f;

    public void OnDrag(PointerEventData eventData)
    {
        islandCamera.orthographicSize -=
            eventData.delta.x * zoomSpeed;

        islandCamera.orthographicSize =
            Mathf.Clamp(
                islandCamera.orthographicSize,
                minZoom,
                maxZoom
            );
    }
}
