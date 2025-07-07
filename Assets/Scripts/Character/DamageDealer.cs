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
        Initialize();
    }

    public void Initialize(GameObject source = null)
    {
        if (_damageSource == null && source == null)
        {
            _damageSource = this.gameObject; // Default to self if not set
        }
        else
        {
            _damageSource = source;
        }
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

    /// <summary>
    /// Tries to apply damage to a target GameObject.
    /// </summary>
    /// <param name="target">The GameObject to attempt to damage.</param>
    /// <param name="hitPoint">The point of impact.</param>
    /// <param name="hitNormal">The normal of the impact surface.</param>
    public void ApplyDamageToTarget(GameObject target, Vector3 hitPoint, Vector3 hitNormal)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
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



/*
 *  ToDo
 * 
 * // HealthUI.cs
using UnityEngine;
using UnityEngine.UI; // For Image (fill bar) or Text

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health _targetHealth;
    [SerializeField] private Image _healthFillImage; // Assign a UI Image (Type: Filled)
    [SerializeField] private Text _healthText;       // Assign a UI Text element

    void OnEnable()
    {
        if (_targetHealth != null)
        {
            // Subscribe to the C# events
            _targetHealth.OnHealthChanged += UpdateHealthDisplay;
            _targetHealth.OnDeath += OnTargetDeath;
            _targetHealth.OnRevived += OnTargetRevived;

            // Initial update
            UpdateHealthDisplay(_targetHealth.CurrentHealth, _targetHealth.MaxHealth);
        }
    }

    void OnDisable()
    {
        if (_targetHealth != null)
        {
            // Unsubscribe to prevent memory leaks
            _targetHealth.OnHealthChanged -= UpdateHealthDisplay;
            _targetHealth.OnDeath -= OnTargetDeath;
            _targetHealth.OnRevived -= OnTargetRevived;
        }
    }

    private void UpdateHealthDisplay(float currentHealth, float maxHealth)
    {
        if (_healthFillImage != null)
        {
            _healthFillImage.fillAmount = currentHealth / maxHealth;
        }
        if (_healthText != null)
        {
            _healthText.text = $"{currentHealth:F0} / {maxHealth:F0}"; // Format to 0 decimal places
        }
    }

    private void OnTargetDeath()
    {
        Debug.Log($"UI: {_targetHealth.gameObject.name} has died! Hiding UI.");
        // Optionally disable UI or show 'Dead' text
        gameObject.SetActive(false);
    }

    private void OnTargetRevived()
    {
        Debug.Log($"UI: {_targetHealth.gameObject.name} has been revived! Showing UI.");
        gameObject.SetActive(true); // Re-enable UI
    }
}
 */