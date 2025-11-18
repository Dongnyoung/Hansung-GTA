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

    [SerializeField]
    public GameObject Info1; // 첫 번째 미션 정보 패널/UI
    public float infoDisplayStartTime = 2.0f; // Info1 활성화 시작 시간
    public float infoDisplayDuration = 5.0f; // Info1 표시 지속 시간
    private bool info1Shown = false; // Info1이 이미 표시되었는지 추적하는 플래그

    void Start()
    {
        gameTime = 0.0f; // 시작 시 게임 시간 초기화
        Player = GameObject.FindWithTag("Player"); // "Player" 태그를 가진 오브젝트를 찾아 저장
        // myPlayerController 대신 CshController 사용
        controller = Player.GetComponent<CshController>();

        // 시작할 때 Info1을 비활성화 상태로 둡니다.
        if (Info1 != null)
        {
            Info1.SetActive(false);
        }
    }

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        gameTime += Time.deltaTime; // 매 프레임마다 경과 시간 더하기

        // 미션정보
        if (Info1 != null)
        {
            if (!info1Shown && gameTime >= infoDisplayStartTime)
            {
                // 지정된 시간이 되면 Info1 활성화
                Info1.SetActive(true);
                info1Shown = true; // 표시되었다고 기록
            }

            // Info1 활성화 후 지정된 지속 시간이 지나면 비활성화
            if (info1Shown && gameTime >= infoDisplayStartTime + infoDisplayDuration)
            {
                Info1.SetActive(false);
            }
            
        }
        

        // 플레이 시간을 소수 둘째 자리까지 표시
        PlayTimetext.text = $"Time : {gameTime.ToString("F2")}";

        // 체력 바의 채워진 정도를 HP 비율로 설정
        if (controller != null)
        {
            hpImage.fillAmount = controller.HP / controller.maxHP;

            // 달리기 게이지 바의 채워진 정도를 현재 게이지 비율로 설정
            runningGazeImage.fillAmount = controller.currentRunningGaze / controller.maxRunningGaze;

            //장학금 텍스트 업데이트
            if (goldText != null)
            {
                goldText.text = $"Gold : {controller.gold}";
            }
        }
    }
    public void CloseInfo1()
    {
        if (Info1 != null)
        {
            Debug.Log("Info1 창을 닫습니다.");
            Info1.SetActive(false);
        }
    }
}