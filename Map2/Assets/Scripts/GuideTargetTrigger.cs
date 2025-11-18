using UnityEngine;

public class GuideTargetTrigger : MonoBehaviour
{
    public QuestManager_Guide questManager;
    public GuideNPCController controller;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (questManager != null && questManager.IsQuestActive())
        {
            questManager.QuestComplete();
            controller.enabled = false;

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
