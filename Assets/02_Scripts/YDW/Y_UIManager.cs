using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Y_UIManager : MonoBehaviour
{

    float canvasAlphaTime = 0;

    public GameObject chooseWorkOut;
    public Y_CountJumpingJack cntJumpingJack;
    public Y_CountSquatt cntSquat;
    public GameObject chooseWorkOutCanvas;
    GameObject playerScreenCanvas;
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    CanvasRenderer[] canvasRenderers;

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


    // 스쿼트 / 팔벌려뛰기 버튼 선택 시 호출되는 함수들
    public void SelectSquat()
    {
        cntSquat.enabled = true;
        playerScreenCanvas = GameObject.Find("PlayerScreenCanvas");
        playerScreenCanvas.GetComponent<Y_PlayerScreenUIManager>().isSelected = true;
        canvasRenderers = chooseWorkOutCanvas.GetComponentsInChildren<CanvasRenderer>();
        StartCoroutine(decreaseAlpha(canvasRenderers));
        StartCoroutine(deactiveChooseWorkOut());
    }

    public void SelectJumpingJack()
    {
        cntJumpingJack.enabled = true;
        playerScreenCanvas = GameObject.Find("PlayerScreenCanvas");
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
