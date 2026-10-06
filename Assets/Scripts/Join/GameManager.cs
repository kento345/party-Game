using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [SerializeField] private GameObject botPreefab = default;
    [SerializeField] private Transform[] pos = default;
    private GameObject joinObj;

/*    private List<GameObject> playerList = new();

    public List<GameObject> GetPlayerList => playerList;*/

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (JoinDataHolder.instance == null) { return; }
        //DontDestoryObjectを取得
        joinObj = JoinDataHolder.instance.gameObject;
        
        //インスタンスで保持しているPlayer情報を取得
        var players = JoinDataHolder.instance.GetPlayerData;
        //playerList = new List<GameObject>(players.Keys);

        foreach (var player in players)
        {
            player.Key.transform.position = pos[player.Value - 1].position;
            player.Key.transform.rotation = pos[player.Value - 1].rotation;
            foreach (var child in player.Key.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.SetActive(true);
            }
        }
        int i = players.Count;
        while(i < pos.Length)
        {
            var bot = Instantiate(botPreefab, pos[i].position, pos[i].rotation);
            JoinDataHolder.instance.SetPlayerData(bot, i + 1);
            i++;
        }
    }

    public void OnReset()
    {
        Destroy(joinObj);
        SceneManager.LoadScene("");
    }
}
