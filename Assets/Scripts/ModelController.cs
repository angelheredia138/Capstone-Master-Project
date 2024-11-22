using UnityEngine;

public class ModelController : MonoBehaviour
{
    public GameObject model; // This will be set at runtime
    private GameObject defaultChild;
    private bool isEditable = true;

    private Lean.Touch.LeanPinchScale pinchScale;
    private Lean.Touch.LeanTwistRotate twistRotate;
    private Lean.Touch.LeanDragTranslate dragTranslate;

    // Initialization happens when SetModel() is called
    void Start()
    {
        // No changes needed here
    }

    // Toggles edit mode and visibility
    public void OnToggleEditButtonClicked()
    {
        if (model == null)
        {
            Debug.LogWarning("ModelController: No model assigned for edit mode toggle.");
            return;
        }

        isEditable = !isEditable;
        SetInteractionComponents(isEditable);

        // Set the model visibility: make it invisible when not editable
        model.SetActive(isEditable);
    }

    // Resets the model's position, scale, and rotation
    public void OnResetModelButtonClicked()
    {
        if (model != null)
        {
            // Reset scale to make the model small
            model.transform.localScale = Vector3.one * 0.1f; // Adjust this value as needed

            // Place it in front of the user
            Transform cameraTransform = Camera.main.transform;
            model.transform.position = cameraTransform.position + cameraTransform.forward * 1.5f;

            // Reset rotation
            model.transform.rotation = Quaternion.identity;

            // Make it face the user
            model.transform.LookAt(cameraTransform);

            // Enable interaction components
            isEditable = true;
            SetInteractionComponents(true);

            // Ensure the model is active
            model.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ModelController: No model assigned to reset.");
        }
    }

    void SetInteractionComponents(bool enable)
    {
        if (pinchScale != null) pinchScale.enabled = enable;
        if (twistRotate != null) twistRotate.enabled = enable;
        if (dragTranslate != null) dragTranslate.enabled = enable;
    }

    // Call this method when your model is loaded
    public void SetModel(GameObject newModel)
    {
        model = newModel;

        if (model != null)
        {
            model.SetActive(true);
            InitializeModel();
            Debug.Log("ModelController: Model assigned successfully. Model name: " + model.name);
        }
        else
        {
            Debug.LogWarning("ModelController: Failed to assign the model, model is null.");
        }
    }

    private void InitializeModel()
    {
        // Initialize the model's child and interaction components
        defaultChild = model.transform.Find("default")?.gameObject;

        if (defaultChild == null)
        {
            Debug.LogWarning("ModelController: Child object 'default' not found under the model.");
        }

        // Attempt to get existing Lean Touch components
        pinchScale = model.GetComponent<Lean.Touch.LeanPinchScale>();
        twistRotate = model.GetComponent<Lean.Touch.LeanTwistRotate>();
        dragTranslate = model.GetComponent<Lean.Touch.LeanDragTranslate>();

        // If the interaction components are not attached, add them
        if (pinchScale == null)
        {
            pinchScale = model.AddComponent<Lean.Touch.LeanPinchScale>();
        }
        if (twistRotate == null)
        {
            twistRotate = model.AddComponent<Lean.Touch.LeanTwistRotate>();
        }
        if (dragTranslate == null)
        {
            dragTranslate = model.AddComponent<Lean.Touch.LeanDragTranslate>();
        }

        SetInteractionComponents(isEditable);
    }
}
