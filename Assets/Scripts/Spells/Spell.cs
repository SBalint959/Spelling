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
    public bool useOwnCollider = false;

    private HashSet<GameObject> affectedEnemies = new HashSet<GameObject>();
    private bool isDOTActive = false;

    private void Awake()
    {

        StartCoroutine(ApplyDamageOverTime());
        if (!useOwnCollider)
        {
            SphereCollider myCollider = GetComponent<SphereCollider>();
            if (myCollider == null)
            {
                myCollider = gameObject.AddComponent<SphereCollider>();
            }

            myCollider.isTrigger = true;
            myCollider.radius = SpellToCast.SpellRadius;
        }

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

        Debug.Log("Hit trigger");

        if (SpellToCast.SpellType == "Strike")
        {
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

            Destroy(gameObject);
        }
        else if (SpellToCast.SpellType == "Burst")
        {
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

            Destroy(gameObject, 2f);
        }
        else if (SpellToCast.SpellType == "Storm")
        {
            if (!affectedEnemies.Contains(other.gameObject))
            {
                affectedEnemies.Add(other.gameObject);
            }
            return;
            
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

                Debug.Log("enemy burned");
            }

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }

        Destroy(gameObject);
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
