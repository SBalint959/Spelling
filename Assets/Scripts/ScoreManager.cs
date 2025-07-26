using UnityEngine;
using TMPro;
using System; // <- Required for TextMeshProUGUI

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int currentScore = 0;
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
    }

    public string GetFinalScore()
    {
        return currentScore.ToString();
    }
}
