using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpellBurst : MonoBehaviour
{
    public SpellScriptableObject SpellToCast;
    public GameObject hitParticleEffectPrefab;
    public float damageDelay = 0.05f;

    private bool hasDamaged = false;

    void Start()
    {
        Invoke(nameof(ApplyDamage), damageDelay);
        Destroy(gameObject, SpellToCast.Lifetime); // Auto-destroy after duration
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player")) return;

        Debug.Log("Hit");


        // Check for different enemy types
        EnemyAi enemy = other.GetComponent<EnemyAi>();
        EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
        EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

        if (enemy != null)
        {
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

        if (hitParticleEffectPrefab != null)
        {
            Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(this.gameObject);
    }
    void ApplyDamage()
    {
        if (hasDamaged) return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, SpellToCast.SpellRadius);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player")) continue;

            EnemyAi baseEnemy = collider.GetComponent<EnemyAi>();
            EnemyAiSniper sniper = collider.GetComponent<EnemyAiSniper>();
            EnemyAiHeavy heavy = collider.GetComponent<EnemyAiHeavy>();

            if (baseEnemy != null) baseEnemy.TakeDamage((int)SpellToCast.DamageAmount);
            else if (sniper != null) sniper.TakeDamage((int)SpellToCast.DamageAmount);
            else if (heavy != null) heavy.TakeDamage((int)SpellToCast.DamageAmount);
        }

        if (hitParticleEffectPrefab != null)
        {
            Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
        }

        hasDamaged = true;
    }
}
