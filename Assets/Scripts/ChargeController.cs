using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ChargeController : MonoBehaviour
{
    [SerializeField] private float maxChargeTime = 1.5f;

    private AtackController ac;
    private StateManager stateManager;

    [SerializeField] private Image image;

    private float upSpeed = 1.0f;
    private Coroutine meter;

    private void Awake()
    {
        ac = GetComponent<AtackController>();
        stateManager = GetComponent<StateManager>();
        image.fillAmount = 0;
    }

    private void Update()
    {
        float speed = 1f / maxChargeTime;
        if(stateManager.attackState == AttackState.Charge)
        {
            image.fillAmount += speed * Time.deltaTime;
        }
        else
        {
            image.fillAmount = 0;
        }
        if(stateManager.state == State.KnockBack)
        {
            image.fillAmount = 0;
        }
        //0~1ÇÃîÕàÕêßå¿
        image.fillAmount = Mathf.Clamp01(image.fillAmount);
        ac.SetCharge(image.fillAmount);
    }
}
