using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private HealthComponent _targetHealth;
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

