using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinDataHolder : MonoBehaviour
{
    public static JoinDataHolder instance { get; private set;}
    private Dictionary<GameObject, int> playerDevice = new ();
    public Dictionary<GameObject, int> GetPlayerData => playerDevice;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerData(GameObject player,int id)
    {
        playerDevice.Add(player, id);
    }
}
