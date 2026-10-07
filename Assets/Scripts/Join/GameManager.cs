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

    private List<HitController> lifeList = new();
    int count = 0;

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

        foreach (var player in players)
        {
            player.Key.transform.position = pos[player.Value - 1].position;
            player.Key.transform.rotation = pos[player.Value - 1].rotation;
            lifeList.Add(player.Key.GetComponent<HitController>());
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
            lifeList.Add(bot.GetComponent<HitController>());
            i++;
        }
    }

    private void Update()
    {
        foreach(var l in lifeList)
        {
            if (l.IsAlive)
            {
                count += 1;
            }
            else
            {
                count -= 1;
            }

            if(count == 1)
            {
                Debug.Log("Wind");
            }
        }
    }

    public void OnReset()
    {
        Destroy(joinObj);
        SceneManager.LoadScene("");
    }
}
