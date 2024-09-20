using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_CountJumpingJack : MonoBehaviour, IPunObservable
{
    public bool startGame;
    public bool isJumpingJack;
    public float jumpingJackCount;
    PhotonView pv;
    Y_MediaPipeTest mediapipe;
    Transform leftHandPos;
    Transform rightHandPos;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(jumpingJackCount);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면...
        else if (stream.IsReading)
        {
            jumpingJackCount = (float)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isJumpingJack = false;
        jumpingJackCount = 0;
        mediapipe = GetComponent<Y_MediaPipeTest>();
        leftHandPos = mediapipe.leftArmTarget.transform;
        rightHandPos = mediapipe.rightArmTarget.transform;
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startGame)
        {
            /////////////////////
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                jumpingJackCount++;
            }
            /////////////////////

            //print("손 좌표 : " + leftHandPos.position.y + " 오른손도!! : " + rightHandPos.position.y);
            if (leftHandPos.position.y > 4.8f && rightHandPos.position.y > 4.8f && !isJumpingJack)
            {
                if (pv.IsMine)
                {
                    jumpingJackCount++;
                    isJumpingJack = true;
                }
                //print("!!!!!!!! 손 좌표 : " + leftHandPos.position.y + " 오른손도!! : " + rightHandPos.position.y);
            }

            if (leftHandPos.position.y < 3.7f && rightHandPos.position.y < 3.7f && isJumpingJack)
            {
                if (pv.IsMine)
                {
                    isJumpingJack = false;
                    //print("?????????/ 손 좌표 : " + leftHandPos.position.y + " 오른손도!! : " + rightHandPos.position.y);
                }
            }

            print("점핑잭 횟수: " + jumpingJackCount);

        }
    }
}
