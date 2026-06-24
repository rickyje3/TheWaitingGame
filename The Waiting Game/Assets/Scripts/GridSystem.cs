using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridSystem : MonoBehaviour
{
    public GameObject objectToPlace;
    public float gridSize = 10f; // The higher this is the less smooth it is (more grid like)
    private GameObject ghostObject; // The object that shows where the position of the placed object would be
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    public int RotationX;
    public int RotationY;

    [SerializeField] private LayerMask floorMask;

    private bool canPlace;


    private void Start()
    {
        CreateGhostObject();
    }


    private void Update()
    {
        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0) && canPlace)
            PlaceObject();

        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateGhostObjectCounterClockwise();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateGhostObjectClockwise();
        }
    }


    void CreateGhostObject()
    {
        ghostObject = Instantiate(
            objectToPlace,
            Vector3.zero,
            Quaternion.Euler(RotationX, RotationY, 0)
        );

        Collider col = ghostObject.GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        Renderer[] renderers =
            ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;

            Color color = mat.color;
            color.a = 0.5f;
            mat.color = color;
        }
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //ghostObject.transform.rotation = Quaternion.Euler(RotationX, RotationY, 0);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(hit.point.x / gridSize) * gridSize,
                0f,
                Mathf.Round(hit.point.z / gridSize) * gridSize
            );

            Collider ghostCollider =
                ghostObject.GetComponent<Collider>();

            if (ghostCollider != null)
            {
                snappedPosition.y =
                    hit.point.y +
                    ghostCollider.bounds.size.y;
            }
            else
            {
                snappedPosition.y = hit.point.y;
            }

            ghostObject.transform.position =
                snappedPosition;

            canPlace = CanPlaceObject();

            if (canPlace)
            {
                SetGhostColor(new Color(0f, 1f, 0f, 0.5f));
            }
            else
            {
                SetGhostColor(new Color(1f, 0f, 0f, 0.5f));
            }
        }
    }


    bool CanPlaceObject()
    {
        Collider ghostCollider =
            ghostObject.GetComponent<Collider>();

        if (ghostCollider == null)
            return true;

        Collider[] overlaps =
            Physics.OverlapBox(
                ghostCollider.bounds.center,
                ghostCollider.bounds.extents * 0.95f,
                ghostObject.transform.rotation
            );

        foreach (Collider col in overlaps)
        {
            if (col.gameObject == ghostObject)
                continue;

            return false;
        }

        return true;
    }

    public void RotateGhostObjectCounterClockwise()
    {
        ghostObject.transform.Rotate(0, 90, 0);
    }

    public void RotateGhostObjectClockwise()
    {
        ghostObject.transform.Rotate(0, -90, 0);
    }

    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }

    void PlaceObject()
    {
        Vector3 placementPosition = ghostObject.transform.position;

        if (!occupiedPositions.Contains(placementPosition))
        {
            Instantiate(objectToPlace, placementPosition, ghostObject.transform.rotation); // Place the object at the ghosts position

            occupiedPositions.Add(placementPosition); // Mark this position as occupied
        }
    }
}
