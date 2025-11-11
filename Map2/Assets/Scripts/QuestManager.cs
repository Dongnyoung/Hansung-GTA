using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questWindow;         // 수락/거절 UI 패널
    public GameObject missionPanel;        // 좌상단 미션 패널
    public TMP_Text missionText;           // 좌상단 텍스트

    [Header("Dialogue UI")]
    public GameObject dialogueUI;          // 완료 후 대화창
    public TMP_Text dialogueText;          // 대화 텍스트

    private bool isQuestActive = false;    // 진행 중
    private bool questCompleted = false;   // 완료 여부

    void Start()
    {
        if (questWindow != null) questWindow.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
        if (dialogueUI != null) dialogueUI.SetActive(false);
    }

    // 퀘스트 수락
    public void AcceptQuest()
    {
        if (isQuestActive || questCompleted) return; // 이미 진행 중이거나 완료되면 무시

        isQuestActive = true;
        if (questWindow != null) questWindow.SetActive(false);
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
        if (isQuestActive || questCompleted) return; // 진행 중이면 거절 불가

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

        if (missionPanel != null) missionPanel.SetActive(false);
        Debug.Log("Quest completed!");

        ShowDialogue("고마워요! 미션 완료했습니다!");
    }

    private void ShowDialogue(string message)
    {
        if (dialogueUI != null && dialogueText != null)
        {
            dialogueText.text = message;
            dialogueUI.SetActive(true);
        }
    }

    public bool IsQuestActive() => isQuestActive;
    public bool IsQuestCompleted() => questCompleted;
}
