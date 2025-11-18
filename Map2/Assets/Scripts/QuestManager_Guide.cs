using UnityEngine;
using TMPro;

public class QuestManager_Guide : MonoBehaviour
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

    private void Start()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (questComplete != null) questComplete.SetActive(false);
        if (questFail != null) questFail.SetActive(false);
        if (Player == null)
            Player = GameObject.FindWithTag("Player");
        remainingTime = questTimeLimit;
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
            questComplete.SetActive(true);
        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {
                controller.gold += 100; // 1000골드 지급
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
            questFail.SetActive(true);

        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {
                controller.gold -= 100;        // 100골드 감소
                if (controller.gold < 0)        // 골드 음수 방지
                    controller.gold = 0;
            }
        }

        Debug.Log("Quest failed due to time out!");
    }

    // 완료/실패 패널 닫기
    public void CloseCompletePanel()
    {
        if (questComplete != null)
            questComplete.SetActive(false);
        questFailed = false;
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
