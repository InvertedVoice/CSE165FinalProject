using System;
using UnityEngine;

public class BunnyController : MonoBehaviour
{
    private Animator animator;

    public enum Mood { Neutral, Happy, Sad, Angry }
    public Mood currentMood = Mood.Neutral;

    public enum FoodType { None, Carrot, Kale }
    public FoodType wantedFood = FoodType.Carrot;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetMood(Mood.Neutral);
        RequestFood();
    }

    public void SetMood(Mood mood)
    {
        currentMood = mood;
        animator.SetInteger("mood", (int)mood);
    }

    public void RequestFood()
    {
        // Randomly pick what food the bunny wants
        wantedFood = UnityEngine.Random.value > 0.5f ? FoodType.Carrot : FoodType.Kale;
        animator.SetBool("wantsFood", true);
    }

    public void FeedBunny(FoodType givenFood)
    {
        animator.SetBool("wantsFood", false);

        if (givenFood == wantedFood)
        {
            // Correct food
            animator.SetTrigger("PlayHappy");
            SetMood(Mood.Happy);
        }
        else
        {
            // Wrong food
            animator.SetTrigger("PlayAngry");
            SetMood(Mood.Sad);
        }

        // After a delay, request food again
        Invoke("RequestFood", 3f);
    }
}