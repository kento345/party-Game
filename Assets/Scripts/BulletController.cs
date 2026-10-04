using Unity.VisualScripting;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private ParticleSystem exprosionPrefab;
    private float height = 1.5f;

    private float angle = 0;

    [SerializeField] private LayerMask hitLayer;

    Rigidbody rb;
    AtackController attack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (rb.position.y < height)
        {
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.Euler(30, angle, 0), Time.deltaTime * 10f);
        }
    }

    public void Shot(Vector3 startPos, Vector3 endPos)
    {
        //水平方向
        var direction = endPos - startPos;
        direction.y = 0;

        var distance = direction.magnitude;

        var gravity = -Physics.gravity.y;

        var verSpeed = Mathf.Sqrt(2f * gravity * height);

        var timeUp = verSpeed / gravity;

        var timeDown = Mathf.Sqrt(2f * height / gravity);

        var totalTime = timeUp + timeDown;

        var horVelocity = direction / totalTime;

        var velocity = horVelocity;
        velocity.y = verSpeed;

        rb.linearVelocity = velocity;
    }

    public void SetAttackController(AtackController controller ,float a)
    {
        attack = controller;
        angle = a;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & hitLayer) != 0)
        {
            attack.BulletDestory();
            var obj = Instantiate(exprosionPrefab, transform.position, Quaternion.identity);
            obj.Play();
            Destroy(this.gameObject);
        }
    }
}
