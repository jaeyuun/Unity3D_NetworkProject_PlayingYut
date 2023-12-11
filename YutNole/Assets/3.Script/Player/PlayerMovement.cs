using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private PlayingYut playingYut;
    private PlayerState playerState;
    private Throw_Yut throw_Yut;
    public Transform[] playerArray; // player가 해당하는 pos array
    public int currentIndex = 0; // player 현재 index, 버튼 클릭 후 이동할 때마다 바뀜
    public int targetIndex = 0; // 버튼 클릭 시 이동할 index
    public Transform targetPos; // 버튼 클릭 시 이동할 위치

    public float speed = 4f;
    private int moveIndex = 0; // 이동 인덱스
    public bool isBackdo = false;
    public bool isGoal = false;

    private void Awake()
    {
        playingYut = FindObjectOfType<PlayingYut>();
        // UI Player 선택했을 때로 나중에 이동
        playerArray = playingYut.pos1;
        playingYut.playerArray = playerArray;
        playerState = GetComponent<PlayerState>();
        throw_Yut = FindObjectOfType<Throw_Yut>();
    }

    public void PlayerMove(int index)
    { // 도 개 걸 윷 모 빽도 버튼 event
        transform.position = playerArray[currentIndex].position; // 플레이어가 위치할 포지션
        targetIndex = index; // 버튼을 눌렀을 때 이동할 플레이어 타겟 인덱스

        if (targetIndex - currentIndex == -1)
        { // Backdo
            isBackdo = true;
            moveIndex = currentIndex;
        }
        else if (targetIndex >= playerArray.Length)
        { // Goal 
            isGoal = true;
            moveIndex = playerArray.Length - 1;
        }
        else
        { // 골인, 빽도 이외
            moveIndex = targetIndex;
        }

        StartCoroutine(Move_Co());
        
        //만약 도착한 장소에 적팀이 있다면?
        //hasChance =true 줘야함
        //없다면? else
        //if(Yutindex.count > 0) 
        // playingYut.PlayingYutPlus();
        //else turn 종료 
        //턴종료 조건 : 카운트 0 , 던질기회 X
    }

    private IEnumerator Move_Co()
    {
        for (int i = currentIndex; i <= moveIndex; i++)
        {
            if (isBackdo)
            {
                targetPos = playerArray[i - 1];
            }
            else
            {
                targetPos = playerArray[i];
            }
            while (transform.position != targetPos.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos.position, Time.deltaTime * speed);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
        }

        isBackdo = false;
        currentIndex = targetIndex;
        playerArray = playingYut.playerArray;
        playingYut.player[GameManager.instance.playerNum].currentIndex = currentIndex;
        playingYut.player[GameManager.instance.playerNum].currentArray = playerArray;

        foreach (PlayerState player in GameManager.instance.tempPlayers)
        {
            if (player.gameObject == gameObject) continue;
            if (Vector2.Distance(player.transform.position, gameObject.transform.position) < 0.01f)
            {
                if (player.tag == gameObject.tag)
                {
                    //업기
                    Debug.Log("업");
                    //player.transform.SetParent(gameObject.transform);
                    //todo.. 내일의민준아... 업기 해줘.
                    //List<PlayerState> list // 업은 말의 리스트
                    //업은애를 리스트에 넣어
                    //잡혔을때 리스트에있는애도 초기화해줘
                    //골인했을대도 리스트에있는 수만큼 +1 해주고 위치도 초기화해줘
                    //현재 2마리 // 한마리더업힘    1  (2)  (3)
                     
                }
                else
                {
                    Debug.Log("잡");
                    Server_Manager.instance.Catch(player);
                    GameManager.instance.hasChance = true;
                    //todo 잡았을때 애니메이션 넣기
                    //잡기
                }
            }
        }
        // playingYut.OnDeleteThisIndex.Invoke(playingYut.removeIndex);
        if (playerState.isGoal)
        {
            gameObject.transform.position = playerState.startPos.transform.position;
            playingYut.goalButton.SetActive(false);
            throw_Yut.Yut_Btn_Click(playingYut.removeIndex); // result panel remove
        }

        GameManager.instance.PlayerTurnChange();
    }
}
