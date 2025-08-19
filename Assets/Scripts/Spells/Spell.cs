using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Spell : MonoBehaviour
{
    public SpellScriptableObject SpellToCast;
    public GameObject hitParticleEffectPrefab;

    private SphereCollider myCollider;
    private Rigidbody myRigidBody;
    // public bool useOwnCollider = false;

    private HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();
    private bool isDOTActive = false;
    private bool hasExploded = false;

    private float moveSpeed = 13f;

    private void Awake()
    {

        StartCoroutine(ApplyDamageOverTime());
        // if (!useOwnCollider)
        // {
        //     SphereCollider myCollider = GetComponent<SphereCollider>();
        //     if (myCollider == null)
        //     {
        //         myCollider = gameObject.AddComponent<SphereCollider>();
        //     }

        //     myCollider.isTrigger = true;
        //     myCollider.radius = SpellToCast.SpellRadius;
        // }

        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.isKinematic = true;

        Destroy(gameObject, SpellToCast.Lifetime);
    }

    private void Update()
    {
        if (SpellToCast.Speed > 0)
            transform.Translate(Vector3.forward * SpellToCast.Speed * Time.deltaTime);

        if (SpellToCast.SpellType == "Destruction" && SpellToCast.SpellElement == "Fire")
        {
            if (!hasExploded)
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;

        Debug.Log("Hit trigger");

        if (SpellToCast.SpellType == "Strike")
        {
            // Check for different enemy types
            EnemyAi enemy = other.GetComponent<EnemyAi>();
            EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
            EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

            EnemyStatus status = other.GetComponent<EnemyStatus>();
            if (SpellToCast.SpellElement == "Fire" || SpellToCast.SpellElement == "Ice" || SpellToCast.SpellElement == "Dark")
            {
                //DAMAGE
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

                //SLOW
                if (status != null && SpellToCast.applySlow)
                {
                    status.ApplySlow(0.5f, 3f); // slow to 50% speed for 3 seconds
                }

                //HIT PARTICLE
                if (hitParticleEffectPrefab != null && (enemy != null || sniper != null || heavy != null))
                {
                    Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
                }


                if (SpellToCast.applyKnockback)
                {
                    var kb = other.GetComponent<EnemyKnockback>();
                    if (kb != null)
                    {
                        Vector3 knockDir = (other.transform.position - transform.position).normalized;
                        kb.ApplyKnockback(knockDir, SpellToCast.knockbackForce);
                    }
                }

                if (!SpellToCast.passThrough) Destroy(gameObject);
            }
            else if (SpellToCast.SpellElement == "Lightning")
            {
                StartCoroutine(DelayedDamage(enemy, sniper, heavy, SpellToCast.DamageAmount, 0.5f));
                //SLOW
                if (status != null && SpellToCast.applySlow)
                {
                    status.ApplySlow(0.01f, 3f); // slow to 50% speed for 3 seconds
                }

                //HIT PARTICLE
                if (hitParticleEffectPrefab != null)
                {
                    Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
                }
            }
        }
        else if (SpellToCast.SpellType == "Burst")
        {
            // Check for different enemy types
            EnemyAi enemy = other.GetComponent<EnemyAi>();
            EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
            EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

            Rigidbody targetRb = other.attachedRigidbody;
            EnemyStatus status = other.GetComponent<EnemyStatus>();

            if (SpellToCast.SpellElement != "Lightning")
            {


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
            }


            //SLOW
            if (status != null && SpellToCast.applySlow && SpellToCast.SpellElement == "Lightning")
            {
                status.ApplySlow(0.01f, 3f); // slow to 50% speed for 3 seconds
            }
            if (!affectedEnemies.Contains(other.gameObject) && SpellToCast.SpellElement == "Lightning")
            {
                affectedEnemies.Add(other.gameObject);
            }


            if (SpellToCast.applyKnockback)
            {
                // Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                // targetRb.AddForce(knockbackDirection * SpellToCast.knockbackForce, ForceMode.Impulse);
                var kb = other.GetComponent<EnemyKnockback>();
                if (kb != null)
                {
                    Vector3 knockDir = (other.transform.position - transform.position).normalized;
                    kb.ApplyKnockback(knockDir, SpellToCast.knockbackForce);
                }
            }

            if (hitParticleEffectPrefab != null)
            {
                Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject, 2f);

        }
        else if (SpellToCast.SpellType == "Storm")
        {
            EnemyStatus status = other.GetComponent<EnemyStatus>();

            //SLOW
            if (status != null && SpellToCast.applySlow && SpellToCast.SpellElement == "Ice")
            {
                status.ApplySlow(0.5f, 3f); // slow to 50% speed for 3 seconds
            }
            if (status != null && SpellToCast.applySlow && SpellToCast.SpellElement == "Lightning")
            {
                status.ApplySlow(0.1f, 3f); // slow to 50% speed for 3 seconds
            }

            if (SpellToCast.applyKnockback)
            {
                var kb = other.GetComponent<EnemyKnockback>();
                if (kb != null)
                {
                    Vector3 knockDir = (other.transform.position - transform.position).normalized;
                    kb.ApplyKnockback(knockDir, SpellToCast.knockbackForce);
                }
            }

            if (!affectedEnemies.Contains(other.gameObject))
            {
                affectedEnemies.Add(other.gameObject);
            }
            return;
        }
        else if (SpellToCast.SpellType == "Destruction")
        {
            if (SpellToCast.SpellElement == "Fire")
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    if (hitParticleEffectPrefab != null)
                    {
                        Vector3 spawnPosition = transform.position;
                        spawnPosition.y -= 2f;
                        Instantiate(hitParticleEffectPrefab, spawnPosition, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
            }
            else if (SpellToCast.SpellElement == "Ice")
            {
                EnemyStatus status = other.GetComponent<EnemyStatus>();
                EnemyAi enemy = other.GetComponent<EnemyAi>();
                EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
                EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

                Rigidbody targetRb = other.attachedRigidbody;

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




                //SLOW
                if (status != null && SpellToCast.applySlow)
                {
                    status.ApplySlow(0.5f, 3f); // slow to 50% speed for 3 seconds
                }

            }
            else if (SpellToCast.SpellElement == "Lightning")
            {
                EnemyAi enemy = other.GetComponent<EnemyAi>();
                EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
                EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

                EnemyStatus status = other.GetComponent<EnemyStatus>();
                StartCoroutine(DelayedDamage(enemy, sniper, heavy, SpellToCast.DamageAmount, 1.5f));
                //SLOW
                if (status != null && SpellToCast.applySlow)
                {
                    status.ApplySlow(0.01f, 4f); // slow to 50% speed for 3 seconds
                }

                //HIT PARTICLE
                if (hitParticleEffectPrefab != null)
                {
                    Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
                }
            }
            else if (SpellToCast.SpellElement == "Dark")
            {
                if (!affectedEnemies.Contains(other.gameObject))
                {
                    affectedEnemies.Add(other.gameObject);
                }

                //Knockback towards spell
                if (SpellToCast.applyKnockback)
                {
                    var kb = other.GetComponent<EnemyKnockback>();
                    if (kb != null)
                    {
                        Vector3 knockDir = (transform.position - other.transform.position).normalized;
                        kb.ApplyKnockback(knockDir, SpellToCast.knockbackForce);
                    }
                }
            }
        }

    }

    private IEnumerator ApplyDamageOverTime()
    {
        float elapsed = 0f;

        while (elapsed < SpellToCast.Lifetime)
        {
            foreach (var obj in affectedEnemies)
            {
                if (obj == null) continue;

                EnemyAi enemy = obj.GetComponent<EnemyAi>();
                EnemyAiSniper sniper = obj.GetComponent<EnemyAiSniper>();
                EnemyAiHeavy heavy = obj.GetComponent<EnemyAiHeavy>();

                if (enemy != null) enemy.TakeDamage((int)SpellToCast.DamageAmount);
                else if (sniper != null) sniper.TakeDamage((int)SpellToCast.DamageAmount);
                else if (heavy != null) heavy.TakeDamage((int)SpellToCast.DamageAmount);

                if (SpellToCast.applyKnockback)
                {
                    var kb = obj.GetComponent<EnemyKnockback>();
                    if (kb != null)
                    {
                        Vector3 knockDir = (obj.transform.position - transform.position).normalized;
                        kb.ApplyKnockback(knockDir, SpellToCast.knockbackForce);
                    }
                }

                Debug.Log("enemy burned");
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        Destroy(gameObject);
    }
    
    private IEnumerator DelayedDamage(EnemyAi enemy, EnemyAiSniper sniper, EnemyAiHeavy heavy, float damage, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy != null)
        {
            enemy.TakeDamage((int)damage);
        }
        else if (sniper != null)
        {
            sniper.TakeDamage((int)damage);
        }
        else if (heavy != null)
        {
            heavy.TakeDamage((int)damage);
        }
    }




    // void OnParticleCollision(GameObject other)
    // {
    //     if (other.CompareTag("Player")) return;

    //     Debug.Log("Hit collision");

    //     // Check for different enemy types
    //     EnemyAi enemy = other.GetComponent<EnemyAi>();
    //     EnemyAiSniper sniper = other.GetComponent<EnemyAiSniper>();
    //     EnemyAiHeavy heavy = other.GetComponent<EnemyAiHeavy>();

    //     if (enemy != null)
    //     {
    //         enemy.TakeDamage((int)SpellToCast.DamageAmount);
    //     }
    //     else if (sniper != null)
    //     {
    //         sniper.TakeDamage((int)SpellToCast.DamageAmount);
    //     }
    //     else if (heavy != null)
    //     {
    //         heavy.TakeDamage((int)SpellToCast.DamageAmount);
    //     }

    //     if (hitParticleEffectPrefab != null)
    //     {
    //         Instantiate(hitParticleEffectPrefab, transform.position, Quaternion.identity);
    //     }

    //     Destroy(gameObject, 2f);
    // }

}
