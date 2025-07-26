using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PrecisionAndSpeed : MonoBehaviour
{
    public GameObject playerObject;  // Reference to the player GameObject
    public GameObject scoreManager;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI precisionText;
    public TextMeshProUGUI typingSpeedText;

    private SpellCasting spellCastingScript;
    private ScoreManager scoreManagerScript;

    private void Awake()
    {
        if (playerObject != null)
        {
            spellCastingScript = playerObject.GetComponent<SpellCasting>();
            scoreManagerScript = scoreManager.GetComponent<ScoreManager>();
        }

        if (spellCastingScript == null)
        {
            Debug.LogWarning("SpellCasting component not found on playerObject.");
        }
    }

    private void OnEnable()
    {
        UpdateResults();
    }

    public void UpdateResults()
    {
        if (spellCastingScript == null)
        {
            Debug.LogWarning("SpellCasting reference missing on EndMenu.");
            return;
        }
        string score = scoreManagerScript.GetFinalScore();
        float precision = spellCastingScript.GetTypingPrecisionPercentage();
        float avgSpeed = spellCastingScript.GetAverageTypingSpeed();

        scoreText.text = score;
        precisionText.text = $"{precision:F1}%";
        typingSpeedText.text = $"{avgSpeed:F1} cps";
    }
}

