using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class HandGestureSpawner : MonoBehaviour
{
    public GameObject carrotPrefab;
    public GameObject cabbagePrefab;

    public float pinchThreshold = 0.02f;

    private XRHandSubsystem handSubsystem;

    private bool leftPinching = false;
    private bool rightPinching = false;

    void Start()
    {
        StartCoroutine(InitializeHandSubsystem());
    }

    IEnumerator InitializeHandSubsystem()
    {
        yield return new WaitUntil(() =>
            XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null);

        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader
            .GetLoadedSubsystem<XRHandSubsystem>();
    }

    void Update()
    {
        if (handSubsystem == null) return;

        DetectPinch(handSubsystem.leftHand, ref leftPinching, carrotPrefab);
        DetectPinch(handSubsystem.rightHand, ref rightPinching, cabbagePrefab);
    }

    void DetectPinch(XRHand hand, ref bool wasPinching, GameObject prefabToSpawn)
    {
        if (!hand.isTracked) return;

        XRHandJoint thumbTip = hand.GetJoint(XRHandJointID.ThumbTip);
        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);

        if (thumbTip.TryGetPose(out Pose thumbPose) && indexTip.TryGetPose(out Pose indexPose))
        {
            float distance = Vector3.Distance(thumbPose.position, indexPose.position);
            bool isPinching = distance < pinchThreshold;

            if (isPinching && !wasPinching)
            {
                // Spawn food at hand position
                Vector3 spawnPos = (thumbPose.position + indexPose.position) / 2f;
                Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            }

            wasPinching = isPinching;
        }
    }
}