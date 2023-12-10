using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingYut : MonoBehaviour
{
    /*
     * Yut에 따른 플레이어 이동 및 버튼 이동, 화살표 생성 및 화살표 눌렀을 시 플레이어 생성
        Trun인 position에 걸리면 Transform 배열을 바꿔주기
     */
    public Transform[] pos1;
    public Transform[] pos2;
    public Transform[] pos3;
    public Transform[] pos4;

    public Transform[] playerArray; // player가 해당하는 pos array
    public RectTransform[] yutButton; // 도, 개, 걸, 윷, 모, 빽도 버튼

    // UI에서 버튼 선택할 때 달라질 예정
    public PlayerState[] player = new PlayerState[4]; // 자신의 말
    // 캐릭터마다 붙어있는 버튼
    public GameObject[] characterButton; // character
    public GameObject[] returnButton; // return

    public int currentIndex = 0; // Button 위치 시킬 기준 인덱스, player 포지션과 동일해야 함
    public int resultIndex = 0; // 버튼 위치할 인덱스
    public List<int> yutResultIndex = new List<int>(); // 버튼이 이동할 위치를 저장

    public int[] yutArray = { 1, 2, 3, 4, 5, -1 }; // 도 개 걸 윷 모 빽도

    // 윷 결과 가져오기
    public string yutResult;
    public GameObject goalButton; // goal button, resultIndex보다 클 때 SetActive(true)

    private void Start()
    {
        player = GameManager.instance.players;
    }

    public void SetButtons()
    {
        if (GameManager.instance.players[0] != null)
        {
            for (int i = 0; i < characterButton.Length; i++)
            {
                characterButton[i].GetComponent<ButtonPositionSetter>().target = GameManager.instance.players[i].gameObject.transform;
                returnButton[i].GetComponent<ButtonPositionSetter>().target = GameManager.instance.players[i].gameObject.transform;
            }
        }
    }

    public void PlayingYutPlus()
    { // 윷 던지기 버튼 event
        if (!yutResult.Equals("Nack") && !(yutResult.Equals("Backdo") && currentIndex == 0))
        { // 낙이거나 현재 인덱스가 0이면서 빽도일 경우 앞으로 가지 않음
            for (int i = 0; i < 4; i++)
            {
                characterButton[i].SetActive(true); // 플레이어 선택 버튼, 골인한 플레이어 오브젝트의 버튼은 활성화 X
            }
        }
        // Nack일 때, 인덱스 0일 때 빽도일 때
    }

    public void YutButtonClick(int name)
    { // Canvas - YutObject - 도, 개, 걸, 윷, 모, 빽도 event
        for (int i = 0; i < 4; i++)
        {
            characterButton[i].SetActive(false);
            returnButton[i].SetActive(false);
        }

        currentIndex += yutArray[name]; // 현재 인덱스 리스트 삭제한 값과 같도록 변경
        yutResultIndex.Remove(name); // 추가된 리스트 삭제

        TurnPosition(playerArray, currentIndex); // 현재 위치 배열 변경

        PositionOut();
        if (goalButton.activeSelf)
        {
            goalButton.SetActive(false);
        }
    }
    #region Goal Button
    public void GoalButtonClick()
    { // Goal Button Event ... Error 있음
        // Goal Count++
        resultIndex = playerArray.Length - 1;
        PositionOut();

        for (int i = 0; i < 4; i++)
        {
            characterButton[i].SetActive(false);
            returnButton[i].SetActive(false);
        }

        /*if (Vector3.Distance(player.transform.position, playerArray[resultIndex - 1].position) <= 0.01f)
        { // move 끝났을 때
            goalButton.SetActive(false);
            player.SetActive(false);
        }*/
    }
    #endregion
    #region ButtonPosition
    private void PositionOut()
    { // Button Position out
        for (int i = 0; i < yutButton.Length; i++)
        {  // 윷 던질 때 마다 모든 버튼 Canvas 밖으로 배치
            yutButton[i].transform.position = Camera.main.WorldToScreenPoint(yutButton[i].transform.parent.position);
        }
    }

    public void PositionIn()
    { // Button Position in
        // Character Button click 시 불러옴, list 최소 1개 이상
        // YutState yutType = YutState.Backdo; // 초기화
        int yutType = 0;
        for (int i = 0; i < yutResultIndex.Count; i++)
        {
            resultIndex = currentIndex + yutArray[yutResultIndex[i]]; // 버튼 배치할 위치, 도 개 걸 윷 모 빽도
            if (yutArray[yutResultIndex[i]] != -1)
            { // 빽도가 아닐 때
                yutType = yutResultIndex[i];
            }

            if (resultIndex >= playerArray.Length)
            { // Goal
                goalButton.SetActive(true);
                continue;
            }
            else if (playerArray.Length > resultIndex)
            { // not Goal
                Vector3 screen = Camera.main.WorldToScreenPoint(playerArray[resultIndex].transform.position);
                yutButton[yutType].transform.position = screen; // 나온 윷에 맞는 버튼 포지션 설정
            }
        }
    }
    #endregion
    #region PlayerButton
    public void CharacterButtonClick(int playerNum)
    { // Canvas - CharacterButton event
        currentIndex = player[playerNum].currentIndex;
        playerArray = player[playerNum].currentArray;

        PositionIn();
        for (int i = 0; i < 4; i++)
        {
            returnButton[i].SetActive(true);
            characterButton[i].SetActive(false);
        }
        // 어떤 말을 선택했는지 설정
        GameManager.instance.playerNum = playerNum;
    }

    public void ReturnButtonClick()
    { // Canvas - ReturnButton event
        goalButton.SetActive(false);
        PositionOut();
        for (int i = 0; i < 4; i++)
        {
            characterButton[i].SetActive(true);
            returnButton[i].SetActive(false);
        }
    }
    #endregion
    public void TurnPosition(Transform[] pos, int num)
    { // Player 현재 위치한 Array 변경
        if (pos == pos1)
        {
            if (num == 5)
            { // pos1 -> pos3, 5
                playerArray = pos3;
                currentIndex = 5;
            }
            else if (num == 10)
            { // pos1 -> pos2, 10
                playerArray = pos2;
                currentIndex = 10;
            }
        }
        else if (pos == pos3)
        {
            if (num == 8)
            { // pos3 -> pos4, 8(22 위치)
                playerArray = pos4;
                currentIndex = 8;
            }
        }
        // Catch 당했을 때 playerArray = pos1로 변경
    }
    public void MoveButton()
    {
        PlayerMovement SelectPlayer = GameManager.instance.players[GameManager.instance.playerNum].GetComponent<PlayerMovement>();
        SelectPlayer.PlayerMove();
    }
}