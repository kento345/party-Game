using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class HitController : MonoBehaviour
{
    [SerializeField] private LayerMask bulletLayer = default;
    private bool isAlive = false;
    /// <summary>
    /// 生存処理(生:true,死:false)
    /// </summary>
    public bool IsAlive => isAlive;


    private void Awake()
    {
        isAlive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & bulletLayer) != 0)
        {
            var b = collision.gameObject.GetComponent<BulletController>();
            // BulletControllerがないなら無視
            if (b == null)
                return;
            // 自分が撃ったBulletなら無視
            if (b.Owner == gameObject)
                return;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(false);
            }
            isAlive = false;
        }
    }
}
