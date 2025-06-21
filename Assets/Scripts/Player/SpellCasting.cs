using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellCasting : MonoBehaviour
{
    [SerializeField] private Spell[] availableSpells;       // perfect variants
    [SerializeField] private Spell[] alternativeSpells;     // _mid and _weak variants
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private SpellNameGenerator spellNameGenerator;
    [SerializeField] private GameObject playerGameObject;


    private PlayerMovement playerMovement;
    private string currentInput = "";
    private bool isTyping = false;
    private float typingStartTime;

    private Dictionary<string, string> generatedToRealNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("PlayerMovement component not found on the player object!");

        // Generate names dynamically and map to actual spell prefab names
        foreach (Spell spell in availableSpells)
        {
            if (TryParseElementAndAction(spell.name, out SpellElement element, out SpellAction action))
            {
                string generatedName = spellNameGenerator.GetSpellName(element, action);
                generatedToRealNameMap[generatedName] = spell.name;
                Debug.Log($"Mapped '{generatedName}' to spell '{spell.name}'");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTyping)
            StartTyping();

        if (isTyping)
            HandleTyping();
    }

    void StartTyping()
    {
        isTyping = true;
        currentInput = "";
        typingStartTime = Time.time;
        playerMovement.isMovementDisabled = true;
    }

    void HandleTyping()
    {
        bool fIgnored = false;
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
                currentInput += c;
            }
        }
    }

    void CastSpell()
    {
        isTyping = false;
        playerMovement.isMovementDisabled = false;

        float typingDuration = Time.time - typingStartTime;

        string bestGeneratedName = null;
        int bestDistance = int.MaxValue;

        // Compare input against generated names
        foreach (string generated in generatedToRealNameMap.Keys)
        {
            int distance = LevenshteinDistance(currentInput, generated);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestGeneratedName = generated;
            }
        }

        if (bestGeneratedName == null)
        {
            Debug.Log($"Spell '{currentInput}' not recognized!");
            return;
        }

        string realSpellName = generatedToRealNameMap[bestGeneratedName];

        SpellVariant? variant = DetermineSpellVariant(typingDuration, bestDistance, bestGeneratedName.Length);
        if (variant == null)
        {
            Debug.Log($"Spell '{currentInput}' failed! Too many mistakes.");
            return;
        }

        Spell spellToCast = GetSpellByVariant(realSpellName, (SpellVariant)variant);
        if (spellToCast == null)
        {
            Debug.Log($"No prefab found for spell variant: {realSpellName} ({variant})");
            return;
        }
        Debug.Log(currentInput);
        InstantiateSpell(spellToCast, (SpellVariant)variant);
    }

    Spell GetSpellByVariant(string baseName, SpellVariant variant)
    {
        switch (variant)
        {
            case SpellVariant.Perfect:
                return Array.Find(availableSpells, s => s.name.Equals(baseName, StringComparison.OrdinalIgnoreCase));

            case SpellVariant.Normal:
                return Array.Find(alternativeSpells, s => s.name.Equals(baseName + "_mid", StringComparison.OrdinalIgnoreCase));

            case SpellVariant.Weak:
                return Array.Find(alternativeSpells, s => s.name.Equals(baseName + "_weak", StringComparison.OrdinalIgnoreCase));

            default:
                return null;
        }
    }

    SpellVariant? DetermineSpellVariant(float timeTaken, int errorCount, int spellLength)
    {
        float errorRatio = (float)errorCount / spellLength;

        if (errorRatio > 0.5f) return null;
        if (errorRatio == 0 && timeTaken < 3f) return SpellVariant.Perfect;
        if (errorRatio <= 0.3f && timeTaken < 5f) return SpellVariant.Normal;
        return SpellVariant.Weak;
    }

    void InstantiateSpell(Spell spell, SpellVariant variant)
    {
        Vector3 spawnOffset = playerCamera.forward * 1.2f + Vector3.up * 0.5f;
        // Vector3 spawnOffset = playerCamera.forward * 1.2f;
        // Vector3 spawnPosition = castPoint.position + spawnOffset;
        Vector3 spawnPosition = castPoint.position;
        spawnPosition.y = playerGameObject.transform.position.y;
        Debug.Log($"Player position: {playerGameObject.transform.position.y}");
        Debug.Log($"SpawnPos: {spawnPosition}");
        Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

        Spell newSpell = Instantiate(spell, spawnPosition, spawnRotation);
        // Debug.Log($"spawn pos:{spawnPosition}, spawn rot {spawnRotation}" );
         Debug.Log($"[Spell Cast] castPoint position: {castPoint.position}, spawnPosition: {spawnPosition}");
        Debug.Log($"Casted {variant} version of {spell.name}");
    }

    int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;

        int[,] dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                dp[i, j] = Mathf.Min(
                    dp[i - 1, j] + 1,
                    Mathf.Min(dp[i, j - 1] + 1, dp[i - 1, j - 1] + cost)
                );
            }
        }
        return dp[a.Length, b.Length];
    }

    // Helper to infer element + action from spell name (e.g., "fireShield" → Fire + Shield)
    bool TryParseElementAndAction(string spellName, out SpellElement element, out SpellAction action)
    {
        foreach (SpellElement elem in Enum.GetValues(typeof(SpellElement)))
        {
            string prefix = elem.ToString().ToLower();
            if (spellName.ToLower().StartsWith(prefix))
            {
                string suffix = spellName.Substring(prefix.Length).ToLower();
                foreach (SpellAction act in Enum.GetValues(typeof(SpellAction)))
                {
                    if (suffix == act.ToString().ToLower())
                    {
                        element = elem;
                        action = act;
                        return true;
                    }
                }
            }
        }

        element = default;
        action = default;
        return false;
    }
}


// Enum for spell variation
public enum SpellVariant
{
    Perfect,
    Normal,
    Weak
}
