using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
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
    bool hasHit = false;


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

    /// <summary>
    /// チャージゲージの同期
    /// </summary>
    /// <param name="value"></param>
    public void SetCharge(float value)
    {
        curentCharge = value * chargeMax;
    }

    private void Update()
    {
        //ステートがチャージかつノックバック時じゃないときにチャージ処理
        if (stateManager.attackState == AttackState.Charge && stateManager.state != State.KnockBack)
        {
            //ゲージmax以外はゲージ上昇
            if (curentCharge < chargeMax)
            {
                curentCharge += Time.deltaTime;
            }
            //maxなら強攻撃に
            if (curentCharge >= chargeMax)
            {
                stateManager.SetAttackPower(AtackPower.Strong); 
            }
        }
        //ノックバック中はゲージ0,
        if (stateManager.state == State.KnockBack)
        {
            SetCharge(0);
            stateManager.SetAttackPower(AtackPower.None);
        }
        //硬直中の処理
        if (stateManager.state == State.Rigid)
        {
            if (curentRecoveryTime > 0f)
            {
                curentRecoveryTime -= Time.deltaTime;
            }
            if(curentRecoveryTime <= 0f)
            {
                stateManager.SetState(State.None);
                curentRecoveryTime = StrongRecoveryTime;
            }
        }
    }

    /// <summary>
    /// チャージ,攻撃処理
    /// </summary>
    /// <param name="x"></param>
    public void Attack(int x)
    {
        //チャージ開始(ステートをチャージ中に)
        if (x == 0)
        {
            if (stateManager.attackState == AttackState.Cooldown || stateManager.attackState == AttackState.Charge || stateManager.state == State.Rigid) { return; }

            stateManager.SetAttackState(AttackState.Charge);
        }
        //攻撃開始
        if (x == 1)
        {
            if (stateManager.attackState == AttackState.Cooldown || stateManager.state == State.Rigid) { return; }

            if (stateManager.attackState == AttackState.Charge)
            {
                stateManager.SetAttackState(AttackState.Atatck);

                //attackPowerステートがStrongならstrongKnockback,それ以外ならweakKnockback
                curentKnockback = stateManager.attackPower == AtackPower.Strong ? strongKnockback : weakKnockback;

                rb.AddForce(transform.forward * curentForce, ForceMode.Impulse);

                Invoke(nameof(EndAttack), duration);
            }
        }
    }

    /// <summary>
    /// 攻撃終了処理
    /// </summary>
    void EndAttack()
    {
        //AddForceの前に飛ばす処理を強制終了
        rb.linearVelocity = Vector3.zero;
        stateManager.SetAttackState(AttackState.Cooldown);
        hasHit = false;

        //強攻撃なら硬直
        if (stateManager.attackPower == AtackPower.Strong)
        {
            stateManager.SetState(State.Rigid);
        }

        stateManager.SetAttackPower(AtackPower.None);
        curentCharge = 0f;

        StartCoroutine(CooldownCount());
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

    /// <summary>
    /// 攻撃の当たり判定処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if(stateManager == null || rb == null || stateManager.attackState != AttackState.Atatck || hasHit) return;

        //PlayerTagに当たった時
        if (other.gameObject.CompareTag("Player"))
        {
            //相手の方向ベクトルを算出
            Vector3 posDir = other.transform.position - transform.position;
            //自身の正面から相手のいる位置の角度
            float target_angle = Vector3.Angle(transform.forward, posDir);
            //距離を取得
            var dist = Vector3.Distance(other.transform.position, transform.position);

            //攻撃範囲外はreturn
            if(target_angle > angle) { return; }
            float radius = attackArea.radius * transform.lossyScale.x;
            //攻撃範囲内
            if(target_angle <= angle && dist <= radius)
            {
                hasHit = true;
                //当たった時の処理

                CancelInvoke(nameof(EndAttack));
                EndAttack();
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        pos.y = 1.5f;
        Handles.color = Color.red;
        Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0.0f, -angle, 0f) * transform.forward, angle * 2f, attackArea.radius);
    }
#endif
}


