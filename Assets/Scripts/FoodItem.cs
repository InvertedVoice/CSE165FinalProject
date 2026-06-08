using UnityEngine;


public class FoodItem : MonoBehaviour
{
    public BunnyController.FoodType foodType;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Stylized bunny")
        {
            Destroy(gameObject);
        }
    }
    public BunnyController.FoodType GetFoodType()
    {
        return foodType;
    }
}