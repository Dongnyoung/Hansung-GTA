using UnityEngine;
using UnityEngine.UI;

public class QuestTrigger : MonoBehaviour
{
    public GameObject questUI;      // UI 패널
    public string questText;        // 퀘스트 내용
    public Text questTextField;     // 텍스트 표시용

    private bool playerInside = false;

    private void Start()
    {
        if (questUI != null)
            questUI.SetActive(false); // 처음엔 비활성화
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (questUI != null)
            {
                questUI.SetActive(true);
                if (questTextField != null)
                    questTextField.text = questText;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (questUI != null)
                questUI.SetActive(false);
        }
    }
}
