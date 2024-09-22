using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Y_TimerUI : MonoBehaviour
{
    public bool hasStart;
    public PlaySceneManager PSM;

    public TMP_Text timerText;
    public float elapsedTime = 0f;

    public float duration;

    public GameObject timerPanel;
    public GameObject resultPanel;

    public Y_UIManager uiManager;
    public WinnerCloseup win;
    CanvasRenderer[] canvasRenderers;
    public bool allReadyGo;

    void Start()
    {
        duration = 30;
    }

    void Update()
    {

        if(hasStart && allReadyGo)
        {
            elapsedTime += Time.deltaTime;

            // 경과 시간을 분:초 형식으로 변환
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            string timeText = string.Format("{0:00}:{1:00}",
                timeSpan.Minutes, timeSpan.Seconds);

            timerText.text = timeText;

            if (elapsedTime > duration)
            {
                timerPanel.SetActive(false); // 타이머 없애고
                resultPanel.SetActive(true); // 결과창 서서히 띄운다
                win.PerformWinnerCloseup();
                canvasRenderers = resultPanel.GetComponentsInChildren<CanvasRenderer>();
                StartCoroutine(uiManager.IncreaseAlpha(canvasRenderers));

                // 이 변수를 스쿼트/점핑잭 세는 스크립트에서 호출하고 해당 스크립트에서 더 이상 횟수 세지 못하게 한다
                hasStart = false; 
            }
        }
    }

    


}


// 클릭시 함수를 rpc로 쏘기