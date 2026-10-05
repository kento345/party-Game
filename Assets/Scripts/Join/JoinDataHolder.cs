using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinDataHolder : MonoBehaviour
{
    public static JoinDataHolder instance { get; private set;}
    private Dictionary<GameObject, int> playerData= new ();
    //参照専用
    public IReadOnlyDictionary<GameObject, int> GetPlayerData => playerData;


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
        playerData.Add(player, id);
    }
}
