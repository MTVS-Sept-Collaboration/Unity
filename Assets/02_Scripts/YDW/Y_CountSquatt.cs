﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Y_CountSquatt : MonoBehaviour
{
    public bool startGame;
    public bool isSquatting;
    public float squatCount;
    Y_MediaPipeTest mediapipe;
    Transform pelvisPos;
    Transform leftHandPos;
    Transform rightHandPos;


    // Start is called before the first frame update
    void Start()
    {
        isSquatting = false;
        squatCount = 0;
        mediapipe = GetComponent<Y_MediaPipeTest>();
        pelvisPos = mediapipe.spineTrans;
    }

    // Update is called once per frame
    void Update()
    {
        if(startGame)
        {

            /////////////////
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                squatCount++;
            }

            ////////////////
            ///
            //print("y 좌표 : " + pelvisPos.position.y);
            if (pelvisPos.position.y < 3.35f && !isSquatting) // 3.25
            {
                squatCount++;
                isSquatting = true;
            }
        
            if(pelvisPos.position.y > 3.45f && isSquatting) // 3.35
            {
                isSquatting = false;
            }

            


            print("스쿼트 횟수: " + squatCount);

        }
    }
}
