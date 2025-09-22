using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerUIPrefab;
    [SerializeField] private PlayerUIController PlayerUIController;
    void Start()
    {
        GeneralManager.UIManager = this;
        if(PlayerUIPrefab != null)
        {
        }
        
    }

    void Update()
    {
        
    }
}
