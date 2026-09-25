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
    Rigidbody rb;

    //-----Script参照-----
    private StateManager stateManager;
    private AtackController atack;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        stateManager = GetComponent<StateManager>();
        atack = GetComponent<AtackController>();

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
        isKonckback = true;
        if (atack != null)
        {
            atack.SetCharge(0);
        }
        if (stateManager != null)
        {
            stateManager.SetAttackState(AttackState.None);
        }
        knockbackCounter = knockbackTime;
        knockbackDir = pos.normalized * force;
        rb.linearVelocity = Vector3.zero;
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        isHit = true;
        stateManager.SetState(State.KnockBack);

        yield return new WaitForSeconds(0.05f);

        gameObject.layer = invincibilityLayer_;

        yield return new WaitForSeconds(StunInvincibleTime);
        gameObject.layer = initLayer_;
        stateManager.SetState(State.None);
        isHit = false;
    }
}
