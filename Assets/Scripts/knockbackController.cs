using System.Collections;
using UnityEngine;

public class knockbackController : MonoBehaviour
{
    /*変更点あり*/
    [Header("ノックバック,無敵設定")]
    private float knockbackTime = 0.3f;
    private float knockbackCounter;

    private Vector3 knockbackDir;

    private int initLayer_;
   private int invincibilityLayer_ = 7;
    /* [SerializeField] private ParticleSystem hit;
     [SerializeField] private ParticleSystem knock;*/

    [SerializeField] private float StunInvincibleTime = 1.0f; //無敵時間
    bool isKonckback = false;
    private bool isHit = false;
    Collider col;
    Rigidbody rb;

    //-----Script参照-----
    private StateManager stateManager;
    private CharacterController cs;
    //private AnimatorController animeCon;

    private AtackController ac;
    private PlayerInputController playerCon;
    //private BOTController botCon;

    private void Start()
    {
  /*      hit.Stop();
        knock.Stop();*/

        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        stateManager = GetComponent<StateManager>();
        cs = GetComponent<CharacterController>();
        //animeCon = GetComponent<AnimatorController>();
        ac = GetComponent<AtackController>();
        playerCon = GetComponent<PlayerInputController>();
        //botCon = GetComponent<BOTController>();

        initLayer_ = gameObject.layer;
    }

    private void Update()
    {
        if (isKonckback)
        {
            knockbackCounter -= Time.deltaTime;
            if (knockbackCounter <= 0)
            {
                isKonckback = false;
                //stateManager.SetState(State.None);
                rb.linearVelocity = Vector3.zero;
            }
        }
    }
    private void FixedUpdate()
    {
        if (isKonckback)
        {
            rb.linearVelocity = knockbackDir;
        }
    }

    public void KnockBack(Vector3 pos, float force)
    {
        if (isHit) return;
        //animeCon.isHit = true;
        isKonckback = true;
        if (ac != null)
        {
            ac.SetCharge(0);
        }
        if (stateManager != null)
        {
            stateManager.SetAttackState(AttackState.None);
        }
        knockbackCounter = knockbackTime;
        knockbackDir = pos.normalized * force;
        rb.linearVelocity = Vector3.zero;
/*
        if (botCon != null)
        {
            botCon.OnMove(Vector2.zero);
        }*/

        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        isHit = true;
        stateManager.SetState(State.KnockBack);
       /* if (hit && !hit.isPlaying)
            hit.Play();*/
        yield return new WaitForSeconds(0.05f);
        /*
                if (hit && hit.isPlaying)
                    hit.Stop();*/
        gameObject.layer = invincibilityLayer_;
        //knock.Play();

        yield return new WaitForSeconds(StunInvincibleTime);
        gameObject.layer = initLayer_;
        //knock.Stop();
        stateManager.SetState(State.None);
        isHit = false;
       /* if (animeCon.isHit)
            animeCon.isHit = false;*/
    }
}
