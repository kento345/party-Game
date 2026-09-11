using System.Linq.Expressions;
using UnityEngine;
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

        if (stateManager.attackState != AttackState.Atatck)
        {
            Vector3 move = new Vector3(inputVer.x, 0, inputVer.y) * curentSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + move);

            if (move != Vector3.zero)
            {
                Quaternion Rot = Quaternion.LookRotation(move, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, Rot, curentRotaSpeed * Time.deltaTime));
            }
        }
    }
}
