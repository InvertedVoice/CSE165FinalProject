using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class QuestControllerInput : MonoBehaviour
{
    [Header("References")]
    public Transform head; // Main Camera
    public CharacterController characterController;

    [Header("Spawn Prefabs")]
    public GameObject carrotPrefab;
    public GameObject cabbagePrefab;
    public float spawnDistance = 1.5f;
    public float spawnHeight = 1.5f;

    [Header("Movement")]
    public InputActionProperty moveAction; // Left stick
    public float moveSpeed = 2.5f;
    public float gravity = -9.81f;

    [Header("Buttons")]
    public InputActionProperty leftTrigger;
    public InputActionProperty rightTrigger;
    public InputActionProperty leftGrip;
    public InputActionProperty rightGrip;
    public InputActionProperty buttonA;
    public InputActionProperty buttonB;
    public InputActionProperty buttonX;
    public InputActionProperty buttonY;

    private float verticalVelocity;
    private XRInteractionManager interactionManager;

    public XRDirectInteractor leftHandInteractor;
    public XRDirectInteractor rightHandInteractor;

    void Reset()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        interactionManager = FindObjectOfType<XRInteractionManager>(); 
    }

    void OnEnable()
    {
        EnableAll();
    }

    void OnDisable()
    {
        DisableAll();
    }

    void Update()
    {
        MoveWithLeftStick();
        DetectButtons();
    }

    void MoveWithLeftStick()
    {
        if (moveAction.action == null || head == null || characterController == null)
            return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

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
        if (buttonA.action.WasPressedThisFrame())
        {   
            Debug.Log("A pressed: spawning carrot");
            SpawnFood(carrotPrefab);
        }

        if (buttonB.action.WasPressedThisFrame())
        {
            Debug.Log("B pressed: spawning cabbage");
            SpawnFood(cabbagePrefab);
        }

        if (buttonX.action.WasPressedThisFrame())
            Debug.Log("X pressed");

        if (buttonY.action.WasPressedThisFrame())
            Debug.Log("Y pressed");

        if (leftTrigger.action.WasPressedThisFrame())
            Debug.Log("Left trigger pressed");

        if (rightTrigger.action.WasPressedThisFrame())
            Debug.Log("Right trigger pressed");

        if (leftGrip.action.WasPressedThisFrame())
            Debug.Log("Left grip pressed");

        if (rightGrip.action.WasPressedThisFrame())
            Debug.Log("Right grip pressed");
    }

    void SpawnFood(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null || head == null)
            return;
        
        Debug.Log("prereqs checked");
        Vector3 spawnPos = head.position + head.forward * spawnDistance;
        spawnPos.y = spawnHeight;

        GameObject spawnedFood = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        if (spawnedFood.GetComponent<Rigidbody>() == null)
            spawnedFood.AddComponent<Rigidbody>();

        XRGrabInteractable grab = spawnedFood.GetComponent<XRGrabInteractable>();

        if (grab != null && interactionManager != null)
            grab.interactionManager = interactionManager;
    }

    void EnableAll()
    {
        moveAction.action.Enable();
        leftTrigger.action.Enable();
        rightTrigger.action.Enable();
        leftGrip.action.Enable();
        rightGrip.action.Enable();
        buttonA.action.Enable();
        buttonB.action.Enable();
        buttonX.action.Enable();
        buttonY.action.Enable();
    }

    void DisableAll()
    {
        moveAction.action.Disable();
        leftTrigger.action.Disable();
        rightTrigger.action.Disable();
        leftGrip.action.Disable();
        rightGrip.action.Disable();
        buttonA.action.Disable();
        buttonB.action.Disable();
        buttonX.action.Disable();
        buttonY.action.Disable();
    }
}