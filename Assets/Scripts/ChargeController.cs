using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChargeController : MonoBehaviour
{
    [SerializeField] private float maxChargeTime = 1.5f;
    [SerializeField] private float maxPos = 500f;
    [SerializeField] private float maxSize = 300f;
    [SerializeField] private Image pointImage;
    [SerializeField] private Image arrowImage;
    //position
    RectTransform pos;
    Vector2 initPos = new Vector2(0, 250);
    //height
    RectTransform imageSize;
    Vector2 initSize = new Vector2(100, 100);


    private AtackController ac;
    private StateManager stateManager;

    private void Awake()
    {
        ac = GetComponent<AtackController>();
        stateManager = GetComponent<StateManager>();

        pos = pointImage.rectTransform;
        imageSize = arrowImage.rectTransform;
        pos.anchoredPosition = initPos;
        imageSize.sizeDelta = initSize;
    }

    private void Update()
    {
        if (stateManager.attackState == AttackState.Charge)
        {
            pointImage.enabled = true;
            float speed = (maxPos - initPos.y) / maxChargeTime;
            float speedSize = (maxSize - initSize.y) / maxChargeTime;
            float height = pos.anchoredPosition.y;
            float heightSize = imageSize.sizeDelta.y;
            height += speed * Time.deltaTime;
            heightSize += speedSize * Time.deltaTime;

            height = Mathf.Clamp(height, initPos.y, maxPos);
            heightSize = Mathf.Clamp(heightSize, initSize.y, maxSize);

            pos.anchoredPosition = new Vector2(initPos.x,height);
            imageSize.sizeDelta = new Vector2(imageSize.sizeDelta.x, heightSize);
            float chargeRate = (height - initPos.y) / (maxPos - initPos.y);
            ac.SetCharge(chargeRate);
        }
        else
        {
            pos.anchoredPosition = initPos;
            imageSize.sizeDelta = initSize;
            ac.SetCharge(0);
            pointImage.enabled = false;
        } 
    }
}
