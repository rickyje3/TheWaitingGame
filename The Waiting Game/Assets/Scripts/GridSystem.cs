using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridSystem : MonoBehaviour
{
    public GameObject objectToPlace;
    public float gridSize = 10f;
    private GameObject ghostObject; // The object that shows where the position of the placed object would be
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    public int RotationX;
    public int RotationY;


    private void Start()
    {
        CreateGhostObject();
    }


    private void Update()
    {
        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
            PlaceObject();
    }


    void CreateGhostObject()
    {
        ghostObject = Instantiate(objectToPlace, Input.mousePosition, Quaternion.Euler(RotationX, RotationY, 0));
        //only instantiates at the beginning so when we change the rotation it's not currently updating
        ghostObject.GetComponent<Collider>().enabled = false;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = 0.5f; // Set transparency
            mat.color = color;

            mat.SetFloat("_Mode", 2); // Set rendering mode to transparent 
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha); // Set blending mode for transparency
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha); // Set blending mode for transparency
            mat.SetInt("_ZWrite", 0); // Disable depth writing
            mat.DisableKeyword("_ALPHATEST_ON"); // Disable alpha testing
            mat.EnableKeyword("_ALPHABLEND_ON"); // Enable alpha blending
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // Disable premultiplied alpha
            mat.renderQueue = 3000; // Set render queue for transparent objects
        }
    }

    void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;

            Vector3 snappedPosition = new Vector3(
                Mathf.Round(point.x / gridSize) * gridSize,
                Mathf.Round(point.y / gridSize) * gridSize,
                Mathf.Round(point.z / gridSize) * gridSize);

            ghostObject.transform.position = snappedPosition;

            if (occupiedPositions.Contains(snappedPosition)) // and isn't an object u can place on top of
            {
                SetGhostColor(Color.red);
            }
            else 
                SetGhostColor(new Color(1f, 1f, 1f, 0.2f)); // Keep transparent unless occupied
        }
    }
        
    void SetGhostColor(Color color)
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
        }
    }

    void PlaceObject()
    {
        Vector3 placementPosition = ghostObject.transform.position;

        if (!occupiedPositions.Contains(placementPosition))
        {
            Instantiate(objectToPlace, placementPosition, Quaternion.Euler(RotationX, RotationY, 0)); // Place the object at the ghosts position

            occupiedPositions.Add(placementPosition); // Mark this position as occupied
        }
    }
}
