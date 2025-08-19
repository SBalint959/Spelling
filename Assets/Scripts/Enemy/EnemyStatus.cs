using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyStatus : MonoBehaviour
{
    private NavMeshAgent agent;
    private Coroutine slowRoutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void ApplySlow(float slowMultiplier, float duration)
    {
        if (agent == null) return;

        // If already slowed, restart the effect
        if (slowRoutine != null)
            StopCoroutine(slowRoutine);

        slowRoutine = StartCoroutine(SlowRoutine(slowMultiplier, duration));
    }

    private IEnumerator SlowRoutine(float slowMultiplier, float duration)
    {
        float originalSpeed = agent.speed;
        agent.speed *= slowMultiplier; // e.g., 0.5f = 50% speed

        yield return new WaitForSeconds(duration);

        agent.speed = originalSpeed;
        slowRoutine = null;
    }
}
