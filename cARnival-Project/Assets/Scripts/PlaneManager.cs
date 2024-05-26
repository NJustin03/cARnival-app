using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(requiredComponent: typeof(ARRaycastManager), requiredComponent2: typeof(ARPlaneManager))]
public class PlaneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private bool objectPlaced = false;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDestroy()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        // Check if an object is already placed
        if (objectPlaced)
            return;

        // Perform raycast
        if (aRRaycastManager.Raycast(screenPoint: finger.currentTouch.screenPosition,
            hitResults: hits, trackableTypes: TrackableType.PlaneWithinPolygon))
        {
            // Place object at first hit position
            ARRaycastHit hit = hits[0];
            Pose pose = hit.pose;
            Instantiate(prefab, pose.position, pose.rotation);

            // Disable further plane detection
            aRPlaneManager.enabled = false;

            // Set objectPlaced to true to prevent further object placement
            objectPlaced = true;
        }
    }
}
