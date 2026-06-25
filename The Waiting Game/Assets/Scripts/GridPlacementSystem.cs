using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cursorIndicator; // attach the parent here
    [SerializeField] private GridInputManager gridInputManager;
    public Grid grid;


    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = gridInputManager.GetSelectedMousePosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position =
            mousePosition + Vector3.up * 0.5f;

        cursorIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
