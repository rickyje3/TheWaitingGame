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

    [Range(0f, 1f)]
    public float uiMovementStrength = 0.25f;


    private float startZoom;

    private Vector2 startMouse;

    private Vector2[] startPositions;


    // =========================================================
    // BEGIN DRAG
    // =========================================================

    public void OnBeginDrag(PointerEventData eventData)
    {
        startZoom =
            cam.orthographicSize;

        startMouse =
            eventData.position;


        // -----------------------------------------------------
        // Remember the starting position of each UI child
        // -----------------------------------------------------

        startPositions =
            new Vector2[uiRoot.childCount];


        for (int i = 0;
             i < uiRoot.childCount;
             i++)
        {
            RectTransform child =
                uiRoot.GetChild(i) as RectTransform;


            if (child != null)
            {
                startPositions[i] =
                    child.anchoredPosition;
            }
        }
    }


    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        float dragAmount =
            eventData.position.x -
            startMouse.x;


        float newZoom =
            startZoom -
            dragAmount * zoomSpeed;


        newZoom =
            Mathf.Clamp(
                newZoom,
                minZoom,
                maxZoom
            );


        // -----------------------------------------------------
        // Change camera zoom
        // -----------------------------------------------------

        cam.orthographicSize =
            newZoom;


        // -----------------------------------------------------
        // Calculate how much the UI should move
        // -----------------------------------------------------

        float zoomRatio =
            newZoom / startZoom;


        float adjustedRatio =
            Mathf.Lerp(
                1f,
                zoomRatio,
                uiMovementStrength
            );


        // -----------------------------------------------------
        // Apply movement
        // -----------------------------------------------------

        for (int i = 0;
             i < uiRoot.childCount;
             i++)
        {
            RectTransform child =
                uiRoot.GetChild(i) as RectTransform;


            if (child == null)
                continue;


            child.anchoredPosition =
                startPositions[i] *
                adjustedRatio;
        }


        // -----------------------------------------------------
        // Camera projection changed intentionally
        // -----------------------------------------------------

        if (dragAcrossScreen != null)
        {
            dragAcrossScreen.ResetCameraTracking();
        }
    }
}