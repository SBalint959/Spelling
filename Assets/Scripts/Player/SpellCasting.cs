using System;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [SerializeField] private Spell[] availableSpells; // Array of all available spells
    [SerializeField] private Spell[] alternativeSpells; // Weak and strong versions of spells
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private SpellNameGenerator spellNameGenerator;


    private PlayerMovement playerMovement;
    private string currentInput = ""; // Stores player's input for spell name
    private bool isTyping = false; // Whether the player is typing a spell name

    


    private float typingStartTime; // When typing starts

    void Awake()
    {
        string fireBall = spellNameGenerator.GetSpellName(SpellElement.Fire, SpellAction.Veil);
        Debug.Log($"Discovered spell: {fireBall}");
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the player object!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTyping)
        {
            StartTyping();
        }

        if (isTyping)
        {
            HandleTyping();
        }
    }

    void StartTyping()
    {
        isTyping = true;
        currentInput = ""; // Clear previous input
 
        typingStartTime = Time.time; // Start tracking time
        playerMovement.isMovementDisabled = true;  // Disable movement while typing
    }

    void HandleTyping()
    {
        bool fIgnored = false; //Whether first f was ignored
        foreach (char c in Input.inputString)
        {
            if (c == '\n' || c == '\r')
            {
                CastSpell();
                return;
            }
            else
            {
                if (currentInput.Length == 0 && c == 'f' && !fIgnored) 
                {
                    fIgnored = true;
                    continue;
                }
                currentInput += c; // Add character to input
            }
        }
    }

    void CastSpell()
    {
        isTyping = false;
        playerMovement.isMovementDisabled = false;

        float typingDuration = Time.time - typingStartTime;

        Spell bestMatch = null;
        int bestMatchDistance = int.MaxValue;

        // Find the closest match from the main spell list
        foreach (Spell spell in availableSpells)
        {
            int distance = LevenshteinDistance(currentInput, spell.name);
            if (distance < bestMatchDistance)
            {
                bestMatchDistance = distance;
                bestMatch = spell;
            }
        }

        if (bestMatch == null)
        {
            Debug.Log($"Spell '{currentInput}' not recognized!");
            return;
        }

        SpellVariant? variant = DetermineSpellVariant(typingDuration, bestMatchDistance, bestMatch.name.Length);

        if (variant == null)
        {
            Debug.Log($"Spell '{currentInput}' failed! Too many mistakes.");
            return;
        }

        Spell spellToCast = GetSpellByVariant(bestMatch.name, (SpellVariant)variant);
        if (spellToCast == null)
        {
            Debug.Log($"No prefab found for spell variant: {bestMatch.name} ({variant})");
            return;
        }

        InstantiateSpell(spellToCast, (SpellVariant)variant);
    }

    Spell GetSpellByVariant(string baseName, SpellVariant variant)
    {
        string targetName = baseName;

        switch (variant)
        {
            case SpellVariant.Perfect:
                return Array.Find(availableSpells, s => s.name.Equals(baseName, StringComparison.OrdinalIgnoreCase));

            case SpellVariant.Normal:
                targetName = baseName + "_mid";
                break;

            case SpellVariant.Weak:
                targetName = baseName + "_weak";
                break;
        }

        return Array.Find(alternativeSpells, s => s.name.Equals(targetName, StringComparison.OrdinalIgnoreCase));
    }




    SpellVariant? DetermineSpellVariant(float timeTaken, int errorCount, int spellLength)
    {
        float errorRatio = (float)errorCount / spellLength;

        if (errorRatio > 0.5f) //If more than 50% of the spell is wrong, cancel the spell
        {
            return null; // No valid spell variant
        }

        if (errorRatio == 0 && timeTaken < 2f) // Perfect Cast (Fast & Accurate)
        {
            return SpellVariant.Perfect;
        }
        else if (errorRatio <= 0.3 && timeTaken < 4f) // Normal Cast (Moderate performance)
        {
            return SpellVariant.Normal;
        }
        else // Weak Cast (Too slow or some mistakes)
        {
            return SpellVariant.Weak;
        }
    }

    void InstantiateSpell(Spell spell, SpellVariant variant)
{
    // Vector3 spawnOffset = playerCamera.forward * 1.2f + Vector3.up * 0.5f;
    Vector3 spawnPosition = castPoint.position;
    Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

    Spell newSpell = Instantiate(spell, spawnPosition, spawnRotation);
    // newSpell.SetVariant(variant); // Uncomment if needed
    Debug.Log($"Casted {variant} version of {spell.name}");
}


    // Levenshtein Distance Algorithm (Fuzzy Matching)
    int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;

        int[,] dp = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++)
            dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                dp[i, j] = Mathf.Min(
                    Mathf.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost
                );
            }
        }
        return dp[a.Length, b.Length];
    }
}

// Enum for spell variation
public enum SpellVariant
{
    Perfect,
    Normal,
    Weak
}
