using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridInputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    public MainMenu mainMenu;
    public Image mainMenuImage;
    public LayoutGroup layoutGroup;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
            Debug.Log("Mouse Click from " + GetEntityId());
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
            mainMenu.CloseMenu();
            mainMenuImage.enabled = true;
            layoutGroup.gameObject.SetActive(true);
            Debug.Log("Exiting edit mode");
        }
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMousePosition()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayermask))
        {
            return hit.point;
        }

        //Debug.Log("MISS");

        return Vector3.zero;
    }
}
