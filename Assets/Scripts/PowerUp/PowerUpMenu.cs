using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PowerUpMenu : MonoBehaviour
{
    [Header("UI References")]
    // public GameObject menuPanel;

    [Header("Element Images")]
    public GameObject fireImage1;
    public GameObject lightningImage1;
    public GameObject shadowImage1;
    public GameObject iceImage1;

    public GameObject fireImage2;
    public GameObject lightningImage2;
    public GameObject shadowImage2;
    public GameObject iceImage2;

    [Header("Action Texts")]
    public TMP_Text actionText1;
    public TMP_Text actionText2;

    private SpellElement? offeredElement1 = null;
    private SpellElement? offeredElement2 = null;
    private SpellAction? offeredAction1 = null;
    private SpellAction? offeredAction2 = null;

    public GameManager gameManager;

    public GameObject PowerUpParticleEffectPrefab;
    public GameObject Player;

    void Start()
    {
        gameManager = GameManager.Instance;
        // menuPanel.SetActive(false);
    }

    void OnEnable()
    {
        // menuPanel.SetActive(true);
        PauseGame();

        List<SpellElement> undiscoveredElements = gameManager.GetUndiscoveredElements();
        List<SpellAction> undiscoveredActions = gameManager.GetUndiscoveredActions();

        bool elementAvailable = undiscoveredElements.Count > 0;
        bool actionAvailable = undiscoveredActions.Count > 0;

        ResetVisuals();

        if (elementAvailable && actionAvailable)
        {
            // One element and one action
            offeredElement1 = undiscoveredElements[Random.Range(0, undiscoveredElements.Count)];
            offeredAction1 = undiscoveredActions[Random.Range(0, undiscoveredActions.Count)];

            ShowElementImage(offeredElement1.Value, true);
            actionText1.text = offeredAction1.Value.ToString();
        }
        else if (elementAvailable && !actionAvailable && undiscoveredElements.Count >= 2)
        {
            // Two elements
            List<SpellElement> chosen = undiscoveredElements.OrderBy(e => Random.value).Take(2).ToList();
            offeredElement1 = chosen[0];
            offeredElement2 = chosen[1];

            ShowElementImage(chosen[0], true);
            ShowElementImage(chosen[1], false);
        }
        else if (actionAvailable)
        {
            // Two actions
            List<SpellAction> chosen = undiscoveredActions.OrderBy(a => Random.value).Take(2).ToList();
            offeredAction1 = chosen[0];
            offeredAction2 = chosen[1];

            actionText1.text = offeredAction1.Value.ToString();
            actionText2.text = offeredAction2.Value.ToString();
        }
    }

    private void ResetVisuals()
    {
        fireImage1.SetActive(false);
        lightningImage1.SetActive(false);
        shadowImage1.SetActive(false);
        iceImage1.SetActive(false);

        fireImage2.SetActive(false);
        lightningImage2.SetActive(false);
        shadowImage2.SetActive(false);
        iceImage2.SetActive(false);

        actionText1.text = "";
        actionText2.text = "";
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void PlayPowerUpEffect()
    {
        if (PowerUpParticleEffectPrefab != null)
        {
            Vector3 spawnPosition = Player.transform.position;
            spawnPosition.y -= 2f;
            Instantiate(PowerUpParticleEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private void ShowElementImage(SpellElement element, bool isFirst)
    {
        switch (element)
        {
            case SpellElement.Fire:
                (isFirst ? fireImage1 : fireImage2).SetActive(true);
                break;
            case SpellElement.Lightning:
                (isFirst ? lightningImage1 : lightningImage2).SetActive(true);
                break;
            case SpellElement.Shadow:
                (isFirst ? shadowImage1 : shadowImage2).SetActive(true);
                break;
            case SpellElement.Ice:
                (isFirst ? iceImage1 : iceImage2).SetActive(true);
                break;
        }
    }

    public void SelectOption1()
    {
        if (offeredElement1 != null)
        {
            gameManager.DiscoverElement(offeredElement1.Value);
        }
        else if (offeredAction2 != null)
        {
            gameManager.DiscoverAction(offeredAction2.Value);
        }

        CleanupAndClose();
    }

    public void SelectOption2()
    {
        if (offeredElement2 != null)
        {
            gameManager.DiscoverElement(offeredElement2.Value);
        }
        else if (offeredAction1 != null)
        {
            gameManager.DiscoverAction(offeredAction1.Value);
        }

        CleanupAndClose();
    }

    private void CleanupAndClose()
    {
        offeredElement1 = null;
        offeredElement2 = null;
        offeredAction1 = null;
        offeredAction2 = null;

        ResetVisuals();
        UnPauseGame();
        PlayPowerUpEffect();
        gameObject.SetActive(false);
    }
}
