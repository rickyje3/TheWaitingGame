using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridInputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask placementLayermask;

    [SerializeField] private LayerMask hoverableLayermask;

    public event Action OnClicked, OnExit;

    public MainMenu mainMenu;
    public Image mainMenuImage;
    public LayoutGroup layoutGroup;

    public GameObject uiRoot;

    public DragAcrossScreen dragAcrossScreen;
    public ScaleSize scaleSize;


    private void Update()
    {
        // =========================================================
        // CLICK
        // =========================================================

        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
            Debug.Log("Mouse Click from " + GetEntityId());
        }


        // =========================================================
        // ESCAPE
        // =========================================================

        if (Input.GetKeyDown(KeyCode.Escape) && mainMenu.isShopOpen)
        {
            OnExit?.Invoke();

            mainMenu.CloseMenu();

            mainMenuImage.enabled = true;

            layoutGroup.gameObject.SetActive(true);
        }


        // =========================================================
        // RAYCAST DETECTION
        // =========================================================

        Vector3 mousePos =
            Input.mousePosition;

        Ray ray =
            sceneCamera.ScreenPointToRay(mousePos);

        RaycastHit hit;


        bool hitHoverable =
            Physics.Raycast(
                ray,
                out hit,
                Mathf.Infinity,
                hoverableLayermask
            );


        // =========================================================
        // UI VISIBILITY
        // =========================================================

        if (hitHoverable &&
            !mainMenu.isShopOpen ||
            dragAcrossScreen.isMoveDragging ||
            scaleSize.isScaleDragging)
        {
            uiRoot.SetActive(true);
        }
        else
        {
            uiRoot.SetActive(false);
        }
    }


    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();



    public Vector3 GetSelectedMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayermask))
        {
            lastPosition = hit.point;
        }

       

        //Debug.Log("MISS");

        return lastPosition;
    }
}
