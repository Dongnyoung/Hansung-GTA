using UnityEngine;

public class GuideTargetTrigger : MonoBehaviour
{
    public QuestManager_Guide questManager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (questManager != null && questManager.IsQuestActive())
        {
            questManager.QuestComplete();

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (questManager.questComplete != null)
                questManager.questComplete.SetActive(false);
        }
    }
}
