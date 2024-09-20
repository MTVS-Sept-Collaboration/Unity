using Google.Protobuf.WellKnownTypes;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Y_PlayerScreenUIManager : MonoBehaviour
{
    Y_SetStandardPos setStandardPos;
    public Y_UIManager uiManager;
    public GameObject directionCanvas;
    public GameObject countdownText;

    public bool isSelected = false;
    public bool canActive = true;
    float countdownTime;
    public int showTime;
    float currTime = 0;
    float duration = 1;

    public TMP_Text PoseRecCountdown;

    bool startWorkOut = false;

    // Start is called before the first frame update
    void LateStart()
    {
        setStandardPos = GameObject.Find("Player").GetComponent<Y_SetStandardPos>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected)
        {
            directionCanvas.SetActive(true);
            countdownTime = setStandardPos.duration;
            showTime = (int)(6 - countdownTime);
            if (canActive) Countdown();
            currTime += Time.deltaTime;
            if (currTime > duration)
            {
                currTime = 0;
            }
        }

        if(startWorkOut)
        {

            startWorkOut = false;
        }
    }

    CanvasRenderer[] canvasRenderers;
    void Countdown()
    {
        if (showTime <= 0)
        {
            PoseRecCountdown.text = "시작!";
            canvasRenderers = directionCanvas.GetComponentsInChildren<CanvasRenderer>();
            StartCoroutine(uiManager.decreaseAlpha(canvasRenderers)); // 투명도 조절 후
            directionCanvas.SetActive(false); // UI 비활성화
            canActive = false;
            isSelected = false;
            startWorkOut = true;
        }
        else if (showTime < 6)
        {
            countdownText.SetActive(true);
            PoseRecCountdown.text = showTime.ToString();
        }
    }
}
