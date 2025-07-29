using UnityEngine;

public class PowerUpEffect : MonoBehaviour
{
    private PowerUpManager  powerupManager;

    void Start()
    {
        GameObject managerObject = GameObject.FindGameObjectWithTag("PowerUp");

        if (managerObject != null)
        {
            powerupManager = managerObject.GetComponent<PowerUpManager>();
        }
        else
        {
            Debug.LogError("PowerUpManager object not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            powerupManager.TurnOnPowerUpMenu();
            Destroy(gameObject);
        }

    }
}
