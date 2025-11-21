using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager_Exam : MonoBehaviour
{
    public GameObject Player;
    [Header("Quest Panels")]
    public GameObject questWindow;               // 최초 수락/거절 창
    public GameObject missionUIPanel;            // 시간 UI 패널
    public GameObject questFailedPanel;
    public GameObject questCompletedPanel;// 성공 패널

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;            // 미션 UI 안 타이머 TextMeshPro

    [Header("Questions")]
    public GameObject[] questionPanels;          // 문제 패널 배열
    public ToggleGroup[] answerToggleGroups;     // 문제별 ToggleGroup
    public int[] correctAnswerIndex;             // 문제별 정답 인덱스

    [Header("Timer Settings")]
    public float totalTime = 100f;               // 제한 시간
    private float remainingTime;
    private bool isTimerRunning = false;

    private int currentIndex = 0;                // 현재 문제 번호

    // ===========================
    // 퀘스트 상태
    // ===========================
    private bool questTaken = false;             // 퀘스트를 이미 받았는지
    private bool questCompleted = false;         // 성공 완료 여부

    // ===========================
    // 초기 상태 설정
    // ===========================
    void Start()
    {
        questWindow.SetActive(false);
        missionUIPanel.SetActive(false);
        questFailedPanel.SetActive(false);
        questCompletedPanel.SetActive(false);

        foreach (var panel in questionPanels)
            panel.SetActive(false);
    }

    // ===========================
    // 트리거존에서 호출
    // ===========================
    public void OpenQuestWindow()
    {
        if (questTaken && questCompleted)
        {
            Debug.Log("퀘스트는 이미 완료되었습니다.");
            return; // 성공 완료 시 재진행 불가
        }

        questWindow.SetActive(true);
    }

    // ===========================
    // 수락 눌렀을 때
    // ===========================
    public void AcceptQuest()
    {
        questWindow.SetActive(false);

        questTaken = true;  // 수락 상태
        currentIndex = 0;
        remainingTime = totalTime;

        missionUIPanel.SetActive(true);
        isTimerRunning = true;

        ShowQuestion(currentIndex);
    }

    // ===========================
    // 거절 눌렀을 때
    // ===========================
    public void DeclineQuest()
    {
        questWindow.SetActive(false);
    }

    // ===========================
    // 문제 표시 함수
    // ===========================
    void ShowQuestion(int index)
    {
        if (index < 0 || index >= questionPanels.Length) return;

        foreach (var panel in questionPanels)
            panel.SetActive(false);

        questionPanels[index].SetActive(true);

        ClearAllToggles(answerToggleGroups[index]);
    }

    void ClearAllToggles(ToggleGroup group)
    {
        group.SetAllTogglesOff();
    }

    public void OnClickSubmit()
    {
        if (currentIndex < 0 || currentIndex >= questionPanels.Length) return;

        ToggleGroup group = answerToggleGroups[currentIndex];
        Toggle selectedToggle = group.ActiveToggles().FirstOrDefault();

        if (selectedToggle == null)
        {
            Debug.Log("선택해주세요!");
            return;
        }

        int selectedIndex = selectedToggle.transform.GetSiblingIndex();

        if (selectedIndex == correctAnswerIndex[currentIndex])
        {
            Debug.Log("정답!");
            currentIndex++;

            if (currentIndex >= questionPanels.Length)
            {
                Debug.Log("모든 문제 성공!");
                EndQuestSuccess();
            }
            else
            {
                ShowQuestion(currentIndex);
            }
        }
        else
        {
            Debug.Log("오답! 실패!");
            EndQuestFail();
        }
    }

    void EndQuestSuccess()
    {
        isTimerRunning = false;
        questCompleted = true; // 성공 처리
        questTaken = false;    // 성공 후 재수락 불가 처리

        // 모든 문제 패널 숨기기
        foreach (var panel in questionPanels)
            panel.SetActive(false);

        // 미션 UI 숨기기
        missionUIPanel.SetActive(false);

        // 성공 패널 표시
        if (questCompletedPanel != null)
            questCompletedPanel.SetActive(true);

        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {

                controller.gold += 10000; // 1000골드 지급
            }
        }
    }

    void EndQuestFail()
    {
        isTimerRunning = false;
        questTaken = false; // 실패 시 다시 받을 수 있도록

        foreach (var panel in questionPanels)
            panel.SetActive(false);

        missionUIPanel.SetActive(false);
        questFailedPanel.SetActive(true);

        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {
                controller.gold -= 100;
                if (controller.gold < 0) controller.gold = 0;

                controller.HP -= 3f;
                if (controller.HP < 0) controller.HP = 0;
            }
        }
    }

    public void CloseFailedPanel()
    {
        questFailedPanel.SetActive(false);
    }

    public void CloseCompletedPanel()
    {
        questCompletedPanel.SetActive(false);
    }

    void Update()
    {
        if (!isTimerRunning) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "시험 시간\n" + Mathf.CeilToInt(remainingTime).ToString() + "s";

        if (remainingTime <= 0)
        {
            Debug.Log("시간초과 실패!");
            EndQuestFail();
        }
    }
}
