using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab = default;
    [SerializeField] private GameObject botPreefab = default;
    [SerializeField] private Transform[] pos = default;

    private GameObject joinObj;

    [SerializeField]
    private List<GameObject> players;

    //参照用のプロパティを作成
    public List<GameObject> playerList => players;

    void Awake()
    {
        if(JoinDataHolder.instance == null) { return; }
        joinObj = JoinDataHolder.instance.gameObject;

        //インスタンスで保持しているPlayer情報を取得
        var players = JoinDataHolder.instance.GetPlayerData;

        for(int i = 0; i < players.Count; i++)
        {
            //players[]
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
