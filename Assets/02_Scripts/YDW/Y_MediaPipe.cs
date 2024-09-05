﻿using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using System.Runtime.InteropServices.WindowsRuntime;


public class Y_MediaPipe : MonoBehaviour
{
    public UDPPoseHandler conn;

    public Vector3 currentScaleFactor = Vector3.one;
    Vector3 startSP;

    public float targetHeight = 0.78f;
    public float targetLegLength = 0.78f;
    float targetScaleBody;
    float targetScaleLeg;

    Vector3 standardPoint;
    Vector3 StartAndNowDiffLocation;
    public Transform startTrans;
    public Transform startDummyTrans;

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
    Vector3 getV3FromLandmark(int i)
    {
        Vector3 localPos = new Vector3(
                    conn.latestPoseList.landmarkList[i].x,
                    conn.latestPoseList.landmarkList[i].y,
                    conn.latestPoseList.landmarkList[i].z);
        localPos.z *= 0.3f;
        return localPos;
    }

    // 골반 사이 위치 잡아줌
    Vector3 getStandardPoint()
    {
        Vector3 vector23rd = getV3FromLandmark(23);
        Vector3 vector24th = getV3FromLandmark(24);
        Vector3 vectorFinal = (vector23rd + vector24th) * 0.5f;

        vectorFinal.z *= 0.3f;

        return vectorFinal;
    }

    // Start is called before the first frame update
    void Start()
    {
        conn = GameObject.Find("UDPConnector").GetComponent<UDPPoseHandler>();
        InitializeRigParts();
    }

    private void InitializeRigParts()
    {
        leftArmTarget = GameObject.Find("Rig_LeftArm_target");
        leftArmHint = GameObject.Find("Rig_LeftArm_hint");
        rightArmTarget = GameObject.Find("Rig_RightArm_target");
        rightArmHint = GameObject.Find("Rig_RightArm_hint");
        leftLegTarget = GameObject.Find("Rig_LeftLeg_target");
        leftLegHint = GameObject.Find("Rig_LeftLeg_hint");
        rightLegTarget = GameObject.Find("Rig_RightLeg_target");
        rightLegHint = GameObject.Find("Rig_RightLeg_hint");
        headAim1 = GameObject.Find("Aim");
        headAim2 = GameObject.Find("Aim2");
        spineTarget = GameObject.Find("Rig_Spine_target");
        spineHint = GameObject.Find("Rig_Spine_hint");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //처음에 시작할 때의 골반 사이 위치 저장
            startSP = getStandardPoint();
        }

        if (conn.latestPoseList.landmarkList.Count > 0)
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
        }
    }

    // a-b, c-d 벡터 사이의 거리
    public float GetFullHeight(int a, int b, int c, int d)
    {
        if (conn.latestPoseList.landmarkList.Count == 0)
            return 0f;

        Vector3 top = (getV3FromLandmark(a) + getV3FromLandmark(b)) * 0.5f;
        Vector3 bottom = (getV3FromLandmark(c) + getV3FromLandmark(d)) * 0.5f;

        return Vector3.Distance(top, bottom);
    }

    #endregion

    #region Rigging

    void UpdateRigPosition()
    {

        // 각자 IK 에 따라 움직이게 하기
        headAim1.transform.position = Vector3.Lerp(headAim1.transform.position, UpdateRigPart(9), 0.1f);
        headAim2.transform.position = Vector3.Lerp(headAim2.transform.position, UpdateRigPart(10), 0.1f);
        leftArmHint.transform.position = Vector3.Lerp(leftArmHint.transform.position, UpdateRigPart(13), 0.1f);
        rightArmHint.transform.position = Vector3.Lerp(rightArmHint.transform.position, UpdateRigPart(14), 0.1f);
        leftArmTarget.transform.position = Vector3.Lerp(leftArmTarget.transform.position, UpdateRigPart(15), 0.1f);
        rightArmTarget.transform.position = Vector3.Lerp(rightArmTarget.transform.position, UpdateRigPart(16), 0.1f);
        leftLegHint.transform.position = Vector3.Lerp(leftLegHint.transform.position, UpdateRigPart(25), 0.1f);
        rightLegHint.transform.position = Vector3.Lerp(rightLegHint.transform.position, UpdateRigPart(26), 0.1f);
        leftLegTarget.transform.position = Vector3.Lerp(leftLegTarget.transform.position, UpdateRigPart(27), 0.1f);
        rightLegTarget.transform.position = Vector3.Lerp(rightLegTarget.transform.position, UpdateRigPart(28), 0.1f);
        spineTarget.transform.position = Vector3.Lerp(spineTarget.transform.position, (UpdateRigPart(11) + UpdateRigPart(12)) * 0.5f, 0.1f);
        spineHint.transform.position = Vector3.Lerp(spineHint.transform.position, (UpdateRigPart(24) + UpdateRigPart(23)) * 0.5f, 0.1f);

        // 최종 위치 보정
        Vector3 newPosition = transform.position - (transform.up * 0.2f) + StartAndNowDiffLocation;  // 새로운 위치 계산 : 현 위치(허리)에서, 조금 밑에서 (골반), 처음과의 달라진 위치를 더한다
        newPosition.y = Mathf.Clamp(newPosition.y, 3f, 3.8f);  // y 값 클램프 적용 -> 땅으로 꺼지지 않게
        startTrans.position = newPosition;

        // 내려갈 때는 다리먼저 그 다음에 허리
        // 올라올 때는 허리 먼저 그 다음에 다리
    }


    Vector3 UpdateRigPart(int i)
    {
        standardPoint = getStandardPoint();

        Vector3 localPos = getV3FromLandmark(i);

        localPos = (localPos - standardPoint); // 골반 사이를 기준으로 하는 로컬 좌표

        localPos = new Vector3(localPos.x, -localPos.y, localPos.z); // y 축 좌표 반전




        // 스케일 팩터 구하기

        // 1. 무릎 좌표일 경우
        if (i == 25 || i == 26)
        {
            if (localPos.y < 3.15) // 앉아 있을 때는 유닛벡터를 기준으로 한다
            {
                currentScaleFactor = Vector3.one;
            }
            else // 아닐 때는 다리용 스케일 팩터 줌
            {
                currentScaleFactor = Vector3.one * targetScaleLeg;
            }
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

            StartAndNowDiffLocation = Vector3.Scale(StartAndNowDiffLocation, currentScaleFactor); // 스케일 팩터 보정

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



    #endregion
}
