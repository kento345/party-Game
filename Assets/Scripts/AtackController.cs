using UnityEngine;

public class AtackController : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private float curentForce = 15f;//攻撃距離
    private float duration = 0.5f;　
    private float cooldown = 1.0f;//攻撃クールダウン
    //-----チャージ-------
    private const float chargeMax = 1.0f; //Maxチャージ量
    private float curentCharge = 0f;        //現在のチャージ量
    //-----硬直---------
    private float StrongRecoveryTime = 1.0f;//硬直時間
    private float curentRecoveryTime;       //現在の硬直時間

    [Header("ノックバック,無敵設定")]
    private float weakKnockback = 10.0f;    //弱ノックバック力
    private float strongKnockback = 20.0f;  //強ノッコバック力
    private float curentKnockback = 0.0f;   //現在のコックバック力

    [Header("当たり判定")]
    [SerializeField] SphereCollider attackArea; //攻撃判定
    [SerializeField] private float angle = 45f; //攻撃範囲

    Rigidbody rb;
    StateManager stateManager;

    private void Awake()
    {
        //初期化
        curentRecoveryTime = StrongRecoveryTime;

        //取得
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<StateManager>();
    }







}
