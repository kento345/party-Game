using Unity.VisualScripting;
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

    float rota = 0;
    float angle = 0;
    bool isRota = false;

    public Vector2 InputVer => inputVer;  //入力値を参照

    [Header("攻撃設定")]
    private bool wasAttack = false;
    private Vector3 moveTarget;
    private bool hasMove = false;

    [Header("判定")]
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask playerLayer;
    private float direction = 1.5f;
    Ray ray;
    RaycastHit hit;


    //-----Component-----


    //-----Script-----
    private StateManager state;
    private AtackController atack;
    MoveControlleer move;


    void Start()
    {
        state = GetComponent<StateManager>();
        atack = GetComponent<AtackController>();
        move  = GetComponent<MoveControlleer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state.attackState == AttackState.Atatck) { return; }
        if (wasAttack)
        {
            if (!hasMove)
            {
                moveTarget = new Vector3(Random.Range(-8, 8), 0.5f, Random.Range(-8, 8));
                hasMove = true;
            }
            Chase(moveTarget);
            var dist = Vector3.Distance(transform.position, moveTarget);
            if (dist < 2f)
            {
                wasAttack = false;
                hasMove = false;
                nearPlayer = null;
                return;
            }
        }
        if (CheckObj() && !isRota)
        {
            isRota = true;
            //0->-1,1->1になる
            rota = Random.Range(0, 2) == 0 ? -1 : 1;
            angle = 0f;
        }
        if (isRota)
        {
            OnMove(new Vector2(rota, 0));
            angle += move.GetRotaSpeed * Time.deltaTime;

            if(angle >= 90)
            {
                isRota = false;
                rota = 0f;
                OnMove(Vector2.zero);
            }
            return;
        }
        //敵を探す
        if (nearPlayer == null)
        {
            NearPlayer();
            return;
        }
        //ターゲットがいる
        else
        {
            Chase(nearPlayer.transform.position);

            var dist = Vector3.Distance(transform.position, nearPlayer.transform.position);
            if (dist < 7f)
            {
                atack.Attack(AttackState.Charge);
                var dir = nearPlayer.transform.position - transform.position;
                dir.y = 0;
                var localDir = transform.InverseTransformDirection(dir.normalized);
                if (dist < 4f)
                {
                    OnMove(new Vector2(localDir.x,0));
                    atack.Attack(AttackState.Atatck);
                    previousPlayer = nearPlayer;
                    nearPlayer = null;
                    wasAttack = true;
                }
            }
        }

       
    }

    /// <summary>
    /// Rayを作りObjの判定
    /// </summary>
    /// <returns></returns>
    bool CheckObj()
    {
        var origin = transform.position + new Vector3(0, 0.5f, 0);
        var dir = transform.forward;
        ray = new Ray(origin, dir);
        Debug.DrawRay(origin, dir * direction, Color.red);

        return Physics.Raycast(ray, out hit, direction, objectLayer);
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
        foreach (var p in JoinDataHolder.instance.GetPlayerData.Keys)
        {
            //自身は除外
            if (p == gameObject || p == previousPlayer || p == null) continue;
            // 死亡しているなら除外
            if (!p.transform.GetChild(0).gameObject.activeSelf)
                continue;
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
    /// 追跡処理
    /// </summary>
    void Chase(Vector3 targetPosition)
    {
        var dir = targetPosition - transform.position;
        dir.y = 0f;
        var localDir = transform.InverseTransformDirection(dir.normalized);
        inputVer = new Vector2 (localDir.x, localDir.z);
        OnMove(inputVer);
    }

    /// <summary>
    /// 入力値などの変更
    /// </summary>
    /// <param name="context"></param>
    void OnMove(Vector2 context)
    {
        move.SetMoveInput(context);
        inputVer = context;
    }

    public void OnMoveStop(bool x)
    {
        move.enabled = x;
        atack.enabled = x;
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
