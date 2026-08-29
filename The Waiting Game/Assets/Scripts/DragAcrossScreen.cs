using UnityEngine;
using UnityEngine.EventSystems;

public class DragAcrossScreen :
    MonoBehaviour,
    IDragHandler
{
    public Transform cameraRig;

    public RectTransform uiRoot;

    public Camera mainCamera;

    public float dragSpeed = 0.01f;


    private Vector3 previousCameraPosition;

    private Vector2 previousScreenSize;


    private void Start()
    {
        ResetCameraTracking();

        previousScreenSize =
            new Vector2(
                Screen.width,
                Screen.height
            );
    }


    private void Update()
    {
        HandleWindowResize();
    }


    // =========================================================
    // DRAG
    // =========================================================

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 oldCameraPosition =
            cameraRig.position;


        // -----------------------------------------------------
        // Move camera
        // -----------------------------------------------------

        Vector3 move = new Vector3(
            -eventData.delta.x * dragSpeed,
            -eventData.delta.y * dragSpeed,
            0f
        );

        cameraRig.position += move;


        // -----------------------------------------------------
        // Calculate camera screen movement
        // -----------------------------------------------------

        Vector3 oldScreenPosition =
            mainCamera.WorldToScreenPoint(
                oldCameraPosition
            );

        Vector3 newScreenPosition =
            mainCamera.WorldToScreenPoint(
                cameraRig.position
            );


        // -----------------------------------------------------
        // Get Canvas
        // -----------------------------------------------------

        Canvas canvas =
            uiRoot.GetComponentInParent<Canvas>();

        if (canvas == null)
            return;


        RectTransform canvasRect =
            canvas.GetComponent<RectTransform>();


        Vector2 oldCanvasPosition;
        Vector2 newCanvasPosition;


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            oldScreenPosition,
            canvas.worldCamera,
            out oldCanvasPosition
        );


        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            newScreenPosition,
            canvas.worldCamera,
            out newCanvasPosition
        );


        Vector2 canvasDelta =
            newCanvasPosition -
            oldCanvasPosition;


        // -----------------------------------------------------
        // Keep the direction that works
        // -----------------------------------------------------

        uiRoot.anchoredPosition -= canvasDelta;


        previousCameraPosition =
            cameraRig.position;
    }


    // =========================================================
    // WINDOW RESIZE
    // =========================================================

    private void HandleWindowResize()
    {
        Vector2 currentScreenSize =
            new Vector2(
                Screen.width,
                Screen.height
            );


        if (currentScreenSize == previousScreenSize)
            return;


        // -----------------------------------------------------
        // Calculate change in the camera's viewport center
        // -----------------------------------------------------

        Vector2 oldCenter =
            previousScreenSize * 0.5f;

        Vector2 newCenter =
            currentScreenSize * 0.5f;


        Vector2 centerDelta =
            newCenter -
            oldCenter;


        // -----------------------------------------------------
        // Move UI with the camera's new viewport
        //
        // This does NOT scale the UI.
        // -----------------------------------------------------

        Canvas canvas =
            uiRoot.GetComponentInParent<Canvas>();


        if (canvas != null)
        {
            RectTransform canvasRect =
                canvas.GetComponent<RectTransform>();


            Vector2 oldCanvasPosition;
            Vector2 newCanvasPosition;


            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Vector2.zero,
                canvas.worldCamera,
                out oldCanvasPosition
            );


            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                centerDelta,
                canvas.worldCamera,
                out newCanvasPosition
            );


            Vector2 canvasDelta =
                newCanvasPosition -
                oldCanvasPosition;


            uiRoot.anchoredPosition +=
                canvasDelta;
        }


        previousScreenSize =
            currentScreenSize;
    }


    // =========================================================
    // RESET CAMERA TRACKING
    // =========================================================

    public void ResetCameraTracking()
    {
        previousCameraPosition =
            cameraRig.position;
    }


    // =========================================================
    // RECENTER
    // =========================================================

    public void Recenter()
    {
        cameraRig.position =
            Vector3.zero;


        Canvas canvas =
            uiRoot.GetComponentInParent<Canvas>();


        if (canvas != null)
        {
            uiRoot.anchoredPosition =
                Vector2.zero;
        }


        previousScreenSize =
            new Vector2(
                Screen.width,
                Screen.height
            );


        ResetCameraTracking();
    }
}