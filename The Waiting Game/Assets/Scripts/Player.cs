using Unity.AI;
using UnityEngine;
using UnityEngine.AI;
using static DesktopActivityManager;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class ActivityData
    {
        public ActivityType activity;
        public Transform targetLocation;
        public string animationName;
    }

    public DesktopActivityManager activityManager;

    public NavMeshAgent agent;
    public Animator animator;

    public ActivityData[] activities;

    private ActivityType currentActivity;


    void Start()
    {
        if(activityManager == null)
        FindAnyObjectByType<DesktopActivityManager>();
    }

    void Update()
    {
        // Only react when the enum changes
        if (currentActivity != activityManager.CurrentActivity)
        {
            currentActivity = activityManager.CurrentActivity;
            Debug.Log("Applying activity: " + currentActivity);
            ApplyActivity(currentActivity);
        }
    }

    void ApplyActivity(ActivityType activity)
    {
        foreach (ActivityData data in activities)
        {
            if (data.activity == activity)
            {
                Debug.Log("Player activity changed to: " + activity);
                // Move character
                if (data.targetLocation != null)
                {
                    agent.SetDestination(data.targetLocation.position);
                }

                // Play animation
                animator.CrossFade(data.animationName, 0.2f);

                break;
            }
        }
    }
}

