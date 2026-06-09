using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Debug = UnityEngine.Debug;

public class QuestControllerInput : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public CharacterController characterController;

    [Header("Spawn Prefabs")]
    public GameObject carrotPrefab;
    public GameObject cabbagePrefab;
    public float spawnDistance = 1.5f;
    public float spawnHeight = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float gravity = -9.81f;

    private float verticalVelocity;
    private XRInteractionManager interactionManager;

    // Direct input actions created in code
    private InputAction moveAction;
    private InputAction buttonAAction;
    private InputAction buttonBAction;

    void Start()
    {
        interactionManager = FindObjectOfType<XRInteractionManager>();

        // Create input actions directly bound to Quest controller bindings
        moveAction = new InputAction("Move", binding: "<XRController>{LeftHand}/thumbstick");
        moveAction.Enable();

        buttonAAction = new InputAction("ButtonA", binding: "<XRController>{RightHand}/primaryButton");
        buttonAAction.Enable();

        buttonBAction = new InputAction("ButtonB", binding: "<XRController>{RightHand}/secondaryButton");
        buttonBAction.Enable();
    }

    void OnDestroy()
    {
        moveAction?.Disable();
        buttonAAction?.Disable();
        buttonBAction?.Disable();
    }

    void Update()
    {
        MoveWithLeftStick();
        DetectButtons();

        Debug.Log("A: " + buttonAAction.ReadValue<float>() + " B: " + buttonBAction.ReadValue<float>());
    }

    void MoveWithLeftStick()
    {
        if (head == null || characterController == null) return;

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 forward = head.forward;
        Vector3 right = head.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * input.y + right * input.x;

        if (characterController.isGrounded && verticalVelocity < 0)
            verticalVelocity = -1f;

        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    void DetectButtons()
    {
        if (buttonAAction.WasPressedThisFrame())
        {
            Debug.Log("A pressed: spawning carrot");
            SpawnFood(carrotPrefab);
        }

        if (buttonBAction.WasPressedThisFrame())
        {
            Debug.Log("B pressed: spawning cabbage");
            SpawnFood(cabbagePrefab);
        }
    }

    void SpawnFood(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null || head == null) return;

        Vector3 spawnPos = head.position + head.forward * spawnDistance;
        spawnPos.y = spawnHeight;

        GameObject spawnedFood = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (spawnedFood.GetComponent<Rigidbody>() == null)
            spawnedFood.AddComponent<Rigidbody>();

        XRGrabInteractable grab = spawnedFood.GetComponent<XRGrabInteractable>();
        if (grab != null && interactionManager != null)
            grab.interactionManager = interactionManager;
    }
}