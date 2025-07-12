using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class DisplaySpells : MonoBehaviour
{

    public TextMeshProUGUI[] elementTexts = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] actionTexts = new TextMeshProUGUI[5];

    

    private void OnEnable()
    {
        UpdateSpellDisplay();
    }

    public void UpdateSpellDisplay()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager or SpellNameGenerator is not assigned.");
            return;
        }

        // === Element display (ordered by enum) ===
        var discoveredElements = GameManager.Instance.GetDiscoveredElements();

        for (int i = 0; i < elementTexts.Length && i < System.Enum.GetValues(typeof(SpellElement)).Length; i++)
        {
            SpellElement element = (SpellElement)i;

            if (discoveredElements.Contains(element))
            {
                string word = GameManager.Instance.spellNameGenerator.GetElementWord(element);
                elementTexts[i].text = word;
            }
            else
            {
                elementTexts[i].text = "";
            }
        }

        // === Action display (ordered by enum) ===
        var discoveredActions = GameManager.Instance.GetDiscoveredActions();

        for (int j = 0; j < actionTexts.Length && j < System.Enum.GetValues(typeof(SpellAction)).Length; j++)
        {
            SpellAction action = (SpellAction)j;

            if (discoveredActions.Contains(action))
            {
                string word = GameManager.Instance.spellNameGenerator.GetActionWord(action);
                actionTexts[j].text = word;
            }
            else
            {
                actionTexts[j].text = "";
            }
        }
    }
}

