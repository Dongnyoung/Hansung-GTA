using UnityEngine;

public class QuestTriggerZone : MonoBehaviour
{
    public QuestManager questManager;   // QuestManager 연결

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 이미 진행 중이거나 완료된 퀘스트면 UI 띄우지 않음
        if (questManager.IsQuestActive() || questManager.IsQuestCompleted())
        {
            Debug.Log("퀘스트가 이미 진행 중이거나 완료됨, UI 표시 안함");
            return;
        }

        // QuestWindow 활성화 (Inspector에서 미리 연결)
        if (questManager.questWindow != null)
            questManager.questWindow.SetActive(true);
    }
}
