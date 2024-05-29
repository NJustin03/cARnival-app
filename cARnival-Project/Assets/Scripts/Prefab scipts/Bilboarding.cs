using UnityEngine;

public class Billboarding : MonoBehaviour
{
    public Camera arCamera;  // Reference to the AR Camera

    void LateUpdate()
    {
        if (arCamera != null)
        {
            // Make the Canvas face the camera
            transform.LookAt(transform.position + arCamera.transform.rotation * Vector3.forward,
                             arCamera.transform.rotation * Vector3.up);
        }
    }
}
