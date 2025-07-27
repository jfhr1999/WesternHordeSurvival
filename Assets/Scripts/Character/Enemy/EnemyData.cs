using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Scriptable Objects/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Base Stats")]
    public float maxHealth = 100f; // This can be overridden by your health component if it's external
    public float movementSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public DamageType damageType = DamageType.Physical;

    [Header("Visuals/Audio (Optional)")]
    public GameObject enemyModelPrefab; // For unique visual representations
    public AudioClip attackSound;
    // Add more properties as needed, e.g., animations, specific behaviors
}
