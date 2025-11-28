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
    public GameObject questFailedPanel;          // 실패 패널
    public GameObject questCompletedPanel;       // 성공 패널

    [Header("Timer UI")]
    public TextMeshProUGUI timerText;

    [Header("Questions")]
    public GameObject[] questionPanels;
    public ToggleGroup[] answerToggleGroups;
    public int[] correctAnswerIndex;

    [Header("Timer Settings")]
    public float totalTime = 100f;
    private float remainingTime;
    private bool isTimerRunning = false;

    private int currentIndex = 0;
    private bool questTaken = false;
    private bool questCompleted = false;

    public QuestManager deliverQuestManager;


    [Header("Info4")]
    public GameObject Info4;
    private bool info4Shown = false;


    public GameObject EndingArrow;

    [Header("Quest BGM")]
    public AudioSource questBGM;
    public AudioSource backgroundBGM;

    // ---------------------------
    // 초기 설정
    // ---------------------------
    void Start()
    {
        questWindow.SetActive(false);
        missionUIPanel.SetActive(false);
        questFailedPanel.SetActive(false);
        questCompletedPanel.SetActive(false);
        if (EndingArrow == null) EndingArrow = GameObject.FindWithTag("EndingArrow");
        if (deliverQuestManager == null)
        {
            GameObject found = GameObject.FindWithTag("DeliverQuest");
            if (found != null)
                deliverQuestManager = found.GetComponent<QuestManager>();
        }

        foreach (var panel in questionPanels)
            panel.SetActive(false);

        // Info4 초기화
        if (Info4 != null) Info4.SetActive(false);
        info4Shown = false;

        if(EndingArrow!=null) EndingArrow.SetActive(false);
    }

    // ---------------------------
    // 퀘스트 열기
    // ---------------------------
    public void OpenQuestWindow()
    {
        if (questTaken && questCompleted)
        {
            Debug.Log("이미 퀘스트를 완료했습니다.");
            return;
        }

        questWindow.SetActive(true);
    }

    // ---------------------------
    // 퀘스트 수락
    // ---------------------------
    public void AcceptQuest()
    {
        questWindow.SetActive(false);

        questTaken = true;
        currentIndex = 0;
        remainingTime = totalTime;

        missionUIPanel.SetActive(true);
        isTimerRunning = true;

        if (deliverQuestManager != null)
            deliverQuestManager.thirdMissionArrow.SetActive(false);

        ShowQuestion(currentIndex);

        if (questBGM != null)
            questBGM.Play();
        if (backgroundBGM != null)
            backgroundBGM.Stop();
    }

    public void DeclineQuest()
    {
        questWindow.SetActive(false);
    }

    // ---------------------------
    // 문제 표시
    // ---------------------------
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
        if (group != null)
            group.SetAllTogglesOff();
    }

    // ---------------------------
    // 제출 버튼
    // ---------------------------
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

    // ---------------------------
    // 성공 처리
    // ---------------------------
    void EndQuestSuccess()
    {
        isTimerRunning = false;
        questCompleted = true;
        questTaken = false;

        foreach (var panel in questionPanels)
            panel.SetActive(false);

        missionUIPanel.SetActive(false);

        if (questCompletedPanel != null)
            questCompletedPanel.SetActive(true);

        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {
                controller.gold += 500;
            }
        }
        if(EndingArrow != null)
        {
            EndingArrow.SetActive(true);
        }

        if (questBGM != null)
            questBGM.Stop();
        if (backgroundBGM != null)
            backgroundBGM.Play();
    }

    // ---------------------------
    // 실패 처리
    // ---------------------------
    void EndQuestFail()
    {
        isTimerRunning = false;
        questTaken = false;

        foreach (var panel in questionPanels)
            panel.SetActive(false);

        missionUIPanel.SetActive(false);

        questFailedPanel.SetActive(true);

        if (Player != null)
        {
            CshController controller = Player.GetComponent<CshController>();
            if (controller != null)
            {

                controller.HP -= 3f;
                if (controller.HP < 0) controller.HP = 0;
            }
        }

        if (questBGM != null)
            questBGM.Stop();
        if (backgroundBGM != null)
            backgroundBGM.Play();
    }

    public void CloseFailedPanel()
    {
        questFailedPanel.SetActive(false);
    }

    public void CloseCompletedPanel()
    {
        questCompletedPanel.SetActive(false);


        if (!info4Shown)
            OpenInfo4();
    }

    // ===========================================================
    //  GuideQuest처럼 Info2 자동 실행 기능
    // ===========================================================
    public void OpenInfo4()
    {
        if (Info4 != null)
        {
            Info4.SetActive(true);
            info4Shown = true;

            Debug.Log("Info2가 활성화되었습니다.");
        }

        
    }

    public void CloseInfo4()
    {
        if (Info4 != null && Info4.activeSelf)
        {
            Info4.SetActive(false);
        }
    }

    // ---------------------------
    // 타이머
    // ---------------------------
    void Update()
    {
        if (!isTimerRunning) return;

        remainingTime -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "시험 시간\n" + Mathf.CeilToInt(remainingTime) + "s";

        if (remainingTime <= 0)
        {
            Debug.Log("시간 초과 실패!");
            EndQuestFail();
        }
    }
}
