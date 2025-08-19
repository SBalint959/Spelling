using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpellCasting : MonoBehaviour
{
    [SerializeField] private Spell[] availableSpells;       // perfect variants
    [SerializeField] private Spell[] alternativeSpells;     // _mid and _weak variants
    [SerializeField] private Transform castPoint;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private SpellNameGenerator spellNameGenerator;
    [SerializeField] private GameObject playerGameObject;

    public GameObject floatingTextPrefab;
    public Transform floatingTextSpawnPoint;

    public TextMeshProUGUI InputTextUI;
    public GameObject InputUI;


    private PlayerMovement playerMovement;
    private string currentInput = "";
    private bool isTyping = false;
    private float typingStartTime;

    private Dictionary<string, string> generatedToRealNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    // typing stats variables
    private int totalCharactersTyped = 0;
    private int totalCharactersMistyped = 0;
    private float totalTypingTime = 0f;
    // private float typingStartTime;

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
        // if (Input.GetKeyDown(KeyCode.F)  && !isTyping)
        //     StartTyping();

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isTyping) { StartTyping(); return; }
            else { CastSpell(); return; }
        }

        if (isTyping)
            HandleTyping();
    }

    void StartTyping()
    {
        isTyping = true;
        currentInput = "";
        InputUI.SetActive(true);
        typingStartTime = Time.time;
        playerMovement.isMovementDisabled = true;
    }
    public bool IsPlayerTyping()
    {
        return isTyping;
    }

    void HandleTyping()
    {
        // bool fIgnored = false;

        // // Check for Enter OR Left Ctrl separately
        // if (Input.GetKeyDown(KeyCode.LeftControl))
        // {
        //     CastSpell();
        //     return;
        // }

        foreach (char c in Input.inputString)
        {
            if (c == '\n' || c == '\r')
            {
                CastSpell();
                return;
            }
            else
            {
                // if (currentInput.Length == 0 && c == 'f' && !fIgnored)
                // {
                //     fIgnored = true;
                //     continue;
                // }
                currentInput += c;
                totalCharactersTyped++;
                InputTextUI.text = currentInput;
            }
        }
    }


    void CastSpell()
    {
        isTyping = false;
        playerMovement.isMovementDisabled = false;
        
        InputUI.SetActive(false);
        InputTextUI.text = "";

        float typingDuration = Time.time - typingStartTime;
        totalTypingTime += typingDuration;

        string bestGeneratedName = null;
        int bestDistance = int.MaxValue;

        // Normalize input to lowercase once
        string lowerInput = currentInput.ToLower();

        // Compare input against generated names (also lowercase)
        foreach (string generated in generatedToRealNameMap.Keys)
        {
            int distance = LevenshteinDistance(lowerInput, generated.ToLower());
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestGeneratedName = generated;
            }
        }

        totalCharactersMistyped += bestDistance;

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
        Debug.Log(bestGeneratedName);
        Debug.Log($"Best distance: '{bestDistance}'");
        InstantiateSpell(spellToCast, (SpellVariant)variant);

        //Write casted spell in UI
        Color spellColor = Color.white;
        switch (variant)
        {
            case SpellVariant.Perfect:
                spellColor = new Color(0.208f, 0.71f, 0.239f); //green
                break;
            case SpellVariant.Normal:
                spellColor = new Color(1f, 0.922f, 0f); //yellow
                break;
            case SpellVariant.Weak:
                spellColor = new Color(0.929f, 0.494f, 0f); //orange
                break;
        }

        if (floatingTextPrefab != null && floatingTextSpawnPoint != null)
        {
            GameObject textObj = Instantiate(floatingTextPrefab, floatingTextSpawnPoint.position, Quaternion.identity, floatingTextSpawnPoint.parent);
            FloatingSpellText floating = textObj.GetComponent<FloatingSpellText>();
            if (floating != null)
            {
                floating.SetText(currentInput, spellColor);
            }
        }
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
        // Debug.Log(errorRatio);

        if (errorRatio > 0.5f) return null;
        if (errorRatio == 0 && timeTaken < 3f) return SpellVariant.Perfect;
        if (errorRatio <= 0.3f && timeTaken < 5f) return SpellVariant.Normal;
        return SpellVariant.Weak;
    }

    void InstantiateSpell(Spell spell, SpellVariant variant)
    {
        string spellName = spell.name.ToLower();

        if (spellName.Contains("strike"))
        {
            InstantiateBallSpell(spell, variant);
        }
        else if (spellName.Contains("burst"))
        {
            InstantiateBurstSpell(spell, variant);
        }
        else if (spellName.Contains("storm"))
        {
            InstantiateStormSpell(spell);
        }
        else if (spellName.Contains("destruction"))
        {
            InstantiateDestructionSpell(spell);
        }
        else
        {
            Debug.LogWarning($"No cast type matched for spell name: {spell.name}");
        }
    }


    void InstantiateBallSpell(Spell spell, SpellVariant variant)
    {
        string spellName = spell.name.ToLower();
        if (spellName.Contains("fire") || spellName.Contains("ice"))
        {
            Vector3 spawnOffset = playerCamera.forward * 1.2f + Vector3.up * 0.5f;
            Vector3 spawnPosition = castPoint.position;
            spawnPosition.y = playerGameObject.transform.position.y;

            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

            Spell newSpell = Instantiate(spell, spawnPosition, spawnRotation);
        }
        else if (spellName.Contains("lightning"))
        {
            Vector3 spawnOffset = playerCamera.forward * 7f;
            Vector3 spawnPosition = castPoint.position + spawnOffset;
            spawnPosition.y = playerGameObject.transform.position.y - 1;

            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

            Spell newSpell = Instantiate(spell, spawnPosition, spawnRotation);
        }
        else if (spellName.Contains("shadow"))
        {
            // Vector3 spawnOffset =  Vector3.up * 0.5f;
            Vector3 spawnPosition = playerGameObject.transform.position;
            spawnPosition.y = playerGameObject.transform.position.y -1f;

            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

            Spell newSpell = Instantiate(spell, spawnPosition, spawnRotation);
        }

        // Debug.Log($"[Spell Cast] castPoint position: {castPoint.position}, spawnPosition: {spawnPosition}");
        // Debug.Log($"Casted {variant} version of {spell.name}");
    }

    void InstantiateBurstSpell(Spell spell, SpellVariant variant)
    {
        // SpellScriptableObject data = spell.SpellToCast;

        Vector3 spawnPosition = playerGameObject.transform.position;
        spawnPosition.y = playerGameObject.transform.position.y -1;
        Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);
        
        Spell newSpell = Instantiate(spell, spawnPosition, spawnRotation);

        // Debug.Log($"[Spell: Burst] Spawned {spell.name} at {spawnPosition}");
    }

    void InstantiateStormSpell(Spell spell)
    {
        float spawnDistance = 10f;

        if (spell.name.Contains("Ice"))
        {
            Debug.Log(spell.name);
            spawnDistance = 0f;
        }
        

        // Project forward and drop to ground height
        Vector3 forwardOffset = playerCamera.forward * spawnDistance;
        Vector3 spawnPosition = new Vector3(
            playerGameObject.transform.position.x + forwardOffset.x,
            0f, // Set to ground level (or raycast to get terrain height if needed)
            playerGameObject.transform.position.z + forwardOffset.z
        );

        // adjust height
        spawnPosition.y = playerGameObject.transform.position.y - 1.5f;

        Quaternion rotation = Quaternion.identity;
        Instantiate(spell, spawnPosition, rotation);
        // Debug.Log($"[Spell: Storm] Spawned {spell.name} at {spawnPosition}");
    }

    public void InstantiateDestructionSpell(Spell spellPrefab)
    {
        if (spellPrefab.name.Contains("Fire"))
        {
            // a point in front of the player
            Vector3 forwardTarget = playerCamera.position + playerCamera.forward * 15f;

            // spawn position above the player
            Vector3 spawnPosition = playerGameObject.transform.position + Vector3.up * 15f;

            // direction from spawn point to forwardTarget
            Vector3 direction = (forwardTarget - spawnPosition).normalized;
            Quaternion spawnRotation = Quaternion.LookRotation(direction);

            Spell newSpell = Instantiate(spellPrefab, spawnPosition, spawnRotation);
        }

        else if (spellPrefab.name.Contains("Ice"))
        {
            Vector3 spawnPosition = playerGameObject.transform.position;
            spawnPosition.y = playerGameObject.transform.position.y - 2;
            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);
            Spell newSpell = Instantiate(spellPrefab, spawnPosition, spawnRotation);
        }
        else if (spellPrefab.name.Contains("Lightning"))
        {
            Vector3 spawnOffset = playerCamera.forward * 8f;
            Vector3 spawnPosition = castPoint.position + spawnOffset;
            spawnPosition.y = playerGameObject.transform.position.y - 1;

            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

            Spell newSpell = Instantiate(spellPrefab, spawnPosition, spawnRotation);
        }
        else if (spellPrefab.name.Contains("Shadow"))
        {
            Vector3 spawnOffset = playerCamera.forward * 8f;
            Vector3 spawnPosition = castPoint.position + spawnOffset;
            spawnPosition.y = playerGameObject.transform.position.y - 2;

            Quaternion spawnRotation = Quaternion.LookRotation(playerCamera.forward);

            Spell newSpell = Instantiate(spellPrefab, spawnPosition, spawnRotation);
        }

        
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


    // function to fetch precision percentage
    public float GetTypingPrecisionPercentage()
    {
        if (totalCharactersTyped == 0) return 0f;
        float correct = totalCharactersTyped - totalCharactersMistyped;
        return Mathf.Clamp01(correct / (float)totalCharactersTyped) * 100f;
    }
    
    
    public float GetAverageTypingSpeed()
    {
        if (totalTypingTime <= 0f) return 0f;
        return totalCharactersTyped / totalTypingTime;
    }
}


// Enum for spell variation
public enum SpellVariant
{
    Perfect,
    Normal,
    Weak
}
