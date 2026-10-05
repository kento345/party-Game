using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject botPreefab = default;
    [SerializeField] private Transform[] pos = default;

    private GameObject joinObj;

    void Awake()
    {
        if(JoinDataHolder.instance == null) { return; }
        //DontDestoryObjectを取得
        joinObj = JoinDataHolder.instance.gameObject;
        
        //インスタンスで保持しているPlayer情報を取得
        var players = JoinDataHolder.instance.GetPlayerData;

        foreach (var player in players)
        {
            player.Key.transform.position = pos[player.Value - 1].position;
            player.Key.transform.rotation = pos[player.Value - 1].rotation;
        }
        int i = players.Count;
        while(i < pos.Length)
        {
            var bot = Instantiate(botPreefab, pos[i].position, pos[i].rotation);
            i++;
        }
    }

    public void OnReset()
    {
        SceneManager.LoadScene("");
        Destroy(joinObj);
    }
}
