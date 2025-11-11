using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questWindow;         // 수락/거절 UI 패널
    public GameObject missionPanel;        // 좌상단 미션 패널
    public TMP_Text missionText;           // 좌상단 텍스트

    [Header("UI Complete")]
    public GameObject questComplete;       // 미션 완료 패널 (인스펙터에서 직접 연결)
    public TMP_Text completeText;          // 완료 텍스트 (인스펙터에서 직접 설정)

    private bool isQuestActive = false;    // 진행 중 여부
    private bool questCompleted = false;   // 완료 여부

    private void Start()
    {
        // 처음엔 UI 패널들 비활성화
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (questComplete != null) questComplete.SetActive(false);
    }

    // 퀘스트 수락
    public void AcceptQuest()
    {
        if (isQuestActive || questCompleted) return; // 이미 진행 중이거나 완료된 경우 무시

        isQuestActive = true;

        if (questWindow != null)
            questWindow.SetActive(false);

        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
            if (missionText != null)
                missionText.text = "배달 중... 목적지로 이동하세요!";
        }

        Debug.Log("Quest accepted");
    }

    // 퀘스트 거절
    public void DeclineQuest()
    {
        if (isQuestActive || questCompleted) return;

        if (questWindow != null)
            questWindow.SetActive(false);

        if (missionPanel != null)
            missionPanel.SetActive(false);

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

        Debug.Log("Quest completed!");
    }

    // 완료 패널 닫기 (버튼에서 호출)
    public void CloseCompletePanel()
    {
        if (questComplete != null)
            questComplete.SetActive(false);
    }

    // 외부 접근용 프로퍼티
    public bool IsQuestActive() => isQuestActive;
    public bool IsQuestCompleted() => questCompleted;
}
