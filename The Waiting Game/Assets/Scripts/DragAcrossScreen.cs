using UnityEngine;
using UnityEngine.EventSystems;

public class DragAcrossScreen :
    MonoBehaviour,
    IDragHandler
{
    public Transform cameraRig;

    public RectTransform uiRoot;

    public Camera mainCamera;

    public Transform cameraCenter;

    public float dragSpeed = 0.01f;

    private Vector3 previousCameraPosition;


    private void Start()
    {
        ResetCameraTracking();
    }


    // ---------------------------------------------------------
    // DRAG
    // ---------------------------------------------------------

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 move = new Vector3(
            -eventData.delta.x * dragSpeed,
            -eventData.delta.y * dragSpeed,
            0f
        );

        cameraRig.position += move;


        // -----------------------------------------------------
        // Calculate actual camera movement on screen
        // -----------------------------------------------------

        Vector3 oldScreenPosition =
            mainCamera.WorldToScreenPoint(previousCameraPosition);

        Vector3 newScreenPosition =
            mainCamera.WorldToScreenPoint(cameraRig.position);

        Vector3 screenMovement =
            newScreenPosition - oldScreenPosition;


        // -----------------------------------------------------
        // Move UI opposite the camera movement
        // -----------------------------------------------------

        uiRoot.position += new Vector3(
            -screenMovement.x,
            -screenMovement.y,
            0f
        );


        previousCameraPosition =
            cameraRig.position;
    }


    // ---------------------------------------------------------
    // RESET CAMERA TRACKING
    // ---------------------------------------------------------

    public void ResetCameraTracking()
    {
        previousCameraPosition =
            cameraRig.position;
    }


    // ---------------------------------------------------------
    // RECENTER
    // ---------------------------------------------------------

    public void Recenter()
    {
        // -----------------------------------------------------
        // RECENTER CAMERA
        // -----------------------------------------------------

        if (cameraCenter != null)
        {
            cameraRig.position =
                cameraCenter.position;
        }
        else
        {
            cameraRig.position =
                Vector3.zero;
        }


        // -----------------------------------------------------
        // RECENTER UI
        // -----------------------------------------------------

        Canvas canvas =
            uiRoot.GetComponentInParent<Canvas>();

        if (canvas != null)
        {
            RectTransform canvasRect =
                canvas.GetComponent<RectTransform>();

            uiRoot.anchoredPosition =
                canvasRect.rect.center;
        }
        else
        {
            uiRoot.position =
                new Vector3(
                    Screen.width * 0.5f,
                    Screen.height * 0.5f,
                    uiRoot.position.z
                );
        }


        // -----------------------------------------------------
        // IMPORTANT
        // -----------------------------------------------------

        // The camera has been manually repositioned, so the
        // next drag must treat THIS as the starting position.
        ResetCameraTracking();
    }
}