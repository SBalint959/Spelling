using UnityEngine;

public class SpellExplosionDamage : MonoBehaviour
{
    public SpellScriptableObject SpellToCast;

    private void Start()
    {
        // Destroy explosion after a short duration
        Destroy(gameObject, 2.5f); // Adjust based on particle duration
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        Debug.Log("Explosion hit");

        // Apply damage to any enemy types
        EnemyAi enemy = other.GetComponent<EnemyAi>();
        EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
        EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

        if (enemy != null)
        {
            Debug.Log(SpellToCast.DamageAmount);
            enemy.TakeDamage((int)SpellToCast.DamageAmount);
        }
        else if (sniper != null)
        {
            sniper.TakeDamage((int)SpellToCast.DamageAmount);
        }
        else if (heavy != null)
        {
            heavy.TakeDamage((int)SpellToCast.DamageAmount);
        }

        // Destroy(gameObject);

    }
}
