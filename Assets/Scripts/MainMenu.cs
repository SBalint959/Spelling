using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Update()
    {
        Cursor.visible = true;
    }
    public void StartGame()
    {
        SceneManager.LoadScene("IslandScene");
    }
}

