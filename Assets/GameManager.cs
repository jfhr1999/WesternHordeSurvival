using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Entity Player;

    public Entity PlayerEntity { get { return Player; } set { Player = value; } }
    void Start()
    {
        GeneralManager.GameManager = this;
        //ToDo: Check and instantiate a player character
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
