using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using static UnityEngine.GraphicsBuffer;
using Photon.Pun;
using Photon.Realtime;



public class Y_MediaPipeTest : MonoBehaviour, IPunObservable
{
    public UDPPoseHandler conn;

    public Vector3 currentScaleFactor = Vector3.one;
    public Vector3 startSP;

    public float targetHeight = 0.78f;
    public float targetLegLength = 0.52f;
    float targetScaleBody;
    float targetScaleLeg;

    Vector3 standardPoint;
    Vector3 StartAndNowDiffLocation;

    public Transform spineTrans;
    public Transform spineDummyTrans;
    public Transform lToeTrans;
    public Transform rToeTrans;
    public Transform lHandTrans;
    public Transform rHandTrans;

    public GameObject ground;
    float groundLevel = 0f;

    //Y_CountSquatt countSquatt;
    //Y_CountJumpingJack countJumpingJack;

    Vector3[] PD = new Vector3[33];
    PhotonView pv;

    public GameObject[] cubes; // 관절 따라 다니게 해 보면서 정확도 맞추자

    // Rigging
    public GameObject
        leftArmTarget,
        leftArmHint,
        rightArmTarget,
        rightArmHint,
        leftLegTarget,
        leftLegHint,
        rightLegTarget,
        rightLegHint,
        headAim1,
        headAim2,
        spineTarget,
        spineHint;


    // conn 의 랜드마크리스트에서 특정 인덱스를 이용, 벡터로 만들어서 가져온다
    public Vector3 getV3FromLandmark(int i)
    {
        Vector3 localPos = Vector3.zero;

        if (conn.latestPoseList.Count > 0)
        {
            if (pv.IsMine)
            {
                localPos = new Vector3(
                        conn.latestPoseList[i].x,
                        conn.latestPoseList[i].y,
                        conn.latestPoseList[i].z);
            }
            else
            {
                localPos = PD[i];
            }

            localPos.z *= 0.3f;
        }
        return localPos;
    }

    // 골반 사이 위치 잡아줌
    public Vector3 getStandardPoint()
    {
        Vector3 vector23rd = getV3FromLandmark(23);
        Vector3 vector24th = getV3FromLandmark(24);
        Vector3 vectorFinal = (vector23rd + vector24th) * 0.5f;

        vectorFinal.z *= 0.3f;

        return vectorFinal;
    }

    private void Awake()
    {
        InitializeRigParts();
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        conn = GameObject.Find("UDPConnector").GetComponent<UDPPoseHandler>();
        ground = GameObject.Find("Ground_01");
        groundLevel = ground.transform.position.y;
        //print("groundLevel: " + groundLevel); // 2.655265
    }

    private void InitializeRigParts()
    {
        //leftArmTarget = GameObject.Find("Rig_LeftArm_target");
        //leftArmHint = GameObject.Find("Rig_LeftArm_hint");
        //rightArmTarget = GameObject.Find("Rig_RightArm_target");
        //rightArmHint = GameObject.Find("Rig_RightArm_hint");
        //leftLegTarget = GameObject.Find("Rig_LeftLeg_target");
        //leftLegHint = GameObject.Find("Rig_LeftLeg_hint");
        //rightLegTarget = GameObject.Find("Rig_RightLeg_target");
        //rightLegHint = GameObject.Find("Rig_RightLeg_hint");
        //headAim1 = GameObject.Find("Aim1");
        //headAim2 = GameObject.Find("Aim2");
        //spineTarget = GameObject.Find("Rig_Spine_target");
        //spineHint = GameObject.Find("Rig_Spine_hint");



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //Q를 눌렀을 때의 골반 사이 위치 저장
            startSP = getStandardPoint();
        }

        if (conn.latestPoseList.Count > 0)
        {
            UpdateScaleFactor();
            UpdateScaleFactorLeg();
            UpdateRigPosition();
        }
    }

    #region ScaleFactor
    // 몸통의 스케일 팩터 구하기
    void UpdateScaleFactor()
    {
        // 몸통 길이 구해서 씬에서의 몸통 길이를 나눈다
        float fullHeight = GetFullHeight(11, 12, 23, 24);
        if (fullHeight > 0)
        {
            targetScaleBody = targetHeight / fullHeight;
        }
    }

    // 하체의 스케일 팩터 구하기
    void UpdateScaleFactorLeg()
    {
        // 다리 길이 구해서 씬에서의 다리 길이를 나눈다
        float fullHeight = GetFullHeight(23, 24, 27, 28);
        if (fullHeight > 0)
        {
            targetScaleLeg = targetLegLength / fullHeight;
            targetScaleLeg = 0.8f;
        }
    }

    // a-b, c-d 벡터 사이의 거리
    public float GetFullHeight(int a, int b, int c, int d)
    {
        if (conn.latestPoseList.Count == 0)
            return 0f;

        Vector3 top = (getV3FromLandmark(a) + getV3FromLandmark(b)) * 0.5f;
        Vector3 bottom = (getV3FromLandmark(c) + getV3FromLandmark(d)) * 0.5f;

        return Vector3.Distance(top, bottom);
    }

    #endregion

    #region Rigging

    Vector3 spinePos;

    void UpdateRigPosition()
    {

        // 각자 IK 에 따라 움직이게 하기

        // 머리
        headAim1.transform.position = Vector3.Lerp(headAim1.transform.position, UpdateRigPart(9), 0.1f);
        headAim2.transform.position = Vector3.Lerp(headAim2.transform.position, UpdateRigPart(10), 0.1f);

        // 팔
        leftArmHint.transform.position = Vector3.Lerp(leftArmHint.transform.position, UpdateRigPart(13), 0.1f);
        rightArmHint.transform.position = Vector3.Lerp(rightArmHint.transform.position, UpdateRigPart(14), 0.1f);
        leftArmTarget.transform.position = Vector3.Lerp(leftArmTarget.transform.position, UpdateRigPart(15), 0.1f);
        rightArmTarget.transform.position = Vector3.Lerp(rightArmTarget.transform.position, UpdateRigPart(16), 0.1f);

        // 다리
        leftLegHint.transform.position = Vector3.Lerp(leftLegHint.transform.position, UpdateRigPart(25), 0.05f);
        rightLegHint.transform.position = Vector3.Lerp(rightLegHint.transform.position, UpdateRigPart(26), 0.05f);
        leftLegTarget.transform.position = Vector3.Lerp(leftLegTarget.transform.position, UpdateRigPart(27), 0.1f);
        //다리가 자꾸 꺼지니까, 다리 위치를 그라운드 레벨과 맞춰준다
        leftLegTarget.transform.position = new Vector3(
            leftLegTarget.transform.position.x,
            Mathf.Max(UpdateRigPart(27).y, groundLevel),  // 지면 레벨보다 아래로 내려가지 않도록
            leftLegTarget.transform.position.z
        );
        rightLegTarget.transform.position = Vector3.Lerp(rightLegTarget.transform.position, UpdateRigPart(28), 0.1f);
        //다리가 자꾸 꺼지니까, 다리 위치를 그라운드 레벨과 맞춰준다
        rightLegTarget.transform.position = new Vector3(
            rightLegTarget.transform.position.x,
            Mathf.Max(UpdateRigPart(28).y, groundLevel),  // 지면 레벨보다 아래로 내려가지 않도록
            rightLegTarget.transform.position.z
        );

        // 몸통
        spineTarget.transform.position = Vector3.Lerp(spineTarget.transform.position, (UpdateRigPart(11) + UpdateRigPart(12)) * 0.5f, 0.1f);
        spineHint.transform.position = Vector3.Lerp(spineHint.transform.position, (UpdateRigPart(24) + UpdateRigPart(23)) * 0.5f, 0.1f);


        // 왼발 위치와 각도 보정
        lToeTrans.position = UpdateRigPart(31) - UpdateRigPart(27);
        Vector3 rightVectorL = Vector3.Cross((UpdateRigPart(31) - UpdateRigPart(27)).normalized, (UpdateRigPart(27) - UpdateRigPart(25)).normalized);
        Vector3 forwardVectorL = Vector3.Cross(rightVectorL, (UpdateRigPart(31) - UpdateRigPart(27)).normalized);
        Quaternion rotationVectorL = Quaternion.LookRotation(forwardVectorL, (UpdateRigPart(31) - UpdateRigPart(27)).normalized);
        leftLegTarget.transform.rotation = Quaternion.Lerp(leftLegTarget.transform.rotation, rotationVectorL, 0.1f);

        // 오른발 위치와 각도 보정
        rToeTrans.position = UpdateRigPart(32) - UpdateRigPart(28);
        Vector3 rightVectorR = Vector3.Cross((UpdateRigPart(32) - UpdateRigPart(28)).normalized, (UpdateRigPart(28) - UpdateRigPart(26)).normalized);
        Vector3 forwardVectorR = Vector3.Cross(rightVectorR, (UpdateRigPart(32) - UpdateRigPart(28)).normalized);
        Quaternion rotationVectorR = Quaternion.LookRotation(forwardVectorR, (UpdateRigPart(32) - UpdateRigPart(28)).normalized);
        rightLegTarget.transform.rotation = Quaternion.Lerp(rightLegTarget.transform.rotation, rotationVectorR, 0.1f);

        // 척추 위치 보정
        spinePos = transform.position + StartAndNowDiffLocation; // 새로운 위치 계산 : 현 위치(허리)에서 처음과의 달라진 위치를 더한다
        spineTrans.position = spinePos; // 척추 위치를 강제로 옮겨준다 (애니메이터가 못 움직이게 막아놓고 있었으므로)

        // 왼손 위치 보정
        // 15번에서 19번/17번 사이 바라보는 벡터를 손목의 업벡터랑 맞추고
        lHandTrans.position = (UpdateRigPart(19) + UpdateRigPart(17)) * 0.5f - UpdateRigPart(15);
        // 외적해서 forward 벡터 구해서
        Vector3 UpVectorHL = Vector3.Cross((((UpdateRigPart(19) + UpdateRigPart(17)) * 0.5f - UpdateRigPart(15)).normalized), (UpdateRigPart(15) - UpdateRigPart(13)).normalized);
        Vector3 forwardVectorHL = Vector3.Cross(UpVectorHL, ((UpdateRigPart(19) + UpdateRigPart(17)) * 0.5f - UpdateRigPart(15)).normalized);
        Quaternion rotationVectorHL = Quaternion.LookRotation(forwardVectorHL, (UpdateRigPart(19) + UpdateRigPart(17)) * 0.5f - UpdateRigPart(15));
        // 똑같이 Lerp 값 주면
        leftArmTarget.transform.rotation = Quaternion.Lerp(leftArmTarget.transform.rotation, rotationVectorHL, 0.1f);

        // 오른손 위치 보정
        // 16번에서 20번/18번 사이 바라보는 벡터를 손목의 업벡터랑 맞추고
        rHandTrans.position = (UpdateRigPart(20) + UpdateRigPart(18)) * 0.5f - UpdateRigPart(16);
        // 외적해서 forward 벡터 구해서
        Vector3 UpVectorHR = Vector3.Cross((((UpdateRigPart(20) + UpdateRigPart(18)) * 0.5f - UpdateRigPart(16)).normalized), (UpdateRigPart(16) - UpdateRigPart(14)).normalized);
        Vector3 forwardVectorHR = Vector3.Cross(UpVectorHR, ((UpdateRigPart(20) + UpdateRigPart(18)) * 0.5f - UpdateRigPart(16)).normalized);
        Quaternion rotationVectorHR = Quaternion.LookRotation(forwardVectorHR, (UpdateRigPart(20) + UpdateRigPart(18)) * 0.5f - UpdateRigPart(16));
        // 똑같이 Lerp 값 주면
        rightArmTarget.transform.rotation = Quaternion.Lerp(rightArmTarget.transform.rotation, rotationVectorHR, 0.1f);


        // 내려갈 때는 다리먼저 그 다음에 허리
        // 올라올 때는 허리 먼저 그 다음에 다리
    }


    Vector3 UpdateRigPart(int i)
    {
        standardPoint = getStandardPoint();

        Vector3 localPos = getV3FromLandmark(i);

        localPos = (localPos - standardPoint); // 골반 사이를 기준으로 하는 로컬 좌표

        localPos = new Vector3(localPos.x, -localPos.y, localPos.z); // y 축 좌표 반전


        // 1. 무릎 좌표일 경우
        if (i >= 25 || i == 26) 
        {
            if (localPos.y > -0.25f) // 앉아 있을 때는 유닛벡터를 기준으로 한다
            {
                currentScaleFactor = Vector3.one;
            }
            else // 아닐 때는 다리용 스케일 팩터 줌
            {
                currentScaleFactor = Vector3.one * targetScaleLeg;
            }

            //임시
            currentScaleFactor = Vector3.one * targetScaleBody * targetScaleLeg * 0.7f;

            currentScaleFactor.x = currentScaleFactor.x * (1f / 0.7f);

            currentScaleFactor.z = currentScaleFactor.z * 0.4f;
        }
        else // 2. 그 외는 몸통 스케일 팩터를 이용한다
        {
            currentScaleFactor = Vector3.one * targetScaleBody;
        }

        // 처음과 지금과의 위치 차이를 구한다
        if (i == 23 || i == 24) // 골반을 기준으로
        {
            StartAndNowDiffLocation = standardPoint - startSP;

            StartAndNowDiffLocation.y = -StartAndNowDiffLocation.y; // y 값 반전

            StartAndNowDiffLocation = transform.TransformDirection(StartAndNowDiffLocation); // 월드 좌표로 변환
        }

        Vector3 finalVector = transform.TransformPoint(Vector3.Scale(localPos, currentScaleFactor)); // 스케일 보정하고 월드 좌표로 변환

        // 9, 10 번의 경우 살짝 밑으로 내려준다 (리깅 위치가 입이 아닌 목 쪽임)
        if (i == 9 || i == 10)
        {
            finalVector = finalVector - (transform.up * 0.05f);
        }

        return finalVector;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new NotImplementedException();
        if (stream.IsWriting)
        {

            // iterable 데이터를 보낸다.
            for (int i = 0; i < 33; i++)
            {

                stream.SendNext(new Vector3(
                    conn.latestPoseList[i].x,
                    conn.latestPoseList[i].y,
                    conn.latestPoseList[i].z));
            }



        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면...
        else if (stream.IsReading)
        {

            for (int i = 0; i < 33; i++)
            {
                PD[i] = (Vector3)stream.ReceiveNext();
            }

        }
    }



    #endregion

}
