using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private HealthComponent Health;
    private void Awake()
    {
        Health.Initialize(Health.MaxHealth);
    }
}
