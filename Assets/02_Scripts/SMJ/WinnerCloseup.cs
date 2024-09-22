using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerCloseup : MonoBehaviour
{
    public Camera mainCamera;
    public float closeUpDistance = 2f;
    public float moveTime = 1f;
    public float leftOffset = 0.5f; // 왼쪽으로의 오프셋 (0.5는 예시 값입니다)

    private Y_RankUI rankUI;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            Debug.Log("Main camera assigned: " + (mainCamera != null));
        }

        rankUI = FindObjectOfType<Y_RankUI>();
        if (rankUI == null)
        {
            Debug.LogError("Y_RankUI not found in the scene!");
        }
    }

    public void PerformWinnerCloseup()
    {
        Debug.Log("PerformWinnerCloseup called");

        rankUI.CalculateRanking();

        Transform winnerTransform = FindWinnerTransform();

        if (winnerTransform != null)
        {
            Debug.Log("Winner found: " + winnerTransform.name);
            StartCoroutine(SmoothCloseUp(winnerTransform));
        }
        else
        {
            Debug.LogError("Winner transform not found!");
        }
    }

    private System.Collections.IEnumerator SmoothCloseUp(Transform target)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        // 카메라 위치 계산: 타겟의 뒤쪽에 위치
        Vector3 targetPosition = target.position - target.forward * closeUpDistance;

        // 카메라가 바라볼 지점 계산: 타겟의 약간 왼쪽
        Vector3 lookAtPoint = target.position + (-target.right * leftOffset);

        // 타겟의 왼쪽을 바라보는 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - targetPosition);

        Debug.Log("Starting closeup. Start pos: " + startPosition + ", Target pos: " + targetPosition);

        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;

        Debug.Log("Closeup completed. Final pos: " + mainCamera.transform.position);
    }

    private Transform FindWinnerTransform()
    {
        string winnerName = rankUI.name1st.text;
        Debug.Log("Searching for winner with name: " + winnerName);

        var players = FindObjectsOfType<PhotonView>();
        foreach (var player in players)
        {
            if (player.Owner.NickName == winnerName)
            {
                return player.transform;
            }
        }

        Debug.LogError("No matching player found for winner name: " + winnerName);
        return null;
    }
}
