using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] private float duration = 1f;          // how long the shove lasts
    [SerializeField] private AnimationCurve ease = null;       // assign EaseOut curve in Inspector
    [SerializeField] private float forceToDistance = 0.45f;    // tune: meters per unit of "force"

    private NavMeshAgent agent;
    private Coroutine routine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (ease == null)
        {
            // default ease-out if none assigned
            ease = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;

        Debug.Log("Knockback");

        direction.y = 0f; // keep it horizontal
        direction.Normalize();

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 dir, float force)
    {
        // Pause pathing and shove manually
        agent.isStopped = true;

        float t = 0f;
        float totalDist = Mathf.Max(0f, force) * forceToDistance;

        while (t < duration)
        {
            if (!agent.enabled || !agent.isOnNavMesh) yield break;

            float k = t / duration;
            // distance this frame (ease gives a tapering velocity)
            float step = (ease.Evaluate(k) * totalDist / duration) * Time.deltaTime;

            // Debug.Log("Knockbacking");
            // Debug.Log(dir);
            // Debug.Log(step);

            agent.Move(dir * step * 5);
            t += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        routine = null;
    }
}
