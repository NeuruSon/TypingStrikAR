using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]

public class PlaceOnPlane : MonoBehaviour
{
    public ARRaycastManager arRaycaster;
    public ARSessionOrigin arSessionOrigin;
    public GameObject startNative, gCon;

    public Transform targetObject;

    public List<ARRaycastHit> hits = new List<ARRaycastHit>();

    ARPlaneManager m_ARPlaneManager;

    void Awake()
    {
        m_ARPlaneManager = GetComponent<ARPlaneManager>();
    }

    bool isDetectingSpawn = false; //나왔냐? 

    void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
        {
            return;
        }

        if (Input.touchCount < 2 && !isDetectingSpawn)
        {

            startGameNative(touch);
        }

        if (!gCon.GetComponent<GameController>().isSpawned)
        {
            startGameNative(touch);
        }
    }

    public void TogglePlaneDetection()
    {
        m_ARPlaneManager.enabled = !m_ARPlaneManager.enabled;

        if (m_ARPlaneManager.enabled)
        {
            SetAllPlanesActive(true);
        }
        else { SetAllPlanesActive(false); }
    }

    public void SetAllPlanesActive(bool value)
    {
        foreach (var plane in m_ARPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }

    public void ChangeRespawnStatus()
    {
        isDetectingSpawn = false;
        arSessionOrigin.MakeContentAppearAt(targetObject, Vector3.zero, Quaternion.Euler(0, 0, 0));
        GameObject.FindGameObjectWithTag("SpawnObject").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(0).gameObject.SetActive(false);
    }


    public void startGameNative(Touch touch)
    {
        if (arRaycaster.Raycast(touch.position, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;

            TogglePlaneDetection();

            startNative.SetActive(true);
            arSessionOrigin.MakeContentAppearAt(targetObject, hitPose.position, hitPose.rotation);

            isDetectingSpawn = true; gCon.GetComponent<GameController>().isSpawned = true;
        }
    }
}
