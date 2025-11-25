using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class QuestManager_Guide : MonoBehaviour
{
    public GameObject Player;
    public GuideNPCController guideContoller;
    public GameObject GuideNPC;
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

    public GameObject Info2;
    private bool info2Shown = false;

    private Vector3 guideStartPos;
    private Vector3 guideCompletePos;
    private GameObject completedZone;

    public GameObject missionStartArrow;
    public GameObject missionCompletedArrow;

    public GameObject secondMissionStartArrow;
    public GameObject secondMissionCompletedArrow;
    private void Start()
    {
        if (GuideNPC == null) GuideNPC = GameObject.FindWithTag("GuideNPC");
        if (completedZone == null) completedZone = GameObject.FindWithTag("GuideCompletedZone");
        if (missionCompletedArrow == null) missionCompletedArrow = GameObject.FindWithTag("MissionCompletedArrow");
        if (missionStartArrow == null) missionStartArrow = GameObject.FindWithTag("MissionStartArrow");
        if (secondMissionStartArrow == null) secondMissionStartArrow = GameObject.FindWithTag("SecondMissionStartArrow");
        if (secondMissionCompletedArrow == null) secondMissionCompletedArrow = GameObject.FindWithTag("SecondMissionCompletedArrow");
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (questComplete != null) questComplete.SetActive(false);
        if (questFail != null) questFail.SetActive(false);
        if (Player == null)
            Player = GameObject.FindWithTag("Player");
        remainingTime = questTimeLimit;
        if (Info2 != null) Info2.SetActive(false);
        info2Shown = false;
        if (GuideNPC != null)
        {
            guideStartPos = GuideNPC.transform.position;
            guideCompletePos = completedZone.transform.position;
        }
        if(missionCompletedArrow != null)
        {
            missionCompletedArrow.SetActive(false);
        }
        if (missionStartArrow != null)
        {
            missionStartArrow.SetActive(true);
        }
        if (secondMissionStartArrow != null)
        {
            secondMissionStartArrow.SetActive(false);
        }
        if(secondMissionCompletedArrow != null)
        {
            secondMissionCompletedArrow.SetActive(false);
        }
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
                missionText.text = $"상상관으로 이동하세요! 남은 시간: {seconds}s";
            }

            // 시간 초과 시 실패 처리
            if (remainingTime <= 0f)
            {
                QuestFailed();
            }
        }
        if (questCompleted && !info2Shown && questComplete != null && !questComplete.activeSelf)
        {
            OpenInfo2(); // Info2를 켜는 함수 호출
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
            guideContoller.isStopped = false;
            guideContoller.enabled = true;
            missionPanel.SetActive(true);
            missionCompletedArrow.SetActive(true);
            missionStartArrow.SetActive(false);
            if (missionText != null)
                missionText.text = $"상상관으로 이동하세요! 남은 시간: {Mathf.CeilToInt(remainingTime)}s";
        }

        Debug.Log("Quest accepted");
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
        {
            questComplete.SetActive(true);

            guideContoller.isStopped = true;
            missionCompletedArrow.SetActive(false);
        }
        if (GuideNPC != null)
        {
            GuideNPC.transform.position = guideCompletePos;
        }
        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {
                
                controller.gold += 150; // 1000골드 지급
            }
        }
        

        Debug.Log("Quest completed!");
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
        {
            questFail.SetActive(true);
            guideContoller.isStopped = true;
           
        }
        if (GuideNPC != null)
        {
            GuideNPC.transform.position = guideStartPos;
            
        }
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
    }

    // 완료/실패 패널 닫기
    public void CloseCompletePanel()
    {
        if (questComplete != null) {
            
            questComplete.SetActive(false);
            
        }
        questFailed = false;
    }

    public void OnClickConfirmButton()
    {
        if (questFail != null)
            questFail.SetActive(false);
        // 실패 상태 초기화
        questFailed = false;
    }
    public void OpenInfo2()
    {
        if (Info2 != null)
        {
            Info2.SetActive(true);
            info2Shown = true; // 플래그를 true로 설정하여 다시 켜지지 않게 함
            Debug.Log("Update 감지 후 Info2가 활성화되었습니다.");
        }
        if(secondMissionStartArrow != null)
        {
            secondMissionStartArrow.SetActive(true);
        }
    }
    public void CloseInfo2()
    {
        if (Info2 != null && Info2.activeSelf)
        {
            Debug.Log("Info2 창을 닫습니다.");
            Info2.SetActive(false);
        }
    }
    // 외부 접근용 프로퍼티
    public bool IsQuestActive() => isQuestActive;
    public bool IsQuestCompleted() => questCompleted;
    public bool IsQuestFailed() => questFailed;
}
