using UnityEngine;

public class LockRotationY : MonoBehaviour
{
    void LateUpdate()
    {
        // Get the current rotation
        Vector3 rotation = transform.eulerAngles;
        
        // Lock the X and Z rotation to 0 to keep the model upright
        rotation.x = 0;
        rotation.z = 0;

        // Apply the adjusted rotation back to the transform
        transform.eulerAngles = rotation;
    }
}
