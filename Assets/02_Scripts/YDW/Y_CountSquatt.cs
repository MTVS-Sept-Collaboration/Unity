using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;

public class Y_CountSquatt : MonoBehaviour, IPunObservable
{
    public bool startGame;
    public bool isSquatting;
    public float squatCount;
    PhotonView pv;
    Y_MediaPipeTest mediapipe;
    Transform pelvisPos;

    float startPelvisPos;

    public Y_TimerUI timerUI;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
             stream.SendNext(squatCount);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면...
        else if (stream.IsReading)
        {
            if (stream.Count > 0)
            {
                object receivedValue = stream.ReceiveNext();
                if (receivedValue is float)
                {
                    squatCount = (float)receivedValue;
                }
                else
                {
                    Debug.LogWarning("Received unexpected data type for squatCount");
                }
            }
            else
            {
                Debug.LogWarning("Received empty stream in OnPhotonSerializeView");
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        isSquatting = false;
        squatCount = -1;
        mediapipe = GetComponent<Y_MediaPipeTest>();
        pelvisPos = mediapipe.spineTrans;
        pv = GetComponent<PhotonView>();
        timerUI = GameObject.Find("Canvas").GetComponent<Y_TimerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (startGame && timerUI.hasStart)
        {
            startPelvisPos = mediapipe.startSP.y;
            /////////////////
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                squatCount++;
            }
            ////////////////

            //print("y 좌표 : " + (pelvisPos.position.y - startPelvisPos));
            if (pelvisPos.position.y - startPelvisPos < 3.58f && !isSquatting) // 3.35
            {
                    squatCount++;
                    isSquatting = true;
                    JSWSoundManager.Get().PlayEftSound(JSWSoundManager.ESoundType.EFT_GETSCORE);
            }
        
            if(pelvisPos.position.y - startPelvisPos > 3.67f && isSquatting) // 3.45
            {
                    isSquatting = false;
            }

            //Debug.LogError("스쿼트 개수 : " + squatCount);
        }
    }
}
