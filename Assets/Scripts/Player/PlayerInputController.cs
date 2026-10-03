using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    //入力値
    Vector2 inputVer;
    //入力値を参照
    public Vector2 InputVer => inputVer;

    //-----Script-----
    private StateManager state;
    private MoveControlleer move;
    private AtackController atack;

    private void Awake()
    {
        state = GetComponent<StateManager>();
        move = GetComponent<MoveControlleer>();
        atack = GetComponent<AtackController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //ノックバック時移動拒否
        if (state.state == State.KnockBack)
        {
            move.SetMoveInput(Vector2.zero);
            state.UpdateMoveState(Vector2.zero);
            return;
        }
        inputVer = context.ReadValue<Vector2>();
        //入力の更新
        //state.UpdateMoveState(inputVer);
        move.SetMoveInput(inputVer);
    }

    public void OnAtack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            atack.Attack(AttackState.Charge);
        }
        if (context.canceled)
        {
            atack.Attack(AttackState.Atatck);
        }
    }
}
