using System.Collections.Generic;
using UnityEngine;

public enum SpellElement { Fire, Water, Earth, Air, Lightning, Shadow }
public enum SpellAction { Veil, Strike, Burst, Storm, Ultimate }

public enum DifficultyProfile { Easy, Medium, Hard }

public class SpellNameGenerator : MonoBehaviour
{
    [Header("Generated Words")]
    public Dictionary<SpellElement, string> ElementWords { get; private set; }
    public Dictionary<SpellAction, string> ActionWords { get; private set; }

    private string[] vowels = { "a", "e", "i", "o", "u" };
    private string[] commonConsonants = { "r", "t", "s", "n", "l", "d", "c", "m", "k" };
    private string[] hardConsonants = { "z", "x", "q", "v", "g", "p" };
    private string[] trickyCombos = { "th", "ph", "kr", "gr", "tr", "sh" };

    private void Awake()
    {
        ElementWords = new Dictionary<SpellElement, string>();
        ActionWords = new Dictionary<SpellAction, string>();

        // Generate names on game start
        GenerateSpellNames();
    }

    private void GenerateSpellNames()
    {
        foreach (SpellElement element in System.Enum.GetValues(typeof(SpellElement)))
        {
            DifficultyProfile difficulty = GetElementDifficulty(element);
            ElementWords[element] = GenerateSpellWord(difficulty);
        }

        foreach (SpellAction action in System.Enum.GetValues(typeof(SpellAction)))
        {
            DifficultyProfile difficulty = GetActionDifficulty(action);
            ActionWords[action] = GenerateSpellWord(difficulty);
        }
    }

    private DifficultyProfile GetElementDifficulty(SpellElement element)
    {
        return element switch
        {
            SpellElement.Fire => DifficultyProfile.Easy,
            SpellElement.Water => DifficultyProfile.Medium,
            SpellElement.Earth => DifficultyProfile.Medium,
            SpellElement.Air => DifficultyProfile.Medium,
            SpellElement.Lightning => DifficultyProfile.Hard,
            SpellElement.Shadow => DifficultyProfile.Hard,
            _ => DifficultyProfile.Medium
        };
    }

    private DifficultyProfile GetActionDifficulty(SpellAction action)
    {
        return action switch
        {
            SpellAction.Veil => DifficultyProfile.Easy,
            SpellAction.Strike => DifficultyProfile.Medium,
            SpellAction.Burst => DifficultyProfile.Medium,
            SpellAction.Storm => DifficultyProfile.Hard,
            SpellAction.Ultimate => DifficultyProfile.Hard,
            _ => DifficultyProfile.Medium
        };
    }

    private string GenerateSpellWord(DifficultyProfile profile)
    {
        int syllables = profile == DifficultyProfile.Easy ? 2 : profile == DifficultyProfile.Medium ? 3 : 4;
        bool allowCombos = profile == DifficultyProfile.Hard;
        string result = "";

        for (int i = 0; i < syllables; i++)
        {
            string consonant = ChooseRandom(profile == DifficultyProfile.Hard ? hardConsonants : commonConsonants);
            string vowel = ChooseRandom(vowels);

            result += consonant + vowel;

            // Occasionally insert a tricky combo for hard difficulty
            if (allowCombos && Random.value < 0.3f)
            {
                result += ChooseRandom(trickyCombos);
            }
        }

        return Capitalize(result);
    }

    private string ChooseRandom(string[] array)
    {
        return array[Random.Range(0, array.Length)];
    }

    private string Capitalize(string word)
    {
        if (string.IsNullOrEmpty(word)) return word;
        return char.ToUpper(word[0]) + word.Substring(1);
    }

    // Public API

    public string GetSpellName(SpellElement element, SpellAction action)
    {
        string elementWord = ElementWords[element];
        string actionWord = ActionWords[action];
        return $"{elementWord} {actionWord}";
    }

    public string GetElementWord(SpellElement element) => ElementWords[element];
    public string GetActionWord(SpellAction action) => ActionWords[action];
}
