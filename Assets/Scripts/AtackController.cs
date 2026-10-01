using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AtackController : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private GameObject bullet;//弾Prefab
    private float cooldown = 1.0f;          //攻撃クールダウン
    //-----チャージ-------
    private const float chargeMax = 1.0f;   //Maxチャージ量
    private float curentCharge = 0f;        //現在のチャージ量
/*    //-----硬直---------
    private float StrongRecoveryTime = 1.0f;//硬直時間
    private float curentRecoveryTime;       //現在の硬直時間*/

    [Header("ノックバック,無敵設定")]
    private float weakPower = 10.0f;    //弱ノックバック力
    private float strongPower = 20.0f;  //強ノッコバック力
    private float curentPower = 0.0f;   //現在のコックバック力

    [Header("当たり判定")]
    [SerializeField] LayerMask playerLayer;
    [SerializeField] private float angle = 45f; //攻撃範囲
    bool hasHit = false;


    Rigidbody rb;
    StateManager stateManager;

    private void Awake()
    {
        //初期化

        //取得
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<StateManager>();
    }

    /// <summary>
    /// チャージゲージの同期
    /// </summary>
    /// <param name="value"></param>
    public void SetCharge(float value)
    {
        curentCharge = Mathf.Clamp01(value);
    }

    private void Update()
    {
        //maxなら強攻撃
        if (curentCharge >= chargeMax)
        {
            stateManager.SetAttackPower(AtackPower.Strong);
        }
    }

    /// <summary>
    /// チャージ,攻撃処理
    /// </summary>
    /// <param name="x"></param>
    public void Attack(AttackState state)
    {
        //チャージ開始(ステートをチャージ中に)
        if (state == AttackState.Charge)
        {
            if (stateManager.attackState == AttackState.Cooldown || stateManager.attackState == AttackState.Charge) { return; }
            stateManager.SetAttackState(state);
        }
        //攻撃開始
        if (state == AttackState.Atatck)
        {
            if (stateManager.attackState == AttackState.Cooldown) { return; }

            if (stateManager.attackState == AttackState.Charge)
            {
                stateManager.SetAttackState(state);
                //attackPowerステートがStrongならstrongPower,それ以外ならweakPower
                curentPower = stateManager.attackPower == AtackPower.Strong ? strongPower : weakPower;

                var obj = Instantiate(bullet,new Vector3(transform.position.x,1f,transform.position.z + 1f),Quaternion.Euler(-20,0,0));

            }
        }
    }

    /// <summary>
    /// クールダウン処理
    /// </summary>
    /// <returns></returns>
    IEnumerator CooldownCount()
    {
        stateManager.SetAttackState(AttackState.Cooldown);
        yield return new WaitForSeconds(cooldown);
        stateManager.SetAttackState(AttackState.None);
    }
}


