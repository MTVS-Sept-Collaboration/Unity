using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Y_PlayerUIManager : MonoBehaviour
{
    Y_CountSquatt cntSquatScript;
    Y_CountJumpingJack cntJumpingJackScript;
    Y_SquatCntUI squatCntUIScript;
    Y_JumpingJackCntUI jumpingJackCntUIScript;

    float prevSquatNum = 0;
    float squatNum = 0;
    float prevJumpingJackNum = 0;
    float jumpingJackNum;
    public TMP_Text squatCntUI;
    public TMP_Text jumpingJackCntUI;

    public GameObject squatCntUIgo;
    public bool squatReset = false;
    public GameObject jumpingJackCntUIgo;
    public bool jumpingJackReset = false;


    // Start is called before the first frame update
    void LateStart()
    {
        cntSquatScript = GameObject.Find("Player").GetComponent<Y_CountSquatt>();
        cntJumpingJackScript = GameObject.Find("Player").GetComponent<Y_CountJumpingJack>();
        squatCntUIScript = GetComponentInChildren<Y_SquatCntUI>();
        jumpingJackCntUIScript = GetComponentInChildren<Y_JumpingJackCntUI>();

    }

    // Update is called once per frame
    void Update()
    {
        squatNum = cntSquatScript.squatCount;
        if (prevSquatNum < squatNum)
        {
            squatCntUIgo.SetActive(true);
            squatCntUI.text = squatNum.ToString();
            squatReset = true;
            prevSquatNum = squatNum;
        }

        jumpingJackNum = cntJumpingJackScript.jumpingJackCount;
        if (prevJumpingJackNum < jumpingJackNum)
        {
            jumpingJackCntUIgo.SetActive(true);
            jumpingJackCntUI.text = jumpingJackNum.ToString();
            jumpingJackReset = true;
            prevJumpingJackNum = jumpingJackNum;
        }
    }




}
