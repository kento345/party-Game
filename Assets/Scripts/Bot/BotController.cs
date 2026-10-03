using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    [Header("移動,回転設定")]
    Vector2 inputVer;           //入力方向
    float curentNearDistance;   //現在の近い距離
    GameObject nearPlayer;      //近くのPlayer  
    GameObject previousPlayer;  //前回のPlayer


    public Vector2 InputVer => inputVer;  //入力値を参照

    [Header("攻撃設定")]
    

    [Header("判定設定")]
    [SerializeField]private LayerMask playerLayaer;
    [SerializeField] private LayerMask objLayer;
    private float rayDistance = 2;        //Rayの長さ

    //-----Component-----
    private NavMeshAgent agent;



    //-----Script-----
    private StateManager state;
    private MoveControlleer move;
    private AtackController atack;


    void Start()
    {
        state = GetComponent<StateManager>();
        move  = GetComponent<MoveControlleer>();
        atack = GetComponent<AtackController>();

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //近いPlayerに移動       
        if (state.attackState == AttackState.None || state.attackState == AttackState.Cooldown)
        {
            NearPlayer();
            if(nearPlayer != null)
            {
                agent.destination = nearPlayer.transform.position;
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    OnMove(Vector2.zero);
                }
                else
                {
                    Vector3 dir = (nearPlayer.transform.position - transform.position).normalized;
                    OnMove(new Vector2(dir.x, dir.z));
                }
            }
        }
    }

    /// <summary>
    /// 攻撃開始判定処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    { 
        if(other.gameObject == nearPlayer)
        {
            OnMove(Vector2.zero);
            atack.Attack(AttackState.Atatck);

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
     * 4:Objectに当たりそうなら回避行動
     * 5:
     */
}
