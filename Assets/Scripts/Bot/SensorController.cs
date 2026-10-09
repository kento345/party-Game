using UnityEngine;

public class SensorController : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    private bool isHit = false;
    public bool IsHit => isHit;

    private void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & playerLayer) != 0){
            isHit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            isHit = false;
        }
    }
}
