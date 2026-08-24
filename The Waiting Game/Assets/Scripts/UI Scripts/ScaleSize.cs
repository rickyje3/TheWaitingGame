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
        // Get the UI's current screen position
        // BEFORE changing zoom
        // -----------------------------------------------------

        Vector3 uiScreenPosition =
            RectTransformUtility.WorldToScreenPoint(
                null,
                uiRoot.position
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
        // IMPORTANT:
        // Keep the UI's screen position consistent.
        // -----------------------------------------------------

        Vector3 newUiScreenPosition =
            new Vector3(
                uiScreenPosition.x,
                uiScreenPosition.y,
                0f
            );

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRoot.parent as RectTransform,
            newUiScreenPosition,
            null,
            out localPoint
        );

        uiRoot.localPosition =
            new Vector3(
                localPoint.x,
                localPoint.y,
                uiRoot.localPosition.z
            );

        // -----------------------------------------------------
        // Camera projection changed, so tell the drag script
        // not to interpret this as camera movement.
        // -----------------------------------------------------

        if (dragAcrossScreen != null)
        {
            dragAcrossScreen.ResetCameraTracking();
        }
    }

    // ---------------------------------------------------------
    // UI SCALE
    // ---------------------------------------------------------

    private void UpdateUIScale(float newZoom)
    {
        float zoomRatio =
            startZoom / newZoom;

        float newScale =
            startUiScale * zoomRatio;

        uiRoot.localScale =
            Vector3.one * newScale;
    }
}