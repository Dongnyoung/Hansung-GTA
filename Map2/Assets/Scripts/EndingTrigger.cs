using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public EndingManager endingManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endingManager.OpenQuestWindow();   // 함수 이름 일치
        }
    }
}
