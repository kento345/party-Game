using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> players;

    //参照用のプロパティを作成
    public List<GameObject> playerList => players;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //インスタンスに自身を取得
        Instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
