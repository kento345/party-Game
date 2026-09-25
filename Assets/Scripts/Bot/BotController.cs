using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BotController : MonoBehaviour
{
    [Header("移動,回転設定")]
    Vector2 inputVer;           //入力方向
    float curentNearDistance;   //現在の近い距離
    GameObject nearPlayer;      //近くのPlayer  
    GameObject previousPlayer;  //前回のPlayer


    [Header("攻撃設定")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider atackCollider; //攻撃判定
    bool isCharging = false;    //チャージ状態

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
         //-----移動-----
         //初期化
         var origin = transform.position + transform.forward * 1f + Vector3.up;
         //Rayの作成
         var ray = new Ray(origin, Vector3.down);
         Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0, false);

         //Rayの当たり判定
         if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
         {
             //Groundに接触中の判定

             if (move.IsRotating())
             {
                 move.SetMoveInput(Vector2.zero);
                 return;
             }
            //近いPlayerに移動
            if (state.attackState != AttackState.Atatck && state.attackState != AttackState.Cooldown && !isCharging)
            {
                NearPlayer();
            }


            //チャージ開始
            if (nearPlayer != null)
             {
                //ターゲットの方向を取得して正規化
                 var dir = nearPlayer.transform.position - transform.position;
                 dir.y = 0;
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
                if (!isCharging)
                {
                    isCharging = true;
                }
                if (isCharging)
                {
                    atack.Attack(AttackState.Charge);

                    atackCollider.enabled = true;
                }
             }
             if (state.state == State.None && (state.attackState == AttackState.None || state.attackState == AttackState.Cooldown || state.attackState == AttackState.Charge))
             {
                 //入力の更新
                 move.SetMoveInput(inputVer);
             }
             //--------------
         }
    }

    /// <summary>
    /// 近接プレイヤーがトリガーに入った際に移動を停止して攻撃を開始し、攻撃コライダーを無効化する。
    /// </summary>
    /// <remarks>攻撃コライダーを無効化し、move.SetMoveInput(Vector2.zero) で移動入力を停止、atack.Attack(AttackState.Atatck)
    /// で攻撃を開始する。isCharging を false に設定し、previousPlayer を更新して nearPlayer をクリアする。</remarks>
    /// <param name="other">トリガーに入ったコライダー。近接プレイヤーとの照合に使用される。</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == nearPlayer)
        {
            atackCollider.enabled = false;

            move.SetMoveInput(Vector2.zero);
            atack.Attack(AttackState.Atatck);

            isCharging = false;
            previousPlayer = nearPlayer;
            nearPlayer = null;
        }
    }

    /// <summary>
    /// 最も近いプレイヤーを検索し、nearPlayer と curentNearDistance を更新する。
    /// </summary>
    /// <remarks>自身および previousPlayer を除外し、GameManager.Instance.playerList の各プレイヤーとの距離を Vector3.Distance
    /// で比較する。最短距離が見つかれば nearPlayer に割り当て、curentNearDistance を更新する。処理開始時に curentNearDistance は Mathf.Infinity
    /// で初期化される。</remarks>
    void NearPlayer()
    {
        //if(GameManager.Instance.playerList == null)return;
        //初期化
        curentNearDistance = Mathf.Infinity;
        nearPlayer = null;

        //PlayerListの中で一番近いPlayerを検索,取得
        foreach (var p in GameManager.Instance.playerList)
        {
            //自身は除外
            if (p == gameObject) continue;
            if(p == previousPlayer) continue;

            //2点間の距離計算
            var dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < curentNearDistance)
            {
                curentNearDistance = dist;
                nearPlayer = p;
            }
        }
    }

    /// <summary>
    /// 入力値を参照
    /// </summary>
    /// <returns></returns>
    public Vector2 InputVer()
    {
        return inputVer;
    }
/*
 *-----BOTの行動パターン-----
 * 1:近くの敵を探索
 * 2:ターゲットの方向に移動
 * 3:距離によって攻撃開始
 * 4:攻撃後待機させ再度探索
 */
}
