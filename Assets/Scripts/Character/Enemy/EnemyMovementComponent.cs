using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovementComponent : MonoBehaviour
{
    private NavMeshAgent agent;
    private float moveSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(float speed)
    {
        moveSpeed = speed;
        agent.speed = moveSpeed;
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        if (agent.isOnNavMesh) // Always check if agent is on NavMesh before setting destination
        {
            agent.SetDestination(targetPosition);
        }
        else
        {
            Debug.LogWarning("NavMeshAgent is not on a NavMesh. Cannot move.", this);
        }
    }

    public void StopMovement()
    {
        if (agent.isOnNavMesh && agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    // Add more movement behaviors like patrolling, fleeing etc.
}
