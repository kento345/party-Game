using UnityEngine;
using UnityEngine.Rendering;

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
    public float GetRotaSpeed => curentRotaSpeed;

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
            Vector3 move = transform.forward * inputVer.y * curentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
            //回転処理
            var rota = inputVer.x * curentRotaSpeed * Time.fixedDeltaTime;
            var Rot = Quaternion.Euler(0,rota,0);
            rb.MoveRotation(rb.rotation * Rot);
            stateManager.UpdateMoveState(inputVer);
        }
    }
}
