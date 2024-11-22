using UnityEngine;

public class ARSceneController : MonoBehaviour
{
    public RaycastCrosshair raycastCrosshair;

    void Start()
    {
        if (HomepageController.selectedModelPrefab != null)
        {
            Debug.Log("ARSceneController: Model prefab reference found, name: " + HomepageController.selectedModelPrefab.name);
            LoadSelectedModel(HomepageController.selectedModelPrefab);
        }
        else
        {
            Debug.LogError("ARSceneController: No model selected to load in AR scene.");
        }
    }

    void LoadSelectedModel(GameObject modelPrefab)
    {
        // Instantiate the model prefab in the AR scene
        GameObject instantiatedModel = Instantiate(modelPrefab, Vector3.zero, Quaternion.identity);

        // Add the ModelController component to the instantiated model
        ModelController modelControllerInstance = instantiatedModel.AddComponent<ModelController>();

        // Set up the raycast crosshair with relevant colliders and canvases from the instantiated model
        UpdateRaycastCrosshair(instantiatedModel);

        // Set the model in the ModelController for interaction
        if (modelControllerInstance != null)
        {
            modelControllerInstance.SetModel(instantiatedModel);
            Debug.Log("LoadSelectedModel: Model successfully assigned to ModelController.");

            // Inform the ModelControllerManager of the new ModelController instance
            ModelControllerManager.Instance.SetModelController(modelControllerInstance);
        }
        else
        {
            Debug.LogError("LoadSelectedModel: Failed to add ModelController to the instantiated model.");
        }
    }

    void UpdateRaycastCrosshair(GameObject instantiatedModel)
    {
        if (raycastCrosshair != null)
        {
            // Trigger the BuildColliderCanvasMap function to build mappings between colliders and canvases
            raycastCrosshair.BuildColliderCanvasMap(instantiatedModel);
        }
        else
        {
            Debug.LogError("UpdateRaycastCrosshair: RaycastCrosshair is null. Cannot set canvases.");
        }
    }
}
