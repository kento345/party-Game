using UnityEngine;

public class BotController : MonoBehaviour
{
    [Header("移動,回転設定")]
    float delaiTime = 3f;         //探索ディレイ
    float time = 0f;              //
    bool isDelai = true;          //ディレイフラグ
    bool wasKnockBack = false;    //ノックバック中フラグ
    bool isAttacker = false;      //攻撃を受けたかフラグ
    Vector2 inputVer;           //入力方向
    float curentNearDistance;   //現在の近い距離
    GameObject curentTarget;    //現在のターゲット
    GameObject nearPlayer;      //近くのPlayer  
    GameObject previousPlayer;  //前回のPlayer

    [Header("攻撃設定")]
    [SerializeField] private BoxCollider atackCollider; //攻撃判定

    [Header("地面判定設定")]
    [SerializeField]private LayerMask groundLayer;
    private float rayDistance = 2;

    //Script
    private StateManager state;
    private MoveControlleer move;
    private AtackController atack;
    private knockbackController knock;



    void Start()
    {
        state = GetComponent<StateManager>();
        move = GetComponent<MoveControlleer>();
        atack = GetComponent<AtackController>();
        knock = GetComponent<knockbackController>();
    }

    // Update is called once per frame
    void Update()
    {        
        //初期化
        var origin = transform.position + transform.forward * 1f + Vector3.up;
        //Rayの作成
        var ray = new Ray(origin, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0, false);

        //Rayの当たり判定(Groundに接触中の判定)
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer))
        {
            //回転中は移動しない
            if (move.IsRotating())
            {
                move.SetMoveInput(Vector2.zero);
                state.UpdateMoveState(Vector2.zero);
                return;
            }
            //ノックバック時移動拒否
            if (state.state == State.KnockBack)
            {
                wasKnockBack = true;

                move.SetMoveInput(Vector2.zero);
                state.UpdateMoveState(Vector2.zero);
                return;
            }
            //攻撃受けた後のターゲット変更
            if (wasKnockBack)
            {
                wasKnockBack = false;
                curentTarget = knock.Target();

                if (curentTarget != null)
                {
                    isAttacker = true;
                }
            }
            //近いPlayerに移動
            if (state.attackState == AttackState.None && !isAttacker)
            {
                //3秒待って近くのPlayer探索
                if (time > 0)
                {
                    time -= Time.deltaTime;
                    if (time <= 0)
                    {
                        isDelai = true;
                    }
                }
                if (isDelai)
                {
                    NearPlayer();
                    curentTarget = nearPlayer;
                    isDelai = false;
                }
            }


            //チャージ開始
            if (curentTarget != null)
            {
                //ターゲットの方向を取得して正規化
                var dir = curentTarget.transform.position - transform.position;
                dir.y = 0;
                var nomalize = dir.normalized;
                //正規化した方向をVector2に変換
                inputVer = new Vector2(nomalize.x, nomalize.z);

                atack.Attack(AttackState.Charge);
                atackCollider.enabled = true;
            }
            if (state.state == State.None && (state.attackState == AttackState.None || state.attackState == AttackState.Cooldown || state.attackState == AttackState.Charge))
            {
                //入力の更新
                move.SetMoveInput(inputVer);
            }
        }
    }

    /// <summary>
    /// 攻撃開始判定処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    { 
        if(other.gameObject == curentTarget)
        {
            atackCollider.enabled = false;

            move.SetMoveInput(Vector2.zero);
            atack.Attack(AttackState.Atatck);

            previousPlayer = curentTarget;
            nearPlayer = null;
            curentTarget = null;
            isAttacker = false;

            time = delaiTime;
            isDelai = false;
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
