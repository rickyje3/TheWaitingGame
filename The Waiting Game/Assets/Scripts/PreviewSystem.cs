using System;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;

    [SerializeField] private GameObject cursorIndicator;
    [HideInInspector] public GameObject previewObject;

    [SerializeField] private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cursorIndicatorRenderer;

    public SoundFeedback soundFeedback;


    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cursorIndicator.SetActive(false);
        cursorIndicatorRenderer = cursorIndicator.GetComponentInChildren<Renderer>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopShowingPreview();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            RotatePreviewObjectCounterClockwise();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            RotatePreviewObjectClockwise();
        }
    }


    public void RotatePreviewObjectCounterClockwise()
    {
        if (previewObject != null)
        {
            previewObject.transform.Rotate(0, -90, 0);
            soundFeedback.PlaySound(SoundType.Rotate);
        }
    }


    public void RotatePreviewObjectClockwise()
    {
        if (previewObject != null)
        {
            previewObject.transform.Rotate(0, 90, 0);
            soundFeedback.PlaySound(SoundType.Rotate);
        }
    }


    public Quaternion PreviewRotation
    {
        get
        {
            if (previewObject != null)
                return previewObject.transform.rotation;

            return Quaternion.identity;
        }
    }


    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        StopShowingPreview();
        previewObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cursorIndicator.SetActive(true);
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 && size.y > 0)
        {
            cursorIndicator.transform.localScale = new Vector3(size.x, 1f, size.y); //something in this function is messing up the preview grid
            cursorIndicatorRenderer.material.SetVector("_DefaultScale", new Vector2(1, size.y));
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cursorIndicator.SetActive(false);

        if(previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }

    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cursorIndicatorRenderer.material.color = c;
    }

    private void ApplyFeedback(Color color)
    {
        color.a = 0.5f;
        cursorIndicatorRenderer.material.color = color;
        previewMaterialInstance.color = color;
    }

    private void MoveCursor(Vector3 position)
    {
        cursorIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        if (previewObject == null)
            return;
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cursorIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
