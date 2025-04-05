using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //CHECK IF STUCK PARAMETRS
    private Vector3 lastPosition;
    private float stuckCheckInterval = 0.5f;
    private float stuckThreshold = 0.2f;
    private float maxStuckTime = 5f;
    private float stuckTimer = 0f;
    private float timeSinceLastCheck = 0f;
    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patrolling()
{
    // Debug.Log("Patrolling");
    // Debug.Log("Agent position: " + transform.position);
    // Debug.Log("Walkpoint position: " + walkPoint);

    if (!walkPointSet)
        SearchWalkPoint();

    if (walkPointSet)
        agent.SetDestination(walkPoint);

    Vector3 distanceToWalkPoint = transform.position - walkPoint;

    // Walkpoint reached
    if (distanceToWalkPoint.magnitude < 1f)
    {
        walkPointSet = false;
        stuckTimer = 0f; // Reset timer on successful arrival
    }

    // Stuck detection logic
    timeSinceLastCheck += Time.deltaTime;
    if (timeSinceLastCheck >= stuckCheckInterval)
    {
        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        if (movedDistance < stuckThreshold)
        {
            stuckTimer += timeSinceLastCheck;
        }
        else
        {
            stuckTimer = 0f; // Reset if movement occurred
        }

        lastPosition = transform.position;
        timeSinceLastCheck = 0f;
    }

    // Force new walkpoint if stuck too long
    if (stuckTimer >= maxStuckTime)
    {
        // Debug.LogWarning("Bot stuck! Forcing new walk point.");
        walkPointSet = false;
        stuckTimer = 0f;
    }
}

    private void SearchWalkPoint() {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y,  transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;

        // Debug.Log(walkPoint);
        
    }
    private void ChasePlayer() {
        // Debug.Log("Chasing");
        // Debug.Log("Player position: " + player.position);
        // Debug.Log("Agent position: " + transform.position);
        agent.SetDestination(player.position);
    }
    private void AttackPlayer() {
        // Debug.Log("Attacking");
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(directionToPlayer);
        Vector3 spawnPosition = transform.position + directionToPlayer * 1.2f + Vector3.up * 0.5f;


        if (!alreadyAttacked) {
            //Attack code
            // Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            Rigidbody rb = Instantiate(projectile, spawnPosition, spawnRotation).GetComponent<Rigidbody>();

            Vector3 shootForce = directionToPlayer * 32f + Vector3.up * 4f;
            rb.AddForce(shootForce, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack() {
        alreadyAttacked = false;
    }

    private void TakeDamage(int damage) {
        health -= damage;
        if (health <= 0) Invoke(nameof(DestroyEnemy), .5f);

    }

    private void DestroyEnemy() {
        Destroy(gameObject);
    }


    //DELETE AFTER - SIGHT RANGE
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

    }
}
