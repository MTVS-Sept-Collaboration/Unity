﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Y_CountSquatt : MonoBehaviour, IPunObservable
{
    public bool startGame;
    public bool isSquatting;
    public float squatCount;
    public float squatCountOther;
    PhotonView pv;
    Y_MediaPipeTest mediapipe;
    Transform pelvisPos;
    Transform leftHandPos;
    Transform rightHandPos;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
             stream.SendNext(squatCount);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면...
        else if (stream.IsReading)
        {
                squatCountOther = (float)stream.ReceiveNext();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        isSquatting = false;
        squatCount = 0;
        mediapipe = GetComponent<Y_MediaPipeTest>();
        pelvisPos = mediapipe.spineTrans;
        pv = GetComponent<PhotonView>();
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
            
            //print("y 좌표 : " + pelvisPos.position.y);
            if (pelvisPos.position.y < 3.35f && !isSquatting) // 3.25
            {
                if (pv.IsMine)
                {
                    squatCount++;
                    isSquatting = true;
                }                
            }
        
            if(pelvisPos.position.y > 3.45f && isSquatting) // 3.35
            {
                if (pv.IsMine)
                {
                    isSquatting = false;
                }
            }
            if (!pv.IsMine)
            {
                squatCount = squatCountOther;
            }
        }
    }
}
