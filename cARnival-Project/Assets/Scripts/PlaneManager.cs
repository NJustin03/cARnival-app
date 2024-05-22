using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneManager : MonoBehaviour
{
    public GameObject spawnableObject;
    private ARPlaneManager arPlaneManager;
    private bool planeDetected = false;

    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }
    private void OnEnable()
    {
        arPlaneManager.planesChanged += OnPlaneChanged;
    }
    private void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlaneChanged;
    }

    void OnPlaneChanged(ARPlanesChangedEventArgs args)
    {
        if (!planeDetected && args.added.Count > 0)
        {
            //Processes the first plane & sets the position for objects to spawn
            ARPlane firstPlane = args.added[0];
            Vector3 spawnPosition = firstPlane.transform.position;
            Instantiate(spawnableObject, spawnPosition, Quaternion.identity);

            planeDetected = true;

            arPlaneManager.enabled = false;

            //Ensures that any planes that are possibly added to the scene are now inactive
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }

            firstPlane.gameObject.SetActive(true);
        }
    }
}
