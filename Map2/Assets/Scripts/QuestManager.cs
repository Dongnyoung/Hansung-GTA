using System;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public GameObject Player;
    [Header("UI References")]
    public GameObject questWindow;
    public GameObject missionPanel;
    public TMP_Text missionText;

    [Header("UI Complete")]
    public GameObject questComplete;
    public TMP_Text completeText;
    public GameObject questFail;
    public TMP_Text failText;

    [Header("Quest Timer")]
    public float questTimeLimit = 20f; // 제한 시간 50초
    private float remainingTime;

    private bool isQuestActive = false;
    private bool questCompleted = false;
    private bool questFailed = false;

    public GameObject Info3;
    private bool info3Shown = false;

    public QuestManager_Guide guideQuestManager;

    public GameObject thirdMissionArrow;
    [Header("Quest BGM")]
    public AudioSource questBGM;
    public AudioSource backgroundBGM; // 평소에 흐르는 배경음악

    private void Start()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (questComplete != null) questComplete.SetActive(false);
        
        if(guideQuestManager==null) guideQuestManager = GameObject.FindWithTag("GuideQuest").GetComponent<QuestManager_Guide>();
        if (thirdMissionArrow == null) thirdMissionArrow = GameObject.FindWithTag("ThirdMissionArrow");
        if (questFail != null) questFail.SetActive(false); 
        if (Player == null)
            Player = GameObject.FindWithTag("Player");
        remainingTime = questTimeLimit;
        if (Info3 != null) Info3.SetActive(false);
        info3Shown = false;
        
        if(thirdMissionArrow!=null) thirdMissionArrow.SetActive(false);
    }

    private void Update()
    {
        if (isQuestActive && !questCompleted)
        {
            // 제한시간 감소
            remainingTime -= Time.deltaTime;

            // 미션 패널 텍스트에 남은 시간 표시
            if (missionText != null)
            {
                int seconds = Mathf.CeilToInt(remainingTime);
                missionText.text = $"배달 중... 목적지로 이동하세요! 남은 시간: {seconds}s";
            }

            // 시간 초과 시 실패 처리
            if (remainingTime <= 0f)
            {
                QuestFailed();
            }
        }
        if (questCompleted && !info3Shown && questComplete != null && !questComplete.activeSelf)
        {
            OpenInfo3(); // Info2를 켜는 함수 호출
        }
    }

    // 퀘스트 수락
    public void AcceptQuest()
    {
        if (isQuestActive || questCompleted) return;

        isQuestActive = true;
        questFailed = false;
        remainingTime = questTimeLimit; // 제한시간 초기화

        if (questWindow != null) questWindow.SetActive(false);

        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
            
            if (missionText != null)
                missionText.text = $"배달 중... \n목적지로 이동하세요! \n남은 시간: {Mathf.CeilToInt(remainingTime)}s";
        }
        if(guideQuestManager != null)
        {
            
            guideQuestManager.secondMissionStartArrow.SetActive(false);
            guideQuestManager.secondMissionCompletedArrow.SetActive(true);
        }
        Debug.Log("Quest accepted");

        if (questBGM != null)
            questBGM.Play();
            questBGM.time = 14f; // 하이라이트 시작 지점(초)
        if (backgroundBGM != null) 
            backgroundBGM.Stop();
    }

    // 퀘스트 거절
    public void DeclineQuest()
    {
        if (isQuestActive || questCompleted) return;

        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);

        Debug.Log("Quest declined");
    }

    // 퀘스트 완료
    public void QuestComplete()
    {
        if (!isQuestActive) return;

        isQuestActive = false;
        questCompleted = true;

        // 진행 중 미션 패널 숨기기
        if (missionPanel != null)
            missionPanel.SetActive(false);

        // 완료 패널 표시 (인스펙터에서 연결된 패널 그대로 사용)
        if (questComplete != null)
            questComplete.SetActive(true);
        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {
                controller.gold += 150; // 1000골드 지급
            }
        }
        if (guideQuestManager != null)
        {
            guideQuestManager.secondMissionCompletedArrow.SetActive(false);
        }
        if (thirdMissionArrow != null)
        {
            thirdMissionArrow.SetActive(true);
        }
        Debug.Log("Quest completed!");

        if (questBGM != null)
            questBGM.Stop();
        if (backgroundBGM != null) 
            backgroundBGM.Play();
    }

    // 퀘스트 실패
    private void QuestFailed()
    {
        isQuestActive = false;
        questFailed = true;

        // 미션 패널 숨기기
        if (missionPanel != null)
            missionPanel.SetActive(false);

        // 완료/실패 패널 표시 (완료 패널 활용)
        if (questFail != null)
            questFail.SetActive(true);

        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {

                controller.HP -= 3f;
                if (controller.HP < 0)
                    controller.HP = 0; // 체력이 음수가 되면 0으로 고정
            }
        }

        Debug.Log("Quest failed due to time out!");

        if (questBGM != null)
            questBGM.Stop();
        if (backgroundBGM != null) 
            backgroundBGM.Play();
    }

    // 완료/실패 패널 닫기
    public void CloseCompletePanel()
    {
        if (questComplete != null)
            questComplete.SetActive(false);
        questFailed = false;
    }

    public void OpenInfo3()
    {
        if (Info3 != null)
        {
            Info3.SetActive(true);
            info3Shown = true; // 플래그를 true로 설정하여 다시 켜지지 않게 함
            Debug.Log("Update 감지 후 Info3가 활성화되었습니다.");
        }
    }
    public void CloseInfo3()
    {
        if (Info3 != null && Info3.activeSelf)
        {
            Debug.Log("Info3 창을 닫습니다.");
            Info3.SetActive(false);
        }
    }
    public void OnClickConfirmButton()
    {
        if (questFail != null)
            questFail.SetActive(false);
        // 실패 상태 초기화
        questFailed = false;
    }

    // 외부 접근용 프로퍼티
    public bool IsQuestActive() => isQuestActive;
    public bool IsQuestCompleted() => questCompleted;
    public bool IsQuestFailed() => questFailed;
}
