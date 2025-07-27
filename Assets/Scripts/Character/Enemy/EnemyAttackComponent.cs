using UnityEngine;

public class EnemyAttackComponent : MonoBehaviour
{
    private float attackDamage;
    private float attackRange; // This will primarily be used by the EnemyController for decision making
    private float attackCooldown;
    private float lastAttackTime;
    private DamageType Type;


    [SerializeField]
    private DamageDealer DamageDealer;


    public void Initialize(float damage, float range, float cooldown, DamageType type)
    {
        attackDamage = damage;
        attackRange = range;
        attackCooldown = cooldown;
        lastAttackTime = -cooldown; // Allows immediate first attack
        Type = type;
        DamageDealer.Initialize(damage, type);

    }

    public void TryAttack(GameObject target)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack(target);
            lastAttackTime = Time.time;
        }
    }

    private void PerformAttack(GameObject target)
    {
        

        Debug.Log($"{gameObject.name} attacks {target.name} for {attackDamage} damage!");
        
        //To Do: handle cases for attacks
     
        
        // Play attack animation, sound, visual effects here
        // For example: GetComponent<AudioSource>()?.PlayOneShot(attackSound);
    }
}
