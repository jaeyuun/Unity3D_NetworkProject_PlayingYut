using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerState : NetworkBehaviour
{
    private PlayingYut playingYut;

    // Player의 위치
    public int playerNum = 0; // 플레이어의 처음 위치
    public bool isPlaying = false; // 대기 상태가 아닌 판에 나와있는지
    public bool isGoal = false; // 골인 했는지 아닌지
    public Transform[] currentArray; // 자신이 현재 위치한 배열

    public int currentIndex = 0; // 현재 위치한 인덱스
    public Transform startPos;
    // public Player_Num myNum;

    // Player Button
    public GameObject characterButton = null;
    public GameObject returnButton = null;

    // Player NumImage
    public GameObject[] numImage; // numberImage GameObject 참조해주기

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("LocalPlayer");
    }

    #region Unity Callback
    private void Start()
    {
        SetUp();
        OnStart();
    }

    private void SetUp()
    { // 플레이어 상태 처음 설정
        playingYut = FindObjectOfType<PlayingYut>();
        currentArray = playingYut.pos1;
    }

    #endregion
    #region SyncVar
    #endregion
    #region Client
    [Client]
    private IEnumerator OnStart()
    {
        yield return new WaitForSeconds(1.5f);
        ButtonSetting();
    }
    #endregion
    #region Command
    [Command]
    private void ButtonSetting()
    {
        PlayerButtonSetting();
    }
    #endregion
    #region ClientRPC
    [ClientRpc] // 왜안되니... 12. 10 AM 03:20
    private void PlayerButtonSetting()
    { // Button Position Setting
        int index = int.Parse(startPos.gameObject.name);
        characterButton = playingYut.characterButton[index];
        returnButton = playingYut.returnButton[index];

        characterButton.GetComponent<ButtonPositionSetter>().target = gameObject.transform;
        returnButton.GetComponent<ButtonPositionSetter>().target = gameObject.transform;

        characterButton.SetActive(false);
        returnButton.SetActive(false);
    }
    #endregion
    #region Hook Method, 다른 클라이언트도 알아야 함
    private void PlayerStateTrans(Transform[] _old, Transform[] _new)
    {
        currentArray = _new;
    }
    #endregion
}