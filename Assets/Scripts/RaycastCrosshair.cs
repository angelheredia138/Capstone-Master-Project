using UnityEngine;
using System.Collections.Generic;

public class RaycastCrosshair : MonoBehaviour
{
    public float rayDistance = 500f;
    private Camera arCamera;

    // Dictionary to hold colliders and their respective canvases
    private Dictionary<string, Canvas> colliderCanvasMap = new Dictionary<string, Canvas>();

    void Start()
    {
        Debug.Log("Start raycast");
        arCamera = Camera.main;

        // Disable all canvases initially
        DisableAllCanvases();
    }

    void Update()
    {
        Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            Debug.Log($"Raycast hit: {hit.collider.name}");

            // Check if there is an associated canvas for the hit collider
            if (colliderCanvasMap.TryGetValue(hit.collider.name, out Canvas hitCanvas))
            {
                EnableCanvas(hitCanvas);
                Debug.Log($"Enabled canvas: {hitCanvas.name}");
            }
            else
            {
                DisableAllCanvases();
                Debug.Log("No associated canvas found, disabling all canvases.");
            }
        }
        else
        {
            DisableAllCanvases();
            Debug.Log("No raycast hit detected, disabling all canvases.");
        }
    }

    // Set colliders and their respective canvases
    // Builds the mapping between colliders and canvases
    public void BuildColliderCanvasMap(GameObject instantiatedModel)
    {
        // Clear existing mappings
        colliderCanvasMap.Clear();

        // Find all colliders and canvases within the instantiated model
        Collider[] colliders = instantiatedModel.GetComponentsInChildren<Collider>();
        Canvas[] canvases = instantiatedModel.GetComponentsInChildren<Canvas>();

        foreach (Collider collider in colliders)
        {
            // Look for a canvas that might be associated with the collider
            // Assuming the Canvas is either a sibling or a child of the collider
            Canvas correspondingCanvas = collider.GetComponentInChildren<Canvas>();
            if (correspondingCanvas == null)
            {
                correspondingCanvas = collider.transform.parent?.GetComponentInChildren<Canvas>();
            }

            if (correspondingCanvas != null)
            {
                colliderCanvasMap[collider.name] = correspondingCanvas;
                Debug.Log($"Mapped collider '{collider.name}' to canvas '{correspondingCanvas.name}'");
            }
            else
            {
                Debug.LogWarning($"No canvas found for collider '{collider.name}'");
            }
        }
    }

    // Enable a specific canvas
    private void EnableCanvas(Canvas targetCanvas)
    {
        if (targetCanvas != null)
        {
            targetCanvas.enabled = true;
            Vector3 direction = arCamera.transform.position - targetCanvas.transform.position;
            direction.y = 0; 
            targetCanvas.transform.rotation = Quaternion.LookRotation(-direction);
        }
    }

    // Disable all canvases managed by the script
    private void DisableAllCanvases()
    {
        foreach (Canvas canvas in colliderCanvasMap.Values)
        {
            if (canvas != null)
            {
                canvas.enabled = false;
            }
        }
    }
}
