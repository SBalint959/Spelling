using UnityEngine;

public class SpellDestruction : MonoBehaviour
{
    public SpellScriptableObject SpellToCast;
    public GameObject explosionEffectPrefab;

    public float moveSpeed = 10f;
    public float maxLifetime = 5f;

    private Rigidbody rb;
    private bool hasExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, maxLifetime);
    }

    void Update()
    {
        if (!hasExploded)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("collision detected");
    //     if (hasExploded) return;
    //     hasExploded = true;

    //     // Spawn the explosion effect at the hit point
    //     Vector3 contactPoint = collision.contacts[0].point;
    //     Instantiate(explosionEffectPrefab, contactPoint, Quaternion.identity);

    //     // Destroy the comet immediately
    //     Destroy(gameObject);
    // }
}
