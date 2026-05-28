using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
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
        //ノックバック時の移動拒否
        if (state.state == State.KnockBack)
        {

        }
        Vector2 inputVer = context.ReadValue<Vector2>();
        //ステート変更
        state.UpdateMoveState(inputVer);

    }

    public void OnAtack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
        if (context.canceled)
        {

        }
    }
}
