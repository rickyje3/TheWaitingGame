using System.Collections.Generic;
using Unity.AppUI.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject cameraRig;
    public Camera gameCamera;
    public ScaleSize scaleSize;

    public void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    public void OpenMenu()
    {
        this.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RecenterGame()
    {
        cameraRig.transform.position = Vector3.zero;

        if (gameCamera.orthographicSize != 8f)
            scaleSize.UpdateUIScale(8);

        gameCamera.orthographicSize = 8f;

        CenterGameWindow();
        //ui doesnt scale correctly
    }

    public void CenterGameWindow()
    {
        #if UNITY_STANDALONE

        // 1. Create a list to store the display information
        List<DisplayInfo> displayLayout = new List<DisplayInfo>();

        // 2. Populate the list with actual OS display data
        Screen.GetDisplayLayout(displayLayout);

        // 3. Ensure we have at least one valid display connected
        if (displayLayout.Count > 0)
        {
            // Grab the primary active display info
            DisplayInfo mainDisplay = displayLayout[0];

            // 4. Use DisplayInfo's dimensions for desktop resolution
            int displayWidth = mainDisplay.width;
            int displayHeight = mainDisplay.height;

            // Get your game's current window size
            int gameWidth = Screen.width;
            int gameHeight = Screen.height;

            // 5. Calculate coordinates to center the window relative to this monitor
            int centerX = (displayWidth - gameWidth) / 2;
            int centerY = (displayHeight - gameHeight) / 2;
            Vector2Int targetPosition = new Vector2Int(centerX, centerY);

            // 6. Pass the correct DisplayInfo struct into the method
            Screen.MoveMainWindowTo(mainDisplay, targetPosition);
        }

        #endif
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
