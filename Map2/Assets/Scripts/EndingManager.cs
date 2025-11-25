using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    [Header("Player Reference")]
    public GameObject Player;

    [Header("UI")]
    public GameObject endingPanel;    // 엔딩 패널 전체
    public TMP_Text endingText;       // 타자기 효과 텍스트

    public GameObject successImage;
    public GameObject failImage;

    [Header("Effects")]
    public float typingSpeed = 0.05f; // 타자기 효과 속도
    public float popScale = 1.3f;      // 쾅! 효과 크기
    public float popDuration = 0.25f;  // 쾅! 효과 시간

    private bool endingStarted = false;

    private void Start()
    {
        if (endingPanel != null) endingPanel.SetActive(false);
        if (successImage != null) successImage.SetActive(false);
        if (failImage != null) failImage.SetActive(false);

        if (Player == null)
            Player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !endingStarted)
        {
            StartEnding();
        }
    }

    // 외부 트리거에서 호출
    public void OpenQuestWindow()
    {
        if (endingStarted) return;
        StartEnding();
    }

    private void StartEnding()
    {
        endingStarted = true;
        endingPanel.SetActive(true);

        int gold = Player.GetComponent<CshController>().gold;
        string name = Player.GetComponent<CshController>().name;
        int grade = Player.GetComponent<CshController>().grade;

        string message = $"이름 : {name}\n학년 : {grade}\n비교과포인트 : {gold}\n";

        // 텍스트 출력 후 이미지를 보여주는 코루틴 실행
        StartCoroutine(ShowEndingAfterText(message, gold >= 800));
    }

    // 타자기 효과
    IEnumerator TypeWriterEffect(string fullText)
    {
        endingText.text = "";

        foreach (char c in fullText)
        {
            endingText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator ShowEndingAfterText(string message, bool isSuccess)
    {
        // 1️ 텍스트 타자기 출력
        yield return StartCoroutine(TypeWriterEffect(message));

        // 2️ 텍스트 끝난 후 Success / Fail 이미지 활성화
        if (isSuccess)
        {
            successImage.SetActive(true);
            failImage.SetActive(false);

            // 이미지 효과 (예: 흔들림)
            StartCoroutine(ShakeEffect(successImage));
        }
        else
        {
            successImage.SetActive(false);
            failImage.SetActive(true);

            StartCoroutine(ShakeEffect(failImage));
        }
    }
    IEnumerator ShakeEffect(GameObject obj, float duration = 0.25f, float strength = 10f)
    {
        Vector3 originalPos = obj.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-strength, strength) * 0.01f;
            float y = Random.Range(-strength, strength) * 0.01f;

            obj.transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.localPosition = originalPos;
    }

    // Accept 버튼 → Scene 재시작
    public void OnClickAccept()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Decline 버튼 → 게임 종료
    public void OnClickDecline()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
