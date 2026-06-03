using UnityEngine;

public class FeedingZone : MonoBehaviour
{
    public BunnyController bunnyController;

    void OnTriggerEnter(Collider other)
    {
        FoodItem food = other.GetComponent<FoodItem>();

        if (food != null)
        {
            bunnyController.FeedBunny(food.GetFoodType());
            Destroy(other.gameObject);
        }
    }
}