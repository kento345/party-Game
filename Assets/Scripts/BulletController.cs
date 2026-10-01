using UnityEngine;

public class BulletController : MonoBehaviour
{
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Shot()
    {
        rb.AddForce(transform.forward * 1000f);
    }
}
