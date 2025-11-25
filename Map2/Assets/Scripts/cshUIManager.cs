using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cshUIManager : MonoBehaviour
{
    [SerializeField] public GameObject Player;
    public TextMeshProUGUI hptext;
    public TextMeshProUGUI PlayTimetext;
    public TextMeshProUGUI goldText;
    [SerializeField] public TextMeshProUGUI runningGazetxt;

    public Image hpImage;
    public Image runningGazeImage;

    float gameTime;

    [SerializeField] CshController controller;

    // --- GameOver ---
    public GameObject gameOverUI;
    private bool isGameOver = false;

    // --- InfoBase (¹öÆ°À¸·Î¸¸ ´ÝÈû) ---
    [SerializeField] public GameObject InfoBase;
    public float infoBaseStartTime = 0.5f;
    private bool infoBaseShown = false;
    private bool infoBaseClosed = false;

    // --- Info1 ---
    [SerializeField] public GameObject Info1;
    public float info1Delay = 1.0f;        // InfoBase ´ÝÈù ÈÄ Info1 ¶ß´Â µô·¹ÀÌ
    public float info1Duration = 5.0f;
    private bool info1Shown = false;
    private float info1StartTime = -1f;    // InfoBase ´ÝÈù ½ÃÁ¡À» ±â·Ï

  

    void Start()
    {
        gameTime = 0.0f;

        Player = GameObject.FindWithTag("Player");
        controller = Player.GetComponent<CshController>();
        
        if (InfoBase != null)
            InfoBase.SetActive(false);

        if (Info1 != null)
            Info1.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
   
    }


    void Update()
    {
        if (isGameOver)
            return;
        gameTime += Time.deltaTime;


        //       InfoBase Ç¥½Ã (ÀÚµ¿ ´ÝÈû ¾øÀ½)
        if (!infoBaseShown && gameTime >= infoBaseStartTime)
        {
            InfoBase.SetActive(true);
            infoBaseShown = true;
        }


        //     Info1 Ç¥½Ã (InfoBase ´ÝÈù ÈÄºÎÅÍ ½Ã°£ °è»ê)
        if (infoBaseClosed)
        {
            // InfoBase ²¨Áø ÈÄ µô·¹ÀÌ µÚ Info1 Ç¥½Ã
            if (!info1Shown && gameTime >= info1StartTime + info1Delay)
            {
                Info1.SetActive(true);
                info1Shown = true;
            }

            // InfoBase ´ÝÈù ÈÄºÎÅÍ info1Duration µÚ Info1 ÀÚµ¿ ´ÝÈû
            if (info1Shown && gameTime >= info1StartTime + info1Delay + info1Duration)
            {
                Info1.SetActive(false);
            }
        }


        //            UI °»½Å
        PlayTimetext.text = $"Time : {gameTime:F2}";

        if (controller != null)
        {
            hpImage.fillAmount = controller.HP / controller.maxHP;
            runningGazeImage.fillAmount = controller.currentRunningGaze / controller.maxRunningGaze;

            if (goldText != null)
                goldText.text = $"Point : {controller.gold}";
        }

        if (!isGameOver && controller.HP <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // ÇÃ·¹ÀÌ¾î ¿òÁ÷ÀÓ Á¤Áö
        if (Player != null)
        {
            CshController cont = Player.GetComponent<CshController>();
            if (cont != null)
                cont.enabled = false;
        }

        Debug.Log("Game Over triggered!");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void CloseInfoBase()
    {
        if (InfoBase != null)
        {
            InfoBase.SetActive(false);
            infoBaseClosed = true;

            info1StartTime = gameTime;
        }
    }

    public void CloseInfo1()
    {
        if (Info1 != null)
        {
            Info1.SetActive(false);
        }
    }
}
