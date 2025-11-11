using UnityEngine;

public class DeliveryTargetTrigger : MonoBehaviour
{
    public QuestManager questManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (questManager != null && questManager.IsQuestActive())
        {
            questManager.QuestComplete();
        }
    }
}
