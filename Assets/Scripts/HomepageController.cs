using UnityEngine;
using TMPro; // Use TextMeshPro namespace for TMP components
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HomepageController : MonoBehaviour
{
    public Button newModelButton;          // Button to bring up the input panel
    public GameObject inputPanel;          // The panel containing the input field and buttons
    public TMP_InputField linkInputField;  // TMP Input field for the user to enter the direct link
    public Button okButton;                // OK button in the input panel
    public Button cancelButton;            // Cancel button in the input panel

    public GameObject scrollViewContent; 
    public GameObject modelListItemPrefab;
    public static GameObject selectedModelPrefab;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Make sure this GameObject persists across scenes
    }

    void Start()
    {
        // Assign button click listener to bring up the input panel
        newModelButton.onClick.AddListener(OnNewModelButtonClicked);

        // Assign click listeners to OK and Cancel buttons within the input panel
        okButton.onClick.AddListener(OnOkButtonClicked);
        cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    void OnNewModelButtonClicked()
    {
        // Display the input panel when the "New Model" button is clicked
        if (inputPanel != null)
        {
            Debug.Log("HomepageController: Displaying the input panel.");
            
            // Show the entire input panel
            inputPanel.SetActive(true);

            // Make sure the input field is ready for user input
            linkInputField.Select();
            linkInputField.ActivateInputField();
        }
        else
        {
            Debug.LogError("HomepageController: Input Panel is not assigned in the Inspector.");
        }
    }

    public void OnOkButtonClicked()
    {
        // Get the link from the input field and hide the panel
        string url = linkInputField.text;

        if (!string.IsNullOrEmpty(url))
        {
            inputPanel.SetActive(false);  // Hide the input panel
            StartCoroutine(DownloadAndLoadAssetBundle(url));  // Start downloading the AssetBundle
        }
        else
        {
            Debug.LogError("HomepageController: No URL provided.");
        }
    }

    public void OnCancelButtonClicked()
    {
        // Hide the input panel without doing anything
        inputPanel.SetActive(false);
    }

    IEnumerator DownloadAndLoadAssetBundle(string url)
    {
        // Start downloading the asset bundle
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("HomepageController: Failed to download AssetBundle - " + www.error);
            yield break;
        }

        // Save downloaded data as an asset bundle
        byte[] bundleData = www.downloadHandler.data;
        var bundleLoadRequest = AssetBundle.LoadFromMemoryAsync(bundleData);
        yield return bundleLoadRequest;

        AssetBundle bundle = bundleLoadRequest.assetBundle;

        if (bundle == null)
        {
            Debug.LogError("HomepageController: Failed to load AssetBundle from downloaded data");
            yield break;
        }

        // Assuming there is one prefab in the AssetBundle - modify if needed
        string[] assetNames = bundle.GetAllAssetNames();
        if (assetNames.Length > 0)
        {
            string prefabName = assetNames[0];

            // Load the prefab asynchronously
            var loadAssetRequest = bundle.LoadAssetAsync<GameObject>(prefabName);
            yield return loadAssetRequest;

            GameObject modelPrefab = loadAssetRequest.asset as GameObject;

            if (modelPrefab != null)
            {
                // Generate a preview and add to the scroll view
                StartCoroutine(GeneratePreviewAndAddToList(modelPrefab));
            }
        }

        // Unload the bundle but keep the assets loaded
        bundle.Unload(false);
    }

    IEnumerator GeneratePreviewAndAddToList(GameObject modelPrefab)
    {
        //Debug.LogError("NUMBER FOUR");

        // Create an instance of the model for preview purposes
        GameObject previewInstance = Instantiate(modelPrefab);
        previewInstance.transform.position = Vector3.zero;

        // Wait a frame to allow the instance to be created properly
        yield return null;

        // Calculate bounds of the model to determine the optimal camera distance
        Renderer modelRenderer = previewInstance.GetComponentInChildren<Renderer>();
        if (modelRenderer == null)
        {
            Destroy(previewInstance);
            Debug.LogError("HomepageController: No renderer found on the prefab instance, cannot generate preview.");
            yield break;
        }

        Bounds modelBounds = modelRenderer.bounds;
        float modelSize = Mathf.Max(modelBounds.size.x, modelBounds.size.y, modelBounds.size.z);
        float cameraDistance = modelSize * 1.8f;

        // Create and position the preview camera
        Camera previewCamera = new GameObject("PreviewCamera").AddComponent<Camera>();
        RenderTexture renderTexture = new RenderTexture(256, 256, 16);
        previewCamera.targetTexture = renderTexture;

        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);

        previewCamera.transform.position = modelBounds.center + new Vector3(0, modelSize / 2, -cameraDistance);
        previewCamera.transform.LookAt(modelBounds.center);

        // Render the camera to capture the preview
        previewCamera.Render();

        // Create a Texture2D from the RenderTexture
        RenderTexture.active = renderTexture;
        Texture2D previewTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        previewTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        previewTexture.Apply();

        // Clean up preview objects
        Destroy(previewInstance);
        Destroy(previewCamera.gameObject);
        RenderTexture.active = null;

        //Debug.LogError("NUMBER FIVE");

        // Add model to scroll view
        AddModelToList(modelPrefab, previewTexture);
    }

    void AddModelToList(GameObject modelPrefab, Texture2D previewImage)
    {
        // Create a new list item and set it as a child of the scroll view content
        GameObject listItem = Instantiate(modelListItemPrefab, scrollViewContent.transform, false);

        if (listItem == null)
        {
            Debug.LogError("HomepageController: Failed to instantiate model list item prefab");
            return;
        }

        // Set the name in the TMP_Text component
        var text = listItem.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = modelPrefab.name;
        }

        // Set the preview image in the Image component
        var image = listItem.transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
        if (image != null)
        {
            image.sprite = Sprite.Create(previewImage, new Rect(0, 0, previewImage.width, previewImage.height), new Vector2(0.5f, 0.5f));
        }

        // Add a click listener to load the model in AR
        var listItemButton = listItem.GetComponent<Button>();
        if (listItemButton != null)
        {
            listItemButton.onClick.AddListener(() => LoadModelInAR(modelPrefab));
        }

        // Force the layout group to update immediately
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewContent.GetComponent<RectTransform>());
    }



    void LoadModelInAR(GameObject modelPrefab)
    {
        //Debug.LogError("NUMBER SEVEN");

        if (modelPrefab == null)
        {
            Debug.LogError("HomepageController: Model prefab is null. Cannot load AR scene.");
            return;
        }

        // Store the selected model prefab in the static variable
        selectedModelPrefab = modelPrefab;

        // Load the AR scene (replace "ARScene" with your actual AR scene name)
        SceneManager.LoadScene("ARScene");
    }
}
