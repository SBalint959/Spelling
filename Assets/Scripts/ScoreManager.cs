using UnityEngine;
using TMPro;
using System; // <- Required for TextMeshProUGUI

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public PowerUpSpawner PowerUpManager;

    public GameObject InfoUI;

    private int currentScore = 0;

    private float powerUpCounter = 0.5f;
    private TextMeshProUGUI pointsText; // <- TextMeshPro instead of UnityEngine.UI.Text

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Find the TextMeshProUGUI object
        GameObject pointsObj = GameObject.FindGameObjectWithTag("Points");
        if (pointsObj != null)
        {
            pointsText = pointsObj.GetComponent<TextMeshProUGUI>();
        }
        else
        {
            Debug.LogError("No TextMeshProUGUI object with tag 'Points' found in scene.");
        }
    }

    public void AddPoints(int amount)
    {
        currentScore += amount;

        if (pointsText != null)
            pointsText.text = currentScore.ToString();

        if (currentScore >= powerUpCounter * 100 && powerUpCounter < 8.0f)
        {
            PowerUpManager.SpawnPowerUp();
            InfoUI.SetActive(true);

            if (powerUpCounter < 1.5f)
            {
                powerUpCounter += 0.5f;
            }
            else if (powerUpCounter > 2.0f)
            {
                powerUpCounter += 2.0f;
            }
            else
            {
                powerUpCounter += 1.5f;
            }
        }
    }

    public string GetFinalScore()
    {
        return currentScore.ToString();
    }
}
