using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class JoinController : MonoBehaviour
{
    [Header("参加設定")]
    [SerializeField] private InputAction joinAction = default;
    [SerializeField] private InputAction startAction = default;
    [SerializeField] private GameObject playerPrefab = default;
    private int maxPlayer = 4;
    //-----Text-----
    [SerializeField] private TextMeshProUGUI p1text;
    [SerializeField] private TextMeshProUGUI p2text;
    [SerializeField] private TextMeshProUGUI p3text;
    [SerializeField] private TextMeshProUGUI p4text;
    private Dictionary<InputDevice, int> playerMap = new();
    [SerializeField] private List<string> debugDevices = new();

    private void Awake()
    {
        //参加InputActionの有効化
        joinAction.Enable();
        joinAction.performed += OnJoin;
        startAction.Enable();
        startAction.performed += OnGameStart;

        UpdateText(false);
    }

    private void OnDestroy()
    {
        joinAction.performed -= OnJoin;
        joinAction.Disable();
        startAction.performed -= OnGameStart;
        startAction.Disable();
    }

    /// <summary>
    /// 参加処理
    /// </summary>
    /// <param name="context"></param>
    void OnJoin(InputAction.CallbackContext context)
    {
        var device = context.control.device;
        //既に参加済み,4人以上参加してたらreturn
        if (playerMap.ContainsKey(device) || playerMap.Count >= maxPlayer) { return; }

        int playerID = playerMap.Count;
        playerMap.Add(device, playerID);

        UpdateText(true);
    }

    /// <summary>
    /// Textの更新
    /// </summary>
    void UpdateText(bool a)
    {
        TextMeshProUGUI[] texts = {p1text,p2text, p3text, p4text};
        if (!a)
        {
            foreach (var p in playerMap)
            {
                var device = p.Key;
                var playerID = p.Value;

                texts[playerID].enabled = true;
                texts[playerID].text = $"Push A To Join";
            }
        }
        if (a)
        {
            foreach (var p in playerMap)
            {
                var device = p.Key;
                var playerID = p.Value;

                texts[playerID - 1].enabled = true;
                texts[playerID - 1].text = $"Player {playerID}\nJoined";
            }
        }
    }

    void CreatePlayer(InputDevice device,int id)
    {
        if(device == null) { return; }
        var obj = PlayerInput.Instantiate(
            prefab: playerPrefab,
            playerIndex: id,
            pairWithDevice: device);

        obj.transform.position = transform.position;
        var input = obj.GetComponent<PlayerInputController>();

/*        if (input != null)
        {
            input.OnMoveStop(false);
        }*/

   /*     JoinDataHolder.instance.SetPlayerData(
            obj.gameObject,
            id
        );*/

        DontDestroyOnLoad(obj);
    }

    public void OnGameStart(InputAction.CallbackContext context)
    {
        if(playerMap.Count <= 0) { return; }
        startAction.Disable();
        joinAction.Disable();
        JoinDataHolder.instance.SetPlayerData(playerMap);

        SceneManager.LoadScene("MainGame");
    }
}