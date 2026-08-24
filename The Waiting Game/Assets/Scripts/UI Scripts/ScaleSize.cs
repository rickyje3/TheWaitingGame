using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleSize : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public Camera cam;

    public RectTransform uiRoot;

    public DragAcrossScreen dragAcrossScreen;

    public float zoomSpeed = 0.4f;

    public float minZoom = 8f;
    public float maxZoom = 25f;

    public float maxUiRootScale = 1.2f;
    public float minUiRootScale = 0.3f;

    [HideInInspector]
    public float startZoom;

    private Vector2 startMouse;

    private float startUiScale;


    // ---------------------------------------------------------
    // BEGIN DRAG
    // ---------------------------------------------------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        startZoom = cam.orthographicSize;

        startMouse = eventData.position;

        startUiScale = uiRoot.localScale.x;
    }


    // ---------------------------------------------------------
    // DRAG
    // ---------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        float dragAmount =
            eventData.position.x - startMouse.x;


        float newZoom =
            startZoom - dragAmount * zoomSpeed;


        newZoom = Mathf.Clamp(
            newZoom,
            minZoom,
            maxZoom
        );


        // -----------------------------------------------------
        // Change camera zoom
        // -----------------------------------------------------

        cam.orthographicSize = newZoom;


        // -----------------------------------------------------
        // Scale UI
        // -----------------------------------------------------

        UpdateUIScale(newZoom);


        // -----------------------------------------------------
        // Tell DragAcrossScreen that the camera projection
        // changed but the camera itself did NOT move.
        // -----------------------------------------------------

        if (dragAcrossScreen != null)
        {
            dragAcrossScreen.ResetCameraTracking();
        }
    }


    // ---------------------------------------------------------
    // UI SCALE
    // ---------------------------------------------------------

    public void UpdateUIScale(float newZoom)
    {
        float zoomRatio =
            startZoom / newZoom;


        float newScale =
            startUiScale * zoomRatio;


        newScale = Mathf.Clamp(
            newScale,
            minUiRootScale,
            maxUiRootScale
        );


        uiRoot.localScale =
            Vector3.one * newScale;
    }
}