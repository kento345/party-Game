using System;
using UnityEngine;

/// <summary>
/// 移動ステート
/// </summary>
public enum MoveState
{
    Idel,Walk,
}
/// <summary>
/// 攻撃ステート
/// 攻撃力と統一可
/// </summary>
public enum AttackState
{
    None,Charge,Atatck,
}
/// <summary>
/// 攻撃力
/// </summary>
public enum AtackPower
{
    None,Weak,Strong,
}
/// <summary>
/// 状態ステート
/// </summary>
public enum State
{
    None,KnockBack,Hit,
}

public class StateManager : MonoBehaviour
{
    public MoveState moveState {  get; private set; } = MoveState.Idel;
    public AttackState attackState { get; private set; } = AttackState.None;
    public AtackPower attackPower { get;private set; }   = AtackPower.None;
    public State state { get; private set; } = State.None;

    public void UpdateMoveState(Vector2 inputVer)
    {
        moveState = inputVer.sqrMagnitude > 0.01 ? MoveState.Walk:MoveState.Idel;
    }

    public void SetAttackState(AttackState state)
    {
        attackState = state;
    }
    public void SetAttackPower(AtackPower power)
    {
        attackPower = power;
    }
    public void SetState(State State)
    {
        state = State;
    }
}
