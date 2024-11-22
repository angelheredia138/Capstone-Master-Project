using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModelControllerManager : MonoBehaviour
{
    public static ModelControllerManager Instance { get; private set; }

    private ModelController modelController;

    public Button toggleEditButton; // Assign in the Unity Editor
    public Button resetModelButton; // Assign in the Unity Editor

    void Awake()
    {
        // Ensure there's only one instance of this manager
        if (Instance == null)
        {
            Instance = this;
            // Optionally, if you want this GameObject to persist across scenes:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Assign button listeners
        if (toggleEditButton != null)
        {
            toggleEditButton.onClick.AddListener(OnToggleEditButtonClicked);
        }
        else
        {
            Debug.LogWarning("ModelControllerManager: Toggle Edit Button not assigned.");
        }

        if (resetModelButton != null)
        {
            resetModelButton.onClick.AddListener(OnResetModelButtonClicked);
        }
        else
        {
            Debug.LogWarning("ModelControllerManager: Reset Model Button not assigned.");
        }
    }

    // Method to set the ModelController instance when it's created
    public void SetModelController(ModelController controller)
    {
        modelController = controller;
    }

    // Methods called by the buttons
    public void OnToggleEditButtonClicked()
    {
        if (modelController != null)
        {
            modelController.OnToggleEditButtonClicked();
        }
        else
        {
            Debug.LogWarning("ModelControllerManager: ModelController is null.");
        }
    }

    public void OnResetModelButtonClicked()
    {
        if (modelController != null)
        {
            modelController.OnResetModelButtonClicked();
        }
        else
        {
            Debug.LogWarning("ModelControllerManager: ModelController is null.");
        }
    }
}
