using UnityEngine;

public class GridInputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayermask;


    public Vector3 GetSelectedMousePosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayermask))
        {
            Debug.Log("HIT: " + hit.collider.name);
            return hit.point;
        }

        Debug.Log("MISS");

        return Vector3.zero;
    }
}
