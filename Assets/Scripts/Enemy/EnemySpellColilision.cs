using UnityEngine;

public class EnemySpellColilision : MonoBehaviour
{
    public GameObject particleEffectPrefab;

    public float collisionDelay = 0.2f;

    private bool canCollide = false;
    public int damage = 10;

    void Start()
    {
        // Start a short delay before enabling collision detection
        // since it spawns within "enemy"
        Invoke(nameof(EnableCollision), collisionDelay);
    }

    void EnableCollision()
    {
        canCollide = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return;

        // Damage the player if hit
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("player damage");
            }
        }

        if (particleEffectPrefab != null)
        {
            Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
