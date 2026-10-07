using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    private List<Material> tankColors = new();

    PlayerInputController inputCon;
    BotController botCon;
    StateManager state;

    private void OnEnable()
    {
        inputCon = GetComponentInParent<PlayerInputController>();
        botCon = GetComponentInParent<BotController>();

        if(inputCon != null)
        {
            inputCon.OnMoveStop(true);
        }
        if (botCon != null)
        {
            botCon.OnMoveStop(true);
        }
    }

    private void OnDisable()
    {
        if(inputCon != null)
        {
            inputCon.OnMoveStop(false);
        }
        if (botCon != null)
        {
            botCon.OnMoveStop(false);
        }
    }

    private void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.materials)
            {
                if (material.name.Contains("TankColor"))
                {
                    tankColors.Add(material);
                }
            }
        }
        var id = JoinDataHolder.instance.GetPlayerData[transform.root.gameObject];
        switch (id)
        {
            case 1:
                SetColor(Color.red);
                break;
            case 2:
                SetColor(Color.blue);
                break;
            case 3:
                SetColor(Color.green);
                break;

            case 4:
                SetColor(Color.yellow);
                break;
        }
    }

    public void SetColor(Color color)
    {
        foreach (var material in tankColors)
        {
            material.color = color;
        }
    }
}
