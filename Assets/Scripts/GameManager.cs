using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerObject;

    // Full lists of all possible elements and actions
    public List<SpellElement> allElements = new List<SpellElement>();
    public List<SpellAction> allActions = new List<SpellAction>();

    public SpellNameGenerator spellNameGenerator;

    // Discovered elements and actions
    private List<SpellElement> discoveredElements = new List<SpellElement>();
    private List<SpellAction> discoveredActions = new List<SpellAction>();


    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject HpBarCanvas;
    [SerializeField] private GameObject PointsCanvas;

    private bool isPaused = false;
    private bool isTyping;

    private SpellCasting spellCastingScript;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            spellCastingScript = playerObject.GetComponent<SpellCasting>();

            InitializeLists();
            DiscoverAction(SpellAction.Strike);
            // DiscoverElement(SpellElement.Fire);
            SpellElement[] allElements = (SpellElement[])System.Enum.GetValues(typeof(SpellElement));
            SpellElement randomElement = allElements[Random.Range(0, allElements.Length)];
            DiscoverElement(randomElement);

            //DELETE LATER
            // DiscoverAction(SpellAction.Burst);
            // DiscoverAction(SpellAction.Storm);
            // DiscoverAction(SpellAction.Destruction);
            // DiscoverElement(SpellElement.Ice);
            // DiscoverElement(SpellElement.Lightning);
            // DiscoverElement(SpellElement.Shadow);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Escape))
        {
            isTyping = spellCastingScript.IsPlayerTyping();
            if (!isTyping)
            {
                if (isPaused)
                {
                    UnPauseGame();
                }
                else
                {
                    PauseGame();

                }
            }
        }
    }

    private void InitializeLists()
    {
        // Fill the lists based on enums
        allElements.AddRange((SpellElement[])System.Enum.GetValues(typeof(SpellElement)));
        allActions.AddRange((SpellAction[])System.Enum.GetValues(typeof(SpellAction)));
    }

    // Public functions to add discoveries
    public void DiscoverElement(SpellElement element)
    {
        if (!discoveredElements.Contains(element))
        {
            discoveredElements.Add(element);
            Debug.Log($"Discovered Element: {element}");
        }
    }

    public void DiscoverAction(SpellAction action)
    {
        if (!discoveredActions.Contains(action))
        {
            discoveredActions.Add(action);
            Debug.Log($"Discovered Action: {action}");
        }
    }

    public List<SpellElement> GetUndiscoveredElements()
    {
        List<SpellElement> allElements = new List<SpellElement>
        {
            SpellElement.Fire,
            SpellElement.Lightning,
            SpellElement.Shadow,
            SpellElement.Ice
        };

        return allElements.Except(discoveredElements).ToList();
    }

    public List<SpellAction> GetUndiscoveredActions()
    {
        List<SpellAction> allActions = new List<SpellAction>
        {
            SpellAction.Strike,
            SpellAction.Burst,
            SpellAction.Storm,
            SpellAction.Destruction,
        };

        return allActions.Except(discoveredActions).ToList();
    }

    public List<SpellElement> GetDiscoveredElements() => new List<SpellElement>(discoveredElements);
    public List<SpellAction> GetDiscoveredActions() => new List<SpellAction>(discoveredActions);



    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);
        HpBarCanvas.SetActive(false);
        PointsCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1f;
        PauseMenu.SetActive(false);
        HpBarCanvas.SetActive(true);
        PointsCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit Game");

        // If running in the editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in a build
        Application.Quit();
#endif
    }
}

