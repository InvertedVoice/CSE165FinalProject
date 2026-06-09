using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class BunnyWalker : MonoBehaviour
{
    public Transform controllerTransform;
    public float rayDistance = 20f;

    private NavMeshAgent agent;
    private InputAction leftTriggerAction;
    private LineRenderer lineRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        leftTriggerAction = new InputAction(
            "LeftTrigger",
            binding: "<XRController>{LeftHand}/trigger"
        );
        leftTriggerAction.Enable();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.enabled = false;
    }

    void OnDestroy()
    {
        leftTriggerAction?.Disable();
    }

    void Update()
    {
        float triggerValue = leftTriggerAction.ReadValue<float>();
        bool isTriggerHeld = triggerValue > 0.5f;

        if (isTriggerHeld && controllerTransform != null)
        {
            Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
            RaycastHit hit;

            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, controllerTransform.position);

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                NavMeshHit navHit;
                if (NavMesh.SamplePosition(hit.point, out navHit, 1.0f, NavMesh.AllAreas))
                {
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                    lineRenderer.SetPosition(1, hit.point);

                    if (leftTriggerAction.WasReleasedThisFrame())
                    {
                        agent.SetDestination(navHit.position);
                    }
                }
                else
                {
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                    lineRenderer.SetPosition(1, hit.point);
                }
            }
            else
            {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                lineRenderer.SetPosition(1, controllerTransform.position + controllerTransform.forward * rayDistance);
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}