using UnityEngine;
using UnityEngine.EventSystems;

public class DragAcrossScreen :
    MonoBehaviour,
    IDragHandler,
    IEndDragHandler
{
    public Transform cameraRig;

    public RectTransform uiRoot;

    public Camera mainCamera;

    public float dragSpeed = 0.01f;

    private Vector3 previousCameraPosition;

    private Vector2 previousScreenSize;

    public Canvas canvas;

    public RectTransform topLeftAnchor;

    public RectTransform topRightAnchor;

    public RectTransform bottomRightAnchor;

    public bool isMoveDragging { get; private set; } = false;


    private void Start()
    {
        //ResetCameraTracking();

        previousScreenSize =
            new Vector2(
                Screen.width,
                Screen.height
            );

        if(canvas == null)
        uiRoot.GetComponentInParent<Canvas>();
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
        isMoveDragging = true;

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


    public void OnEndDrag(PointerEventData eventData)
    {
        isMoveDragging = false;
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


        RecenterUI();


        previousScreenSize =
            new Vector2(
                Screen.width,
                Screen.height
            );


        ResetCameraTracking();
    }

    public void RecenterUI()
    {
        topRightAnchor.anchoredPosition = new Vector2(-300, -300);
        topLeftAnchor.anchoredPosition = new Vector2(300, -300);
        bottomRightAnchor.anchoredPosition = new Vector2(-300, 300);
    }
}