using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cshUIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject Player; // 플레이어 오브젝트를 저장할 변수
    public TextMeshProUGUI hptext; // HP(체력)를 표시할 텍스트
    public TextMeshProUGUI PlayTimetext; // 게임 시간을 표시할 텍스트
    public TextMeshProUGUI goldText; // Inspector에서 연결
    [SerializeField]
    public TextMeshProUGUI runningGazetxt; // 달리기 게이지를 표시할 텍스트
    public Image hpImage; // HP(체력)을 시각적으로 표시할 이미지(UI 바)
    public Image runningGazeImage; // 달리기 게이지를 시각적으로 표시할 이미지(UI 바)
    float gameTime; // 게임이 진행된 시간을 저장하는 변수
    [SerializeField]
    CshController controller; // 플레이어 컨트롤러 스크립트 참조

    void Start()
    {
        gameTime = 0.0f; // 시작 시 게임 시간 초기화
        Player = GameObject.FindWithTag("Player"); // "Player" 태그를 가진 오브젝트를 찾아 저장
        controller = Player.GetComponent<CshController>(); // 플레이어 오브젝트에서 myPlayerController 스크립트 가져오기
    }

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        gameTime += Time.deltaTime; // 매 프레임마다 경과 시간 더하기

        // 플레이 시간을 소수 둘째 자리까지 표시
        // PlayTimetext.text = "Time :" + gameTime.ToString("F2");
        PlayTimetext.text = $"Time : {gameTime.ToString("F2")}";

        // 체력 바의 채워진 정도를 HP 비율로 설정
        // hptext.text = "HP : " + Player.GetComponent<myPlayerController>().HP;
        hpImage.fillAmount = Player.GetComponent<CshController>().HP / Player.GetComponent<CshController>().maxHP;

        // 달리기 게이지 바의 채워진 정도를 현재 게이지 비율로 설정
        runningGazeImage.fillAmount = Player.GetComponent<CshController>().currentRunningGaze / Player.GetComponent<CshController>().maxRunningGaze;

        // 달리기 게이지를 텍스트로 표시 (현재는 주석 처리됨)
        // runningGazetxt.text= $"Gaze : {(Player.GetComponent<myPlayerController>().currentRunningGaze).ToString("F2")}";
        if (goldText != null)
        {
            goldText.text = $"Gold : {controller.gold}";
        }
    }

}
