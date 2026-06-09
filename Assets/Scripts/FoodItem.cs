using UnityEngine;


public class FoodItem : MonoBehaviour
{
    public BunnyController.FoodType foodType;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    public BunnyController.FoodType GetFoodType()
    {
        return foodType;
    }
}