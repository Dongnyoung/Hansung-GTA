using UnityEngine;

public class QuestTriggerZone_Exam : MonoBehaviour
{
    public QuestManager_Exam examManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            examManager.OpenQuestWindow();   // 함수 이름 일치
        }
    }
}
