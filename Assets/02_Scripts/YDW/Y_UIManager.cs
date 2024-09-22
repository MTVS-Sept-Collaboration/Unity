using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.UI;

public class Y_UIManager : MonoBehaviour
{

    float canvasAlphaTime = 0;

    public GameObject chooseWorkOut;
    public Y_CountJumpingJack cntJumpingJack;
    public Y_CountSquatt cntSquat;
    public GameObject chooseWorkOutCanvas;
    public PlaySceneManager PSM;
    

    public GameObject LoadingPanel;
    public GameObject LoadingPanelNotMaster;

    bool isnextPanel;
    bool endSelect;

    public GameObject BeforeStartUI;


    GameObject playerScreenCanvas;
    

    void Start()
    {
        
    }

    void Update()
    {
        if (!isnextPanel && PSM.myPlayer != null && PSM.myPlayer.GetComponent<JSWPhotonVoiceTest>().isMaster)
        {
            chooseWorkOut.SetActive(true);
            isnextPanel = true;
        }
        if (PSM.myPlayer != null && PSM.myPlayer.GetComponent<JSWPhotonVoiceTest>().AllplayerInRoom)
        {
            LoadingPanel.SetActive(false);
            if (!PSM.myPlayer.GetComponent<JSWPhotonVoiceTest>().isMaster && !endSelect)
            {
                LoadingPanelNotMaster.SetActive(true);
            }
            else
            {
                LoadingPanelNotMaster.SetActive(false);
            }
        }
    }

    CanvasRenderer[] canvasRenderers;

    //public void CountDownUpdateCoroutine()
    //{
        
    //    PSM.myPlayer.GetComponent<JSWPhotonVoiceTest>().AllReadyGO_RPC();
    //}
    //IEnumerator CountDownUpdate()
    //{
    //    while (true)
    //    {

    //    }
    //}

    public IEnumerator decreaseAlpha(CanvasRenderer[] canvasRenderers)
    {
        while(true)
        {
            canvasAlphaTime += Time.deltaTime;
            if(canvasAlphaTime > 1)
            {
                canvasAlphaTime = 0;
                break;
            }

            foreach (CanvasRenderer canvasRenderer in canvasRenderers)
            {
                Color originalColor = canvasRenderer.GetColor();
                originalColor.a = 1 - canvasAlphaTime;
                canvasRenderer.SetColor(originalColor);
            }

            yield return null;
        }
    }

    public IEnumerator IncreaseAlpha(CanvasRenderer[] canvasRenderers)
    {
        while (true)
        {
            canvasAlphaTime += Time.deltaTime;
            if (canvasAlphaTime > 1)
            {
                canvasAlphaTime = 0;
                break;
            }

            foreach (CanvasRenderer canvasRenderer in canvasRenderers)
            {
                Color originalColor = canvasRenderer.GetColor();
                originalColor.a = canvasAlphaTime;
                canvasRenderer.SetColor(originalColor);
            }

            yield return null;
        }
    }


    // 스쿼트 / 팔벌려뛰기 버튼 선택 시 호출되는 함수들
    public void SelectSquat()
    {
        PSM.myPlayer.GetComponent<JSWPhotonVoiceTest>().ChooseSquateOrJump_RPC(1);
    }
    public void SelectSquat2()
    {
        endSelect = true;
        cntSquat = PSM.myPlayer.GetComponent<Y_CountSquatt>();
        cntSquat.enabled = true;
        playerScreenCanvas = PSM.myPlayer.transform.GetChild(3).gameObject;
        playerScreenCanvas.GetComponent<Y_PlayerScreenUIManager>().isSelected = true;
        canvasRenderers = chooseWorkOutCanvas.GetComponentsInChildren<CanvasRenderer>();
        StartCoroutine(decreaseAlpha(canvasRenderers));
        StartCoroutine(deactiveChooseWorkOut());
    }
    public void SelectJumpingJack()
    {
        PSM.myPlayer.GetComponent<JSWPhotonVoiceTest>().ChooseSquateOrJump_RPC(2);
    }

    public void SelectJumpingJack2()
    {
        endSelect = true;
        cntJumpingJack = PSM.myPlayer.GetComponent<Y_CountJumpingJack>();
        cntJumpingJack.enabled = true;
        playerScreenCanvas = PSM.myPlayer.transform.GetChild(3).gameObject;
        playerScreenCanvas.GetComponent<Y_PlayerScreenUIManager>().isSelected = true;
        canvasRenderers = chooseWorkOutCanvas.GetComponentsInChildren<CanvasRenderer>();
        StartCoroutine(decreaseAlpha(canvasRenderers));
        StartCoroutine(deactiveChooseWorkOut());
    }

    IEnumerator deactiveChooseWorkOut()
    {
        yield return new WaitForSeconds(1f);
        chooseWorkOut.SetActive(false);
    }

}
