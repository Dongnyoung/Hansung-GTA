using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questWindow;         // 수락/거절 패널
    public GameObject missionPanel;        // 좌상단 미션 패널
    public TMP_Text missionText;           // 좌상단 텍스트

    private bool isQuestActive = false;

    void Start()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
    }

    // 버튼에서 호출할 함수 (public, 파라미터 없음)
    public void AcceptQuest()
    {
        isQuestActive = true;
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
            if (missionText != null)
                missionText.text = " 배달 중... 목적지로 이동하세요!";
        }
        Debug.Log("Quest accepted");
    }

    public void DeclineQuest()
    {
        isQuestActive = false;
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        Debug.Log("Quest declined");
    }

    public void QuestComplete()
    {
        if (!isQuestActive) return;
        isQuestActive = false;
        if (missionPanel != null) missionPanel.SetActive(false);
        Debug.Log("Quest completed!");
        // 보상, 이펙트 등 추가 가능
    }

    // DeliveryTargetTrigger에서 검사할 때 사용할 공개 접근자
    public bool IsQuestActive() => isQuestActive;
}
