using UnityEngine;

public class HitController : MonoBehaviour
{
    [SerializeField] private LayerMask bulletLayer = default;

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & bulletLayer) != 0)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
        }
    }
}
