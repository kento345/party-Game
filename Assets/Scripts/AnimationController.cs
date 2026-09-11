using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private bool isStart = false;
    private bool isAttack1 = false;
    private bool isAttack2 = false;
    private bool isHit = false;
    float mag = 0;

    private PlayerInputController inpCon;
    private StateManager stateManager;
    //private 

    Animator animator;

    private void Start()
    {
        inpCon = GetComponent<PlayerInputController>();
        stateManager = GetComponent<StateManager>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
       
        if (inpCon != null) {
            
                mag = inpCon.InputVer().magnitude;
        }

        animator.SetFloat("Speed",mag);
        animator.SetInteger("IsChage", (int)stateManager.attackState);
        animator.SetInteger("IsAttack", (int)stateManager.attackState);
        animator.SetInteger("IsHit", (int)stateManager.state);
        /*        animator.SetBool("IsChage", isStart);
                animator.SetBool("IsAttack1", isAttack1);
                animator.SetBool("IsAttack2", isAttack2);
                animator.SetBool("IsHit", isHit);*/
    }

    public void IsStart(bool a) { isStart = a; } 
    public void IsAttack1(bool a) { isAttack1 = a; }

    public void IsAttack2(bool a) {isAttack2 = a; } 

    public void IsHit1(bool a) { isHit = a; }


}
