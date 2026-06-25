using UnityEngine;

public class GridPlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GridInputManager gridInputManager;
    //public Grid grid;


    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = gridInputManager.GetSelectedMousePosition();
        mouseIndicator.transform.position =
            mousePosition + Vector3.up * 0.5f;
    }
}
