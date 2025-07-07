using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct DamageInfo
{
    public float Amount;
    public DamageType Type;
    public GameObject Source; //Change to Entity?
    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public bool IsCriticalHit;

    public DamageInfo(float amount, DamageType type, GameObject source = null, Vector3 hitPoint = default, Vector3 hitNormal = default, bool isCritical = false)
    {
        Amount = amount;
        Type = type;
        Source = source;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
        IsCriticalHit = isCritical;
    }
}

public enum DamageType
{
    Physical,
    TrueDamage

}


public class HealthComponent : MonoBehaviour, IDamageable
{

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _currentHealth;

    [Header("Resistances")]
    // A simple array for resistances. You could use a Dictionary<DamageType, float>
    // but arrays are easier to manage in the Inspector for fixed enums.
    [SerializeField] private float[] _damageTypeResistances = new float[1]; //ToDo: needed? Maybe

    // C# Events for internal/code-based subscriptions (highly recommended for decoupling)
    public event System.Action<float, float> OnHealthChanged; // Current Health, Max Health
    public event System.Action<DamageInfo> OnDamageTaken;
    public event System.Action OnDeath;
    public event System.Action OnRevived;

    // Unity Events for Inspector-based subscriptions (for designers/quick prototyping)
    [Header("Unity Events (for Inspector)")]
    public UnityEvent<float, float> OnHealthChangedUnityEvent;
    public UnityEvent<DamageInfo> OnDamageTakenUnityEvent;
    public UnityEvent OnDeathUnityEvent;
    public UnityEvent OnRevivedUnityEvent;


    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;
    public bool IsAlive => _currentHealth > 0;

    public void Initialize()
    {
        _currentHealth = _maxHealth;
        // Initialize resistances to default (no resistance/vulnerability)
        for (int i = 0; i < _damageTypeResistances.Length; i++)
        {
            _damageTypeResistances[i] = 1.0f; // 1.0 means 100% damage taken
        }

        // Example: Set specific resistances in Awake or a separate setup method
        // SetResistance(DamageType.Fire, 0.5f); // Takes 50% fire damage
        // SetResistance(DamageType.Ice, 1.5f);  // Takes 150% ice damage (vulnerability)
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        if (!IsAlive) return; // Cannot take damage if already dead

        float effectiveDamage = damageInfo.Amount;

        // Apply resistance/vulnerability based on damage type
        if (damageInfo.Type != DamageType.TrueDamage) // True damage bypasses resistances
        {
            int typeIndex = (int)damageInfo.Type;
            if (typeIndex >= 0 && typeIndex < _damageTypeResistances.Length)
            {
                effectiveDamage *= _damageTypeResistances[typeIndex];
            }
            else
            {
                Debug.LogWarning($"DamageType {damageInfo.Type} is not configured in resistances array.");
            }
        }

        // Basic critical hit modification (you might have more complex crit formulas)
        if (damageInfo.IsCriticalHit)
        {
            effectiveDamage *= 2.0f; // Double damage for critical hits
            Debug.Log($"Critical Hit! Effective Damage: {effectiveDamage}");
        }

        _currentHealth -= effectiveDamage;
        _currentHealth = Mathf.Max(_currentHealth, 0f); // Ensure health doesn't go below zero

        // Notify subscribers
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnDamageTaken?.Invoke(damageInfo);
        OnHealthChangedUnityEvent?.Invoke(_currentHealth, _maxHealth);
        OnDamageTakenUnityEvent?.Invoke(damageInfo);

        Debug.Log($"{gameObject.name} took {effectiveDamage:F2} {damageInfo.Type} damage from {damageInfo.Source?.name ?? "Unknown Source"}. Current Health: {_currentHealth:F2}");

        if (_currentHealth <= 0 && IsAlive) // Check IsAlive to avoid multiple death calls
        {
            Die();
        }
    }

    /// <summary>
    /// Heals the entity for a specified amount.
    /// </summary>
    /// <param name="amount">The amount of health to restore.</param>
    public void Heal(float amount)
    {
        if (!IsAlive)
        {
            Debug.LogWarning($"{gameObject.name} tried to heal but is dead.");
            return; // Can't heal if dead, unless it's a revive mechanic
        }

        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth); // Cap health at max

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnHealthChangedUnityEvent?.Invoke(_currentHealth, _maxHealth);
        Debug.Log($"{gameObject.name} healed for {amount}. Current Health: {_currentHealth:F2}");
    }

    /// <summary>
    /// Sets a specific resistance multiplier for a damage type.
    /// A value of 1.0f means 100% damage.
    /// A value of 0.5f means 50% damage (resistance).
    /// A value of 1.5f means 150% damage (vulnerability).
    /// </summary>
    /// <param name="type">The DamageType to set resistance for.</param>
    /// <param name="multiplier">The resistance multiplier.</param>
    public void SetResistance(DamageType type, float multiplier)
    {
        int typeIndex = (int)type;
        if (typeIndex >= 0 && typeIndex < _damageTypeResistances.Length)
        {
            _damageTypeResistances[typeIndex] = multiplier;
        }
        else
        {
            Debug.LogError($"Invalid DamageType {type} for resistance setting.");
        }
    }

    /// <summary>
    /// Handles the entity's death.
    /// </summary>
    private void Die()
    {
        _currentHealth = 0; // Ensure health is exactly 0 upon death
        Debug.Log($"{gameObject.name} has died!");

        OnDeath?.Invoke();
        OnDeathUnityEvent?.Invoke();

        // --- Add your death logic here ---
        // For example:
        // GetComponent<Collider>().enabled = false;
        // GetComponent<Rigidbody>().isKinematic = true;
        // Play death animation
        // Disable AI
        // Destroy(gameObject, 5f); // Destroy after 5 seconds
        // Inform a Game Manager
    }

    /// <summary>
    /// Revives the entity and restores health.
    /// </summary>
    /// <param name="healthPercentage">Percentage of max health to restore upon revive (0-1).</param>
    public void Revive(float healthPercentage = 0.5f)
    {
        if (IsAlive)
        {
            Debug.LogWarning($"{gameObject.name} is already alive, cannot revive.");
            return;
        }

        _currentHealth = _maxHealth * Mathf.Clamp01(healthPercentage);
        // Re-enable components if they were disabled on death
        // GetComponent<Collider>().enabled = true;
        // GetComponent<Rigidbody>().isKinematic = false;

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        OnHealthChangedUnityEvent?.Invoke(_currentHealth, _maxHealth);
        OnRevived?.Invoke();
        OnRevivedUnityEvent?.Invoke();

        Debug.Log($"{gameObject.name} has been revived with {_currentHealth:F2} health!");
    }

    // You could also add methods for:
    // - AddStatusEffect(StatusEffect effect)
    // - RemoveStatusEffect(StatusEffect effect)
    // - GetDamageReductionFromArmor()
    // - IsImmuneTo(DamageType type)
}
