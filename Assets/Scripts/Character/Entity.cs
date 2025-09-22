using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private HealthComponent Health;

    [SerializeField]
    private bool isPlayer = false;
    private void Awake()
    {
        Health.Initialize(Health.MaxHealth);
        if (isPlayer)
        {
            Debug.Log("Player on scene");
        }
    }

    public HealthComponent GetHealthComponent()
    {
        return Health;
    }
}
