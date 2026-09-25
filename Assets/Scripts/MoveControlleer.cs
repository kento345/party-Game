using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
using UnityEngine.Windows;

public class MoveControlleer : MonoBehaviour
{
    //数値の変更はpublicじゃなく関数で行う
    [Header("移動,回転設定")]
    [SerializeField] private float speed = 15f;//移動速度
    private float speed2 = 0f;//チャージ中の移動
    [SerializeField] private float moveRate = 0.3f; //移動速度低下率
    float curentSpeed = 0f; //現在の速度

    [SerializeField] private float rotaSpeed = 10.0f;//回転速度
    private float rotaSpeed2 = 0f;//チャージ中の回転速度
    [SerializeField] private float rotaRate = 0.7f;//回転速度低下率
    private float curentRotaSpeed = 0f;//現在の回転速度

    private bool isRotating = false; //回転中かどうか
    private float targetRotationY = 0f; //目標の回転角度

    Vector2 inputVer;  //移動入力
    Rigidbody rb;

    private StateManager stateManager;

    private void Awake()
    {
        //初期化
        speed2 = speed * moveRate;
        rotaSpeed2 = rotaSpeed * rotaRate;

        //取得
        stateManager = GetComponent<StateManager>();
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 入力受け取り
    /// </summary>
    /// <param name="input"></param>
    public void SetMoveInput(Vector2 input)
    {
        inputVer = input;
    }

    private void FixedUpdate()
    {
        curentSpeed = speed;
        curentRotaSpeed = rotaSpeed;

        if (stateManager.attackState == AttackState.Charge)
        {
            curentSpeed = speed2;
            curentRotaSpeed = rotaSpeed2;
        }

        //攻撃以外の時の処理
        if (stateManager.attackState != AttackState.Atatck)
        {
            //移動処理
            Vector3 move = new Vector3(inputVer.x, 0, inputVer.y) * curentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);
            //回転処理
            if (move != Vector3.zero && !isRotating)
            {
                Quaternion Rot = Quaternion.LookRotation(move, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Rot, curentRotaSpeed * Time.deltaTime));
            }
            stateManager.UpdateMoveState(inputVer);
        }
        //場外の手前での回転処理
        if (stateManager.moveState == MoveState.Idel)
        {
            if (isRotating)
            {
                float currentY = rb.rotation.eulerAngles.y;

                float angle = Mathf.DeltaAngle(currentY, targetRotationY);

                if (Mathf.Abs(angle) <= 0.5f)
                {
                    rb.MoveRotation(Quaternion.Euler(0f, targetRotationY, 0f));
                    inputVer = Vector2.zero;

                    isRotating = false;
                }
                else
                {
                    float rotationAmount = Mathf.Sign(angle) * 100 * Time.fixedDeltaTime;

                    rotationAmount = Mathf.Clamp(rotationAmount, -Mathf.Abs(angle), Mathf.Abs(angle));
                    Quaternion deltaRotation = Quaternion.Euler(0f, rotationAmount, 0f);
                    rb.MoveRotation(rb.rotation * deltaRotation);
                }
            }
        } 
    }

    /// <summary>
    /// 180度回転処理
    /// </summary>
    /// <param name="x"></param>
    public void Rotate(bool x)
    {
        if (x && !isRotating)
        {
            isRotating = true;

            // 現在のY角度 + 180度を目標にする
            targetRotationY = rb.rotation.eulerAngles.y + 180f;

            // 0～360度に収める
            targetRotationY %= 360f;
        }
    }

    /// <summary>
    /// 回転の状態
    /// </summary>
    /// <returns></returns>
    public bool IsRotating()
    {
        return isRotating;
    }
}
