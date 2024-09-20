using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlaySceneManager : MonoBehaviourPunCallbacks
{
    public GameObject myPlayer;
    public Transform[] playerPositions;

    public static PlaySceneManager instance;

    private Hashtable CP;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 씬 전환이 되도 게임 오브젝트를 파괴하고 싶지않다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MoveMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    void Start()
    {
        StartCoroutine(SpawnPlayer());

        // OnPhotonSerializeView에서 데이터 전송 빈도 수 설정하기 (per seconds)
        PhotonNetwork.SerializationRate = 30;
        // 대부분의 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SendRate = 30;

        GameObject playerListUI = GameObject.Find("text_PlayerList");
        CP = PhotonNetwork.LocalPlayer.CustomProperties;
        
    }

    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });


        Vector2 randomPos = Random.insideUnitCircle * 5.0f;
        Vector3 initPosition = new Vector3(randomPos.x, 0, randomPos.y);

        myPlayer = PhotonNetwork.Instantiate("Player", initPosition, Quaternion.identity);
        string[] avatarsetting = new string[11];
        avatarsetting[0] = (string)CP["Backpack"];
        avatarsetting[1] = (string)CP["Body"];
        avatarsetting[2] = (string)CP["Eyebrow"];
        avatarsetting[3] = (string)CP["Glasses"];
        avatarsetting[4] = (string)CP["Glove"];
        avatarsetting[5] = (string)CP["Hair"];
        avatarsetting[6] = (string)CP["Hat"];
        avatarsetting[7] = (string)CP["Mustache"];
        avatarsetting[8] = (string)CP["Outerwear"];
        avatarsetting[9] = (string)CP["Pants"];
        avatarsetting[10] = (string)CP["Shoe"];
        myPlayer.GetComponent<JSWPhotonVoiceTest>().SettingAvatar_RPC(avatarsetting);
    }
}
