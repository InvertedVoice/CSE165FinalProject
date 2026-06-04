using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandGestureSpawner : MonoBehaviour
{
    public GameObject carrotPrefab;
    public GameObject cabbagePrefab;
    public float fistThreshold = 0.05f;
    public Transform playerCamera;
    public float spawnDistance = 1.5f;
    public float spawnHeight = 1.5f;

    private XRHandSubsystem handSubsystem;
    private bool leftFisting = false;
    private bool rightFisting = false;
    private XRInteractionManager interactionManager;

    void Start()
    {
        interactionManager = FindObjectOfType<XRInteractionManager>();
        StartCoroutine(InitializeHandSubsystem());
    }

    IEnumerator InitializeHandSubsystem()
    {
        yield return new WaitUntil(() =>
            XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null);

        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader
            .GetLoadedSubsystem<XRHandSubsystem>();

        if (handSubsystem != null)
        {
            handSubsystem.updatedHands += OnUpdatedHands;
        }
    }

    void OnDestroy()
    {
        if (handSubsystem != null)
        {
            handSubsystem.updatedHands -= OnUpdatedHands;
        }
    }

    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags flags,
        XRHandSubsystem.UpdateType updateType)
    {
        if (updateType == XRHandSubsystem.UpdateType.BeforeRender) return;

        DetectFist(subsystem.leftHand, ref leftFisting, carrotPrefab);
        DetectFist(subsystem.rightHand, ref rightFisting, cabbagePrefab);
    }

    void DetectFist(XRHand hand, ref bool wasFisting, GameObject prefabToSpawn)
    {
        if (!hand.isTracked) return;

        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        XRHandJoint indexMeta = hand.GetJoint(XRHandJointID.IndexMetacarpal);
        XRHandJoint middleTip = hand.GetJoint(XRHandJointID.MiddleTip);
        XRHandJoint middleMeta = hand.GetJoint(XRHandJointID.MiddleMetacarpal);

        if (!indexTip.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return;
        if (!indexMeta.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return;
        if (!middleTip.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return;
        if (!middleMeta.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return;

        if (indexTip.TryGetPose(out Pose indexTipPose) &&
            indexMeta.TryGetPose(out Pose indexMetaPose) &&
            middleTip.TryGetPose(out Pose middleTipPose) &&
            middleMeta.TryGetPose(out Pose middleMetaPose))
        {
            float indexCurl = Vector3.Distance(indexTipPose.position, indexMetaPose.position);
            float middleCurl = Vector3.Distance(middleTipPose.position, middleMetaPose.position);

            bool isFisting = indexCurl < fistThreshold && middleCurl < fistThreshold;

            if (isFisting && !wasFisting)
            {
                Vector3 spawnPos = playerCamera.position
                    + playerCamera.forward * spawnDistance;
                spawnPos.y = spawnHeight;

                GameObject spawnedFood = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

                if (spawnedFood.GetComponent<Rigidbody>() == null)
                    spawnedFood.AddComponent<Rigidbody>();

                XRGrabInteractable grab = spawnedFood.GetComponent<XRGrabInteractable>();
                if (grab != null)
                    grab.interactionManager = interactionManager;
            }

            wasFisting = isFisting;
        }
    }
}