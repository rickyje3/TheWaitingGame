using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Forces the sprite to look directly at the main camera
        transform.LookAt(Camera.main.transform);
        // Rotates it 180 degrees so it faces the correct way
        transform.Rotate(0, 180, 0);
    }
}
