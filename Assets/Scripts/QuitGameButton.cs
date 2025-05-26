using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    // This method can be assigned to a button's OnClick() event
    public void QuitGame()
    {
        Debug.Log("Quit Game");

        // If running in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build
        Application.Quit();
#endif
    }
}

