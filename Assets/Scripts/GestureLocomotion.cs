using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class GestureLocomotion : MonoBehaviour
{
    public Transform xrOrigin;
    public Transform playerCamera;
    public float moveSpeed = 1.5f;

    private XRHandSubsystem handSubsystem;
    private bool isMoving = false;

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

        if (handSubsystem != null)
            handSubsystem.updatedHands += OnUpdatedHands;
    }

    void OnDestroy()
    {
        if (handSubsystem != null)
            handSubsystem.updatedHands -= OnUpdatedHands;
    }

    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags flags,
        XRHandSubsystem.UpdateType updateType)
    {
        if (updateType == XRHandSubsystem.UpdateType.BeforeRender) return;

        bool leftPointing = IsIndexPointing(subsystem.leftHand);
        bool rightPointing = IsIndexPointing(subsystem.rightHand);

        isMoving = leftPointing && rightPointing;
    }

    bool IsIndexPointing(XRHand hand)
    {
        if (!hand.isTracked) return false;

        XRHandJoint indexTip = hand.GetJoint(XRHandJointID.IndexTip);
        XRHandJoint indexProximal = hand.GetJoint(XRHandJointID.IndexProximal);
        XRHandJoint middleTip = hand.GetJoint(XRHandJointID.MiddleTip);
        XRHandJoint middleProximal = hand.GetJoint(XRHandJointID.MiddleProximal);

        if (!indexTip.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return false;
        if (!indexProximal.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return false;
        if (!middleTip.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return false;
        if (!middleProximal.trackingState.HasFlag(XRHandJointTrackingState.Pose)) return false;

        if (indexTip.TryGetPose(out Pose indexTipPose) &&
            indexProximal.TryGetPose(out Pose indexProximalPose) &&
            middleTip.TryGetPose(out Pose middleTipPose) &&
            middleProximal.TryGetPose(out Pose middleProximalPose))
        {
            float indexExtended = Vector3.Distance(indexTipPose.position, indexProximalPose.position);
            float middleCurled = Vector3.Distance(middleTipPose.position, middleProximalPose.position);

            // Index extended, middle curled = pointing gesture
            return indexExtended > 0.08f && middleCurled < 0.07f;
        }

        return false;
    }

    void Update()
    {
        if (xrOrigin != null && playerCamera != null)
        {
            Vector3 forward = playerCamera.forward;
            forward.y = 0;
            forward.Normalize();

            CharacterController cc = xrOrigin.GetComponent<CharacterController>();
            if (cc != null)
            {
                // Apply gravity
                Vector3 move = Vector3.zero;
                if (isMoving)
                    move += forward * moveSpeed;

                move.y -= 9.81f * Time.deltaTime;
                cc.Move(move * Time.deltaTime);
            }
            else
            {
                if (isMoving)
                    xrOrigin.position += forward * moveSpeed * Time.deltaTime;
            }
        }
    }
}