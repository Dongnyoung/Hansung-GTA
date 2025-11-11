using UnityEngine;

public class DeliveryTargetTrigger : MonoBehaviour
{
    public QuestManager questManager; // Inspector에 드래그할 것

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered DeliveryTarget trigger.");
            if (questManager != null && questManager.IsQuestActive())
            {
                questManager.QuestComplete();
            }
        }
    }
}
