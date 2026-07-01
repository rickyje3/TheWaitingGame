using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleSize : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    //scaled resolution, that created mirror effect. Scaled island size but it didnt scale ui proportionally .

    public Camera cam;

    public RectTransform uiRoot; // ONLY the UI that should scale with the world

    public float zoomSpeed = 0.4f;

    public float minZoom = 25f; // Max amount the zoom will scale up
    public float maxZoom = 80f; // max amount the zoom will scale down

    public float maxUiRootScale = 1.2f; //Max scale that the ui root will go to
    public float minUiRootScale = 0.3f; //Min scale that the ui root will will go to

    private float startZoom; // Stores the camera zoom at the moment dragging begins
    private Vector2 startMouse; // Stores the mouse position when dragging begins
    private float startUiScale;  // Stores the UI scale before resizing starts. This prevents the UI from snapping instantly


    // Called once when the resize drag begins
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save the current zoom level
        startZoom = cam.orthographicSize;

        // Save initial mouse position
        startMouse = eventData.position;

        // Save the current UI scale
        // This fixes the instant snapping issue
        startUiScale = uiRoot.localScale.x;
    }


    // Called continuously while dragging
    public void OnDrag(PointerEventData eventData)
    {
        // Calculate horizontal mouse movement
        float dragAmount =
            eventData.position.x - startMouse.x;

        // Calculate zoom based on drag distance
        // Subtracting gives:
        // drag right = zoom in
        // drag left = zoom out
        float newZoom =
            startZoom - dragAmount * zoomSpeed;

        // Prevent zoom from going too far in/out
        newZoom = Mathf.Clamp(
            newZoom,
            minZoom,
            maxZoom
        );

        // Apply zoom to camera
        cam.orthographicSize = newZoom;

        // Scale world UI proportionally
        UpdateUIScale(newZoom);
    }


    void UpdateUIScale(float newZoom)
    {
        // Compare old zoom to new zoom
        // This creates proportional scaling
        float zoomRatio =
            startZoom / newZoom;

        // Scale UI relative to the ORIGINAL scale
        // instead of recalculating from scratch
        //
        // This fixes:
        // - snapping
        // - incorrect starting scale
        // - drifting scale values
        float newScale =
            startUiScale * zoomRatio;

        // Prevent UI from becoming too large/small
        newScale = Mathf.Clamp(
            newScale,
            minUiRootScale,
            maxUiRootScale
        );

        // Apply final scale
        uiRoot.localScale =
            Vector3.one * newScale;
    }
}


