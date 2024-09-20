using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Reflection;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Networking;
using static Gpm.Common.Util.XmlHelper;

public class JSWConnectionManager : MonoBehaviourPunCallbacks
{
    public GameObject roomPrefab;
    public Transform scrollContent;
    public GameObject[] panelList;

    private readonly string CreateRoomUrl = "https://125.132.216.190:12502/api/room";
    private readonly string RemoveRoomUrl = "https://125.132.216.190:12502/api/room";

    List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    void Start()
    {
        // 모든 인증서 무시 설정
        ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        Screen.SetResolution(640, 480, FullScreenMode.Windowed);
        StartLogin();
    }

    // 모든 인증서를 신뢰하는 메서드
    private bool TrustAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    public void StartLogin()
    {
        if (AvatarInfo.instance.NickName.Length >= 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = AvatarInfo.instance.NickName;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnected()
    {
        base.OnConnected();
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogError("Disconnected from Server - " + cause);
        LobbyUIController.lobbyUI.btn_login.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LobbyUIController.lobbyUI.ShowRoomPanel();

        string[] avatarInfo = AvatarInfo.instance.ReturnAvatarInfo();
        print(avatarInfo);
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {
            { "Backpack", avatarInfo[0]}, { "Body", avatarInfo[1] }, { "Eyebrow", avatarInfo[2] },
            { "Glasses", avatarInfo[3] }, { "Glove", avatarInfo[4] }, { "Hair", avatarInfo[5] },
            { "Hat", avatarInfo[6] }, { "Mustache", avatarInfo[7] }, { "Outerwear", avatarInfo[8]},
            { "Pants", avatarInfo[9] }, { "Shoe", avatarInfo[10] }
        });
    }

    public void CreateRoom()
    {
        string roomName = LobbyUIController.lobbyUI.roomSetting[0].text;
        int playerCount = Convert.ToInt32(LobbyUIController.lobbyUI.roomSetting[1].text);

        print(roomName + " " + playerCount);
        if (roomName.Length > 0 && playerCount > 1)
        {
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.MaxPlayers = playerCount;
            roomOpt.IsOpen = true;
            roomOpt.IsVisible = true;

            PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);
        }
    }

    void ChangePanel(int offIndex, int onIndex)
    {
        panelList[offIndex].SetActive(false);
        panelList[onIndex].SetActive(true);
    }

    public void JoinRoom()
    {
        ChangePanel(1, 2);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        // 이따가 다시 원상복구
        //StartCoroutine(SendRoomInfo("POST", PhotonNetwork.CurrentRoom.Name, PlayerPrefs.GetString("userId")));
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        PhotonNetwork.LoadLevel(8);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.LogError(message);
        LobbyUIController.lobbyUI.PrintLog("입장 실패..." + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        string playerMsg = $"{newPlayer.NickName}님이 입장하셨습니다.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        string playerMsg = $"{otherPlayer.NickName}님이 퇴장하셨습니다.";
        LobbyUIController.lobbyUI.PrintLog(playerMsg);

        if (PhotonNetwork.CountOfPlayersInRooms == 0)
        {
            StartCoroutine(SendRoomInfo("DELETE", PhotonNetwork.CurrentRoom.Name));
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                cachedRoomList.Remove(room);
            }
            else
            {
                if (cachedRoomList.Contains(room))
                {
                    cachedRoomList.Remove(room);
                }
                cachedRoomList.Add(room);
            }
        }

        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        foreach (RoomInfo room in cachedRoomList)
        {
            GameObject go = Instantiate(roomPrefab, scrollContent);
            JSWRoomPanel roomPanel = go.GetComponent<JSWRoomPanel>();
            roomPanel.SetRoomInfo(room);
            roomPanel.btn_join.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

    public void MoveMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void MoveProfileScene()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    private IEnumerator SendRoomInfo(string method, string roomId, string ownerId = null)
    {
        string url;
        if (method == "POST")
        {
            url = CreateRoomUrl;
        }
        else if (method == "DELETE")
        {
            url = $"{RemoveRoomUrl}/{roomId}";
        }
        else
        {
            Debug.LogError("Invalid HTTP method");
            yield break;
        }

        // JWT 토큰 가져오기
        string jwtToken = PlayerPrefs.GetString("jwtToken");
        if (string.IsNullOrEmpty(jwtToken))
        {
            Debug.LogError("JWT token is missing or empty");
            yield break;
        }

        UnityWebRequest www;
        if (method == "POST")
        {
            CreateRoomData roomData = new CreateRoomData
            {
                roomId = long.Parse(roomId),
                ownerId = long.Parse(ownerId)
            };
            string jsonData = JsonUtility.ToJson(roomData);
            www = UnityWebRequest.Put(url, jsonData);
            www.method = "POST"; // 메서드를 POST로 변경
            www.SetRequestHeader("Content-Type", "application/json");
        }
        else // DELETE
        {
            www = UnityWebRequest.Delete(url);
        }

        // JWT 토큰을 Authorization 헤더에 추가
        www.SetRequestHeader("Authorization", "Bearer " + jwtToken);

        www.certificateHandler = new BypassCertificate1();

        Debug.Log($"Sending {method} request to {url}");
        Debug.Log($"Authorization: Bearer {jwtToken.Substring(0, Math.Min(jwtToken.Length, 10))}..."); // 토큰의 일부만 로그에 출력
        if (method == "POST")
        {
            Debug.Log($"Request body: {www.uploadHandler.data}");
        }

        yield return www.SendWebRequest();

        Debug.Log($"Response Code: {www.responseCode}");
        Debug.Log($"Response Headers: {www.GetResponseHeaders()}");
        Debug.Log($"Response Body: {www.downloadHandler.text}");

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{method} request to {url} failed. Error: {www.error}");
            Debug.LogError($"Full response: {www.downloadHandler.text}");
        }
        else
        {
            Debug.Log($"{method} request to {url} successful!");
        }

        www.certificateHandler.Dispose();
    }
}

public class BypassCertificate1 : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}

[Serializable]
public class CreateRoomData
{
    public long roomId;
    public long ownerId;
}