using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float Health;
    public Scrollbar HealthScroll;
    public GameManager gameManager;
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

        }
    }

    private void EndGame()
    {
        Time.timeScale = 0f;
        gameManager.DisableTime();
        GameOverCanvas.SetActive(true);
        HpBarCanvas.SetActive(false);
        PointsCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
