using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Spell : MonoBehaviour
{
    public SpellScriptableObject SpellToCast;
    public GameObject hitParticleEffectPrefab;

    private SphereCollider myCollider;
    private Rigidbody myRigidBody;

    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = SpellToCast.SpellRadius;

        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.isKinematic = true;

        Destroy(this.gameObject, SpellToCast.Lifetime);
    }

    private void Update()
    {
        if (SpellToCast.Speed > 0)
            transform.Translate(Vector3.forward * SpellToCast.Speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
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

}
