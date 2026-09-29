using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [Header("移動,回転設定")]
    Vector2 inputVer;           //入力方向
    float curentNearDistance;   //現在の近い距離
    GameObject curentTarget;    //現在のターゲット
    GameObject nearPlayer;      //近くのPlayer  
    GameObject previousPlayer;  //前回のPlayer


    public Vector2 InputVer() => inputVer;  //入力値を参照

    [Header("攻撃設定")]
    [SerializeField] private BoxCollider atackCollider; //攻撃判定
    bool isAttack = false;
    bool wasKnockBack = false;    //ノックバック中フラグ


    [Header("地面判定設定")]
    [SerializeField]private LayerMask groundLayer;
    private float rayDistance = 2;        //Rayの長さ



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

        //Rayの当たり判定フラグ
        bool hasGround = Physics.Raycast(ray, out RaycastHit hit, rayDistance, groundLayer);

        //地面がない時
        if (!hasGround)
        {
            OnMove(Vector2.zero);

            curentTarget = null;
            nearPlayer = null;

            atackCollider.enabled = false;

            NearPlayer();
            curentTarget = nearPlayer;

            if (curentTarget != null)
            {
                var dir = curentTarget.transform.position - transform.position;
                dir.y = 0;

                var normalize = dir.normalized;
                inputVer = new Vector2(normalize.x, normalize.z);

                // Player方向へ移動
                move.SetMoveInput(inputVer);
            }
            else
            {
                OnMove(Vector2.zero);
            }

            return;
        }
        //ノックバック時移動拒否
        if (state.state == State.KnockBack)
        {
            wasKnockBack = true;
            OnMove(Vector2.zero);
            return;
        }
        //攻撃受けた後のターゲット変更
        if (wasKnockBack)
        {
            wasKnockBack = false;
            curentTarget = knock.Target();

            if (curentTarget != null)
            {
                isAttack = true;
            }
        }
        //近いPlayerに移動
        Debug.Log(state.attackState);
       
        if (state.attackState == AttackState.None || state.attackState == AttackState.Cooldown && !isAttack)
        {
            NearPlayer();
            curentTarget = nearPlayer;
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
            if (state.state == State.None &&
               (state.attackState == AttackState.None ||
                state.attackState == AttackState.Cooldown ||
                state.attackState == AttackState.Charge))
            {
                //入力の更新
                move.SetMoveInput(inputVer);
            }
            if (state.attackState == AttackState.None)
            {
                atack.Attack(AttackState.Charge);
                atackCollider.enabled = true;
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

            OnMove(Vector2.zero);
            atack.Attack(AttackState.Atatck);

            previousPlayer = curentTarget;
            nearPlayer = null;
            curentTarget = null;
            isAttack = false;
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
    /// 入力値などの変更
    /// </summary>
    /// <param name="context"></param>
    void OnMove(Vector2 context)
    {
        state.UpdateMoveState(context);
        move.SetMoveInput(context);
        inputVer = context;
    }

    /*
     *-----BOTの行動パターン-----
     * 1:近くの敵を探索
     * 2:ターゲットの方向に移動
     * 3:距離によって攻撃開始
     * 4:攻撃後待機させ再度探索
     */
}
