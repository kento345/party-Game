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

    public void SetCharge(float value)
    {
        curentCharge = value * chargeMax;
    }

    private void Update()
    {
        if (stateManager.attackState == AttackState.Charge && stateManager.state != State.KnockBack)
        {
            if (curentCharge < chargeMax)
            {
                curentCharge += Time.deltaTime;
            }
            if (curentCharge >= chargeMax)
            {
                stateManager.SetAttackPower(AtackPower.Strong); 
            }
        }
        if (stateManager.state == State.KnockBack)
        {
            SetCharge(0);
            stateManager.SetAttackPower(AtackPower.None);
        }

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

    public void Attack(int x)
    {
        if (x == 0)
        {
            if (stateManager.attackState == AttackState.Cooldown) { return; }
            if (stateManager.attackState == AttackState.Charge) { return; }
            //stateManager.SetState(State.None);

            stateManager.SetAttackState(AttackState.Charge);
        }
        if (x == 1)
        {
            if (stateManager.attackState == AttackState.Cooldown || stateManager.state == State.Rigid) { return; }

            if (stateManager.attackState == AttackState.Charge)
            {
                stateManager.SetAttackState(AttackState.Atatck);

                curentKnockback = stateManager.attackPower == AtackPower.Strong ? strongKnockback : weakKnockback;

                rb.AddForce(transform.forward * curentForce, ForceMode.Impulse);

                Invoke("EndAttack", duration);
            }
        }
    }
    void EndAttack()
    {
        rb.linearVelocity = Vector3.zero;
        stateManager.SetAttackState(AttackState.Cooldown);
        hasHit = false;

        if (stateManager.attackPower == AtackPower.Strong)
        {
            stateManager.SetState(State.Rigid);
        }

        stateManager.SetAttackPower(AtackPower.Weak);
        curentCharge = 0f;

        StartCoroutine(CooldownCount());
    }

    IEnumerator CooldownCount()
    {
        stateManager.SetAttackState(AttackState.Cooldown);
        yield return new WaitForSeconds(cooldown);
        stateManager.SetAttackState(AttackState.None);
    }

    private void OnTriggerStay(Collider other)
    {
        if(hasHit) {return; }
        if(stateManager == null || rb == null) return;

        if(stateManager.attackState != AttackState.Atatck) {return; }
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 posDir = other.transform.position + transform.position;
            float target_angle = Vector3.Angle(transform.forward, posDir);

            var dist = Vector3.Distance(other.transform.position, transform.position);

            if(target_angle > angle) { return; }
            float radius = attackArea.radius * transform.lossyScale.x;
            if(target_angle <= angle && Vector3.Distance(transform.position,other.transform.position) <= radius)
            {
                hasHit = true;
                //当たった時の処理

                CancelInvoke("EndAttack");
                EndAttack();
            }
        }
    }

#if UNITY_EDITORS
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        pos.y = 1.0f;
        Handles.color = Color.red;
        Handles.DrawSolidArc(pos, Vector3.up, Quaternion.Euler(0.0f, -angle, 0f) * transform.forward, angle * 2f, searchArea.radius);
    }
#endif
}


