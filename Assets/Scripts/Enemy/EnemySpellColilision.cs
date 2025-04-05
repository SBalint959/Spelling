using UnityEngine;

public class EnemySpellColilision : MonoBehaviour
{
    public GameObject particleEffectPrefab;

    public float collisionDelay = 0.2f;

    private bool canCollide = false;

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

        if (particleEffectPrefab != null)
        {
            Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
