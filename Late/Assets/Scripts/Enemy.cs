using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Stuck detection
    private Vector3 lastPosition;
    private float timeNotMoving = 0f;
    private float stuckThreshold = 2f; // seconds
    private float minMoveDistance = 0.05f; // meters
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 7.0f;
    [Tooltip("Enable extra debug logs for the enemy (patrol/search).")]
    public bool debugLogs = false;
    [Tooltip("How many consecutive path failures before choosing a new walk point.")]
    public int maxPathFailures = 2;
    // runtime counter
    private int pathFailureCount = 0;

    public void Awake()
    {
        var playerObj = GameObject.Find("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else if (debugLogs)
            Debug.LogWarning("[Enemy] Could not find object named 'Player' in scene.");

        agent = GetComponent<NavMeshAgent>();
        if (agent == null && debugLogs)
            Debug.LogWarning("[Enemy] No NavMeshAgent found on enemy.");

        lastPosition = transform.position;
    }
    public void Update()
    {
        if (agent == null) return; // nothing to do without agent

        // Ensure we have a player reference (in case player is spawned later)
        if (player == null)
        {
            var playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (!playerInSightRange && !playerInAttackRange)
            Patroling();
        else if (playerInSightRange && !playerInAttackRange)
            ChasePlayer();
        else if (playerInAttackRange && playerInSightRange)
            AttackPlayer();
    }

    private void SearchWalkPoint()
    {
        // Try to find a valid point on the NavMesh within walkPointRange
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 2.0f, NavMesh.AllAreas))
            {
                walkPoint = hit.position;
                // Optionally check ground with raycast if whatIsGround is used
                walkPointSet = true;
                return;
            }
        }
        // If we fail to find a navmesh point, mark unset and try again next frame
        walkPointSet = false;
    }

    private void Patroling()
    {
        agent.speed = patrolSpeed;
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            if (agent.destination != walkPoint)
                agent.SetDestination(walkPoint);

            // If the agent has a non-complete or invalid path (and not pending), try rerouting after retries
            if (!agent.pathPending)
            {
                if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    pathFailureCount++;
                    if (debugLogs) Debug.Log($"[Enemy] Path status {agent.pathStatus} (failure #{pathFailureCount})");
                    if (pathFailureCount >= maxPathFailures)
                    {
                        // try a new point
                        agent.ResetPath();
                        walkPointSet = false;
                        SearchWalkPoint();
                        pathFailureCount = 0;
                        if (debugLogs) Debug.Log("[Enemy] Rerouting to a new walk point after path failures");
                    }
                }
                else
                {
                    // path is good
                    pathFailureCount = 0;
                }
            }

            // Stuck detection
            float movedDistance = (transform.position - lastPosition).magnitude;
            if (movedDistance < minMoveDistance)
            {
                timeNotMoving += Time.deltaTime;
                if (timeNotMoving >= stuckThreshold)
                {
                    // try a new walk point
                    walkPointSet = false;
                    SearchWalkPoint();
                    timeNotMoving = 0f;
                    if (debugLogs) Debug.Log("[Enemy] Stuck - searching new walk point");
                }
            }
            else
            {
                timeNotMoving = 0f;
            }
            lastPosition = transform.position;

            // Use agent.remainingDistance when available to detect arrival
            if (!agent.pathPending && agent.remainingDistance <= Mathf.Max(1f, agent.stoppingDistance))
            {
                walkPointSet = false;
                timeNotMoving = 0f;
                if (debugLogs) Debug.Log("[Enemy] Reached walk point");
            }
        }
    }
    private void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }
    private void AttackPlayer()
    {
        // Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            ///Attack code here

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}