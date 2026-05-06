using UnityEngine;

public class Player : MonoBehaviour
{
    public DesktopActivityManager activityManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(activityManager == null)
        FindAnyObjectByType<DesktopActivityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
