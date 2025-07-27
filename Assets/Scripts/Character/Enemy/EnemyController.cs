using UnityEngine;

[RequireComponent(typeof(HealthComponent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData; // Assign your specific EnemyData Scriptable Object here
    private HealthComponent healthComponent; // Reference to your existing Health component
    private EnemyMovementComponent enemyMovement; // Reference to the movement component
    private EnemyAttackComponent enemyAttack;   // Reference to the attack component


    public EnemyData GetEnemyData() => enemyData;

    void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        healthComponent = GetComponent<HealthComponent>();
        enemyMovement = GetComponent<EnemyMovementComponent>(); // Get references to modular components
        enemyAttack = GetComponent<EnemyAttackComponent>();

        if (healthComponent != null && enemyData != null)
        {
            healthComponent.Initialize(enemyData.maxHealth); // Initialize health from EnemyData
        }

        if (enemyMovement != null && enemyData != null)
        {
            enemyMovement.Initialize(enemyData.movementSpeed); // Initialize movement speed
        }

        if (enemyAttack != null && enemyData != null)
        {
            enemyAttack.Initialize(enemyData.attackDamage, enemyData.attackRange, enemyData.attackCooldown, enemyData.damageType); // Initialize attack
        }
    }

    // Example of how enemy controller might coordinate
    void Update()
    {
        // Example: If an enemy needs to move towards a target
        // For demonstration, let's say target is always player (needs to be found/assigned)
        Transform target = FindFirstObjectByType<FirstPersonController>()?.transform; // Replace with actual target finding
        if (target != null)
        {
            enemyMovement?.MoveToTarget(target.position);

            // Example: Attack logic To Do, check if is ok
            if (Vector3.Distance(transform.position, target.position) <= enemyData.attackRange)
            {
                enemyAttack?.TryAttack(target.gameObject);
            }
        }
    }
}
