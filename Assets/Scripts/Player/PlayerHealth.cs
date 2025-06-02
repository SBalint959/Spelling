using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float Health;
    public Scrollbar HealthScroll;
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private GameObject HpBarCanvas;
    [SerializeField] private GameObject PointsCanvas;

    public void TakeDamage(int damage)
    {
        Health -= damage;

        HealthScroll.size = Health / 100;

        if (Health <= 0)
        {

            EndGame();


            //             // If running in the editor
            // #if UNITY_EDITOR
            //             UnityEditor.EditorApplication.isPlaying = false;
            // #else
            //                                     // If running in a build
            //                                     Application.Quit();
            // #endif
            //         }

        }
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
        GameOverCanvas.SetActive(true);
        HpBarCanvas.SetActive(false);
        PointsCanvas.SetActive(false);

    }
}
