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

    private void Start()
    {
        if (uiRoot == null)
        {
            uiRoot = GetComponentInParent<RectTransform>();
        }

        previousCameraPosition = cameraRig.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 move = new Vector3(
            -eventData.delta.x * dragSpeed,
            -eventData.delta.y * dragSpeed,
            0f
        );

        cameraRig.position += move;

        Vector3 oldScreenPosition =
            mainCamera.WorldToScreenPoint(previousCameraPosition);

        Vector3 newScreenPosition =
            mainCamera.WorldToScreenPoint(cameraRig.position);

        Vector3 screenMovement =
            newScreenPosition - oldScreenPosition;

        uiRoot.position += new Vector3(
            -screenMovement.x,
            -screenMovement.y,
            0f
        );

        previousCameraPosition = cameraRig.position;
    }

    public void ResetCameraTracking()
    {
        previousCameraPosition = cameraRig.position;
    }
}