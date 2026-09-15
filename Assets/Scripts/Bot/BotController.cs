using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BotController : MonoBehaviour
{
    [Header("移動,回転設定")]
    Vector2 inputVer;
    float curentNearDistance;
    GameObject nearPlayer;
    [Header("攻撃設定")]

    int atackDis;
    bool isCharging = false;

    [Header("地面判定設定")]
    [SerializeField]private LayerMask groundLayer;
    private float rayDistance = 2;

    //Script
    private StateManager state;
    private MoveControlleer move;
    private AtackController atack;



    void Start()
    {
        state = GetComponent<StateManager>();
        move = GetComponent<MoveControlleer>();
        atack = GetComponent<AtackController>();
    }

    // Update is called once per frame
    void Update()
    {
        //初期化
        var origin = transform.position + transform.forward * 1f + Vector3.up;

        //Rayの作成
        var ray = new Ray(origin, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance,Color.red,0, false);

        //Rayの当たり判定
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            //Groundに接触中の判定
            if(((1 << hit.collider.gameObject.layer) & groundLayer) != 0) {
                //近いPlayerに移動
                NearPlayer();
                if (nearPlayer != null)
                {
                    //ターゲットの方向を取得して正規化
                    var dir = nearPlayer.transform.position - transform.position;
                    var nomalize = dir.normalized;
                    //正規化した方向をVector2に変換
                    inputVer = new Vector2(nomalize.x, nomalize.z);

                    //KnockBack状態になった場合は移動を止める
                    if (state.state == State.KnockBack)
                    {
                        move.SetMoveInput(Vector2.zero);
                        state.UpdateMoveState(Vector2.zero);
                        return;
                    }
                    // 距離が10未満になったらチャージ開始
                    if (dir.magnitude < 5f && !isCharging)
                    {
                        isCharging = true;

                        atack.Attack(AttackState.Charge);
                        Debug.Log("Charge");
                        atackDis = Random.Range(2, 5);
                    }

                    // 距離が3未満になったら攻撃
                    if (dir.magnitude < atackDis && isCharging)
                    {
                        atack.Attack(AttackState.Atatck);
                        Debug.Log("Attack");

                        isCharging = false;
                        nearPlayer = null;
                    }
                }
                if (state.state == State.None && (state.attackState == AttackState.None || state.attackState == AttackState.Cooldown))
                {
                    //入力の更新
                    move.SetMoveInput(inputVer);
                }
            }
        }
        //Rayに何も接触していなかったとき
        else
        {
            Debug.Log("Not ground!");
        }
    }

    void NearPlayer()
    {
        //初期化
        curentNearDistance = Mathf.Infinity;
        nearPlayer = null;

        //PlayerListの中で一番近いPlayerを検索,取得
        foreach (var p in GameManager.Instance.playerList)
        {
            //自身は除外
            if (p == gameObject) continue;

            //2点間の距離計算
            var dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < curentNearDistance)
            {
                curentNearDistance = dist;
                nearPlayer = p;
            }
        }
    }

    public Vector2 InputVer()
    {
        return inputVer;
    }
}
