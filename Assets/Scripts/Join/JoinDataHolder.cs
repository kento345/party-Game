using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinDataHolder : MonoBehaviour
{
    public static JoinDataHolder instance { get; private set;}
    private Dictionary<InputDevice, int> playerDevice = new ();
    public Dictionary<InputDevice, int> GetPlayerData => playerDevice;

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

    public void SetPlayerData(InputDevice device,int id)
    {
        playerDevice.Add(device, id);
    }
}
