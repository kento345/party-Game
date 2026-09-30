using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChargeController : MonoBehaviour
{
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float maxPos = 500f;
    [SerializeField] private Image image;
    RectTransform pos;
    Vector2 initPos = new Vector2(0, 250);

    private AtackController ac;
    private StateManager stateManager;

    private void Awake()
    {
        ac = GetComponent<AtackController>();
        stateManager = GetComponent<StateManager>();

        pos = image.rectTransform;
        pos.anchoredPosition = initPos;
    }

    private void Update()
    {

        if (stateManager.attackState == AttackState.Charge)
        {
            float speed = (maxPos - initPos.y) / maxChargeTime;
            float height = pos.anchoredPosition.y;
            height += speed * Time.deltaTime;

            height = Mathf.Clamp(height, initPos.y, maxPos);

            pos.anchoredPosition = new Vector2(initPos.x,height);
            float chargeRate = (height - initPos.y) / (maxPos - initPos.y);
            ac.SetCharge(chargeRate);
        }
        else
        {
            pos.anchoredPosition = initPos;
            ac.SetCharge(0);
        } 
    }
}
