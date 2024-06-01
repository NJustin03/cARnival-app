using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private Material transparentPlane;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool objectPlaced = false;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();

        // Ensure the prefab is set
        if (prefab == null)
        {
            Debug.LogError("Prefab is not set. Please assign a prefab in the inspector.");
        }

        prefab.SetActive(false);

    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        // Check if an object is already placed or if the prefab is not set
        if (objectPlaced || prefab == null)
            return;

        // Perform raycast
        if (aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // Place object at first hit position
            ARRaycastHit hit = hits[0];
            Pose pose = hit.pose;

            //Ensures that the game will be in the position of which the player has set the raycast
            Vector3 posePosition = new(pose.position.x, (float)(pose.position.x + 0.1), pose.position.z);
            prefab.transform.position = posePosition;

            //Reveal the game
            prefab.SetActive(true);

            // Make all existing planes transparent
            MakeAllPlanesTransparent();

            // Disable further plane detection
            aRPlaneManager.enabled = false;

            // Set objectPlaced to true to prevent further object placement
            objectPlaced = true;

            Debug.Log("Object placed and planes made transparent.");
        }
    }

    private void MakeAllPlanesTransparent()
    {
        foreach (var plane in aRPlaneManager.trackables)
        {
            Renderer renderer = plane.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = transparentPlane;
            }
        }
    }
}
