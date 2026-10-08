using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [SerializeField] private GameObject botPreefab = default;
    [SerializeField] private Transform[] pos = default;
    private GameObject joinObj;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;

    private Dictionary<GameObject,int> ActivePlayers = new();
    private List<GameObject> playerList = new();
    private List<GameObject> botList = new();

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
            botList.Add(bot);
            i++;
        }

        ActivePlayers = new Dictionary<GameObject,int>(JoinDataHolder.instance.GetPlayerData);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        List<GameObject> deadPlayers = new();
        Debug.Log(Time.timeScale);
        foreach (var p in ActivePlayers)
        {
            var life = p.Key.GetComponent<HitController>();

            if (life != null && !life.IsAlive)
            {
                deadPlayers.Add(p.Key);
            }
        }

        foreach (var player in deadPlayers)
        {
            ActivePlayers.Remove(player);
        }

        if (ActivePlayers.Count == 1)
        {
            foreach (var p in ActivePlayers)
            {
                resultText.text = $"Win: {p.Key.name.Replace("(Clone)", "")}{p.Value}";
                resultPanel.SetActive(true);
                Time.timeScale = 0.0f;
            }
        }
    }

    public void OnReset()
    {
        var currentScene = SceneManager.GetActiveScene().name;
        if (JoinDataHolder.instance != null)
        {
            foreach (var bot in botList)
            {
                JoinDataHolder.instance.RemoveBotData(bot);
            }
        }
        SceneManager.LoadScene(currentScene);
    }

    public void OnTitle()
    {
        foreach(var p in JoinDataHolder.instance.GetPlayerData)
        {
            Destroy(p.Key);
        }
        Destroy(joinObj);
        SceneManager.LoadScene("JoinScene");
    }
}
