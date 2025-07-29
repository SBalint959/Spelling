using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] private GameObject PowerupMenu;

    public void TurnOnPowerUpMenu()
    {
        PowerupMenu.SetActive(true);
    }
}
