using UnityEngine;
using UnityEngine.Device;

public class ScaleScreenSize : MonoBehaviour
{
    public int screenWidth;
    public int screenHeight;

    public void changeScreenSize()
    {
        UnityEngine.Screen.SetResolution(screenWidth, screenHeight, false);
    }
}
