using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerState : NetworkBehaviour
{
    private PlayingYut playingYut;

    // Player의 위치
    public int playerNum = 0; // 플레이어의 처음 위치
    public bool isPlaying = false; // 대기 상태가 아닌 판에 나와있는지
    public bool isGoal = false; // 골인 했는지 아닌지

    public Transform currentPositon;
    public Transform[] currentArray; // 자신이 현재 위치한 배열
    public int currentIndex = 0; // 현재 위치한 인덱스

    // Player NumImage
    public GameObject[] numImage; // numberImage GameObject 참조해주기

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }

    #region Unity Callback
    private void Start()
    {
        SetUp();
        playingYut.goalButton.GetComponent<Button>().onClick.AddListener(GoalInClick); // 골인 버튼을 눌렀을 때
    }

    private void Update()
    {
        currentPositon = currentArray[currentIndex];
    }

    private void SetUp()
    { // 플레이어 상태 처음 설정
        playingYut = FindObjectOfType<PlayingYut>();
        currentArray = playingYut.pos1;
    }

    #endregion
    #region SyncVar
    
    [SyncVar (hook = nameof(GoalTrans))]
    public bool isGoal = false; // 골인 했는지 아닌지
    [SyncVar (hook = nameof(StartPosTrans))]
    public Transform startPos;
    #endregion
    #region Client
    [Client]
    public void GoalInClick()
    {
        GoalIn_Command();
    }
    #endregion
    #region Command
    [Command(requiresAuthority = false)]
    private void GoalIn_Command()
    { 
        isGoal = true;
        GoalIn_RPC();
    }
    #endregion
    #region ClientRpc
    [ClientRpc]
    public void GoalIn_RPC()
    {
        if (isLocalPlayer)
        {
            startPos.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    #endregion
    #region Hook Method, 다른 클라이언트도 알아야 함
        private void StartPosTrans(Transform _old, Transform _new)
        {
            startPos = _new;
        }    
        private void PlayerPosTrans(Transform _old, Transform _new)
        {
            currentPositon = _new;
        }
        private void CurrentArrayTrans(Transform[] _old, Transform[] _new)
        {
            currentArray = _new;
        }
        private void CurrentIndexTrans(int _old, int _new)
        {
            currentIndex = _new;
        }
    private void GoalTrans(bool _old, bool _new)
    {
        isGoal = _new;
    }

    private void StartTrans(Transform _old, Transform _new)
    {
        startPos = _new;
    }
    #endregion
}