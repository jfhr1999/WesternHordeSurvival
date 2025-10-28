using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float _baseDamageAmount = 10f;
    [SerializeField] private DamageType _damageType = DamageType.Physical;
    [SerializeField] private bool _isCriticalHitChance = false; // Simple crit chance example
    [SerializeField] private float _criticalChance = 0.2f; // 20% chance for critical
    [SerializeField] private bool _destroyOnHit = true;

    // Optional: Reference to the GameObject that owns this damage dealer (e.g., the player)
    // This allows the Health component to know who dealt the damage.
    [Header("Source")]
    [SerializeField] private GameObject _damageSource;


    void Start()
    {

    }

    public void Initialize(GameObject source = null)
    {
        if (_damageSource == null && source == null)
        {
            _damageSource = this.gameObject; // Default to self if not set
        }
        else if(_damageSource == null)
        {
            _damageSource = source;
        }
    }

    public void Initialize(float Damage, DamageType type, GameObject source)
    {
        _baseDamageAmount = Damage;
        _damageType = type;
        Initialize(source);
    }

    // Example for a projectile hitting something
    void OnCollisionEnter(Collision collision)
    {
        ApplyDamageToTarget(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal);

        if (_destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    // Example for a trigger (e.g., an AoE spell, a player's attack collider)
    void OnTriggerEnter(Collider other)
    {
        ApplyDamageToTarget(other.gameObject, other.ClosestPoint(transform.position), Vector3.zero); // For triggers, hit normal might be less relevant

        // If this is a one-shot trigger (like an explosion)
        // if (_destroyOnHit)
        // {
        //     Destroy(gameObject);
        // }
    }

    public void ApplyDamageToRaycastHit(RaycastHit hit)
    {
        // The target GameObject is the one hit by the raycast
        GameObject target = hit.transform.gameObject;

        // Extract the hit point and hit normal from the RaycastHit object
        Vector3 hitPoint = hit.point;
        Vector3 hitNormal = hit.normal;

        // Call the existing method to apply damage
        ApplyDamageToTarget(target, hitPoint, hitNormal);
    }

    /// <summary>
    /// Tries to apply damage to a target GameObject.
    /// </summary>
    /// <param name="target">The GameObject to attempt to damage.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="hitNormal">The normal of the impact surface.</param>
    public void ApplyDamageToTarget(GameObject target, Vector3 hitPoint, Vector3 hitNormal)
    {      

        IDamageable damageable = null;
        if (target.transform.parent != null) 
        {
            damageable = target.GetComponentInParent<IDamageable>();
        }
        else
        {
            damageable = target.GetComponent<IDamageable>();
        }

        if (_damageSource != null && target == _damageSource)
        {
            return;
        }
        if (_damageSource != null && target.transform.IsChildOf(_damageSource.transform))
        {
            return;
        }

        if (damageable != null)
        {
            bool isCrit = _isCriticalHitChance && Random.value < _criticalChance;

            DamageInfo damage = new DamageInfo(
                _baseDamageAmount,
                _damageType,
                _damageSource,
                hitPoint,
                hitNormal,
                isCrit
            );
            damageable.TakeDamage(damage);
        }
    }

    // You might want a public method to set the damage source dynamically
    public void SetDamageSource(GameObject source)
    {
        _damageSource = source;
    }
}
