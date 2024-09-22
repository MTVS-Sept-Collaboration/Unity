using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.PUN;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using TMPro;
using UnityEditor.Build;

public class JSWPhotonVoiceTest : MonoBehaviourPunCallbacks, IPunObservable
{
    public RawImage voiceIcon;
    public TMP_Text names;
    public bool isMaster;

    Y_UIManager y_uiManager;
    public Y_TimerUI y_timerUI;

    PhotonVoiceView voiceView;
    PhotonView pv;

    public int currentPlayers;
    public int maxPlayers;

    bool isTalking = false;
    private string[] avatarSettings;
    string nickName;

    public GameObject GameStartReady;
    public bool mineHasStart;
    public bool otherHasStart;

    public GameObject infoUI;
    public Vector3[] infoVector;

    bool allStart;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
        gameObject.transform.position = PlaySceneManager.instance.playerPositions[photonView.Owner.ActorNumber - 1].position + Vector3.up * 1.4f;
        //AvatarInfo.instance.SettingAvatarInPlay(gameObject);
        y_uiManager = GameObject.Find("Canvas").GetComponent<Y_UIManager>();
        y_timerUI = GameObject.Find("Canvas").GetComponent<Y_TimerUI>();
        infoUI.GetComponent<RectTransform>().localPosition = infoVector[photonView.Owner.ActorNumber - 1];
    }

    bool isStart;
    public bool AllplayerInRoom;

    // Update is called once per frame
    void Update()
    {
        currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        mineHasStart = y_timerUI.hasStart;

        if (currentPlayers == maxPlayers)
        {
            AllplayerInRoom = true;
        }

        if (PhotonNetwork.IsMasterClient && pv.IsMine && currentPlayers == maxPlayers && !isStart)
        {
            isMaster = true;
            isStart = true;
        }


        if (pv.IsMine)
        {
            // 현재 말을 하고 있다면 보이스 아이콘을 활성화한다.
            voiceIcon.gameObject.SetActive(voiceView.IsRecording);
            GameStartReady.SetActive(y_timerUI.hasStart); // Pose에서 받아온 값
        }
        else
        {
            voiceIcon.gameObject.SetActive(isTalking);
            GameStartReady.SetActive(otherHasStart); // 에서 받아온 값
        }

        if (y_timerUI.hasStart && !allStart && IsAllGoOkay())
        {
            allStart = true;
            AllReadyGO_RPC();
        }
    }

    public bool IsAllGoOkay()
    {
        var AllReadyList = PhotonNetwork.PlayerList
            .Select(player =>
            {
                PhotonView photonView = null;
                foreach (var view in PhotonNetwork.PhotonViewCollection)
                {
                    if (view.Owner == player)
                    {
                        photonView = view;
                        break;
                    }
                }

                bool okay = photonView.gameObject.GetComponent<JSWPhotonVoiceTest>().GameStartReady.gameObject.activeSelf;
                return okay;
            })
            .ToList();

        bool isAllGo = true;
        //if (AllReadyList.Count >= 1)
        //{
        //    isAllGo = true;
        //}
        //else
        //{
        //    isAllGo = false;
        //}


        for (int i = 0; i < AllReadyList.Count; i++)
        {
            if (AllReadyList[i] == false)
            {
                isAllGo = false;
            }
        }
        return isAllGo;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일, 데이터를 서버에 전송(Photonview.IsMine == true)하는 상태라면...
        if (stream.IsWriting)
        {
            stream.SendNext(voiceView != null ? voiceView.IsRecording : false);
            stream.SendNext(y_timerUI != null ? y_timerUI.hasStart : false);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면...
        else
        {
            // 타입 체크를 추가하여 안전하게 캐스팅
            if (stream.Count >= 2)
            {
                object nextItem = stream.ReceiveNext();
                if (nextItem is bool)
                {
                    isTalking = (bool)nextItem;
                }

                nextItem = stream.ReceiveNext();
                if (nextItem is bool)
                {
                    otherHasStart = (bool)nextItem;
                }
            }
        }
    }

    public void SettingAvatar_RPC(string[] avatarsetting, string nickname)
    {
        pv = GetComponent<PhotonView>();
        pv.RPC(nameof(SettingAvatar), RpcTarget.AllBuffered, avatarsetting, nickname);
    }


    [PunRPC]
    public void SettingAvatar(string[] avatarsetting, string nickname)
    {
        for (int i = 0; i < 11; i++)
        {
            if (avatarsetting[i] != null)
            {
                gameObject.transform.GetChild(1).GetChild(i).GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>(avatarsetting[i]);
            }
            else
            {
                gameObject.transform.GetChild(1).GetChild(i).GetComponent<SkinnedMeshRenderer>().sharedMesh = null;
            }
        }
        names.text = nickname;
        avatarSettings = avatarsetting;
        nickName = nickname;
    }


    public void ChooseSquateOrJump_RPC(int chooseNum)
    {
        pv = GetComponent<PhotonView>();
        pv.RPC(nameof(ChooseSquateOrJump), RpcTarget.AllBuffered, chooseNum);
    }

    [PunRPC]
    public void ChooseSquateOrJump(int chooseNum)
    {
        if (chooseNum == 1)
        {
            y_uiManager.SelectSquat2();
        }
        else if (chooseNum == 2)
        {

            y_uiManager.SelectJumpingJack2();
        }
    }

    public void AllReadyGO_RPC()
    {
        pv = GetComponent<PhotonView>();
        pv.RPC(nameof(AllReadyGo), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void AllReadyGo()
    {
        StartCoroutine(CountingBeforeSquat());
        //y_timerUI.allReadyGo = true;

    }



    public IEnumerator CountingBeforeSquat()
    {
        JSWSoundManager.Get().PlayEftSound(JSWSoundManager.ESoundType.EFT_COUNDOWN);
        y_uiManager.BeforeStartUI.SetActive(true);
        y_uiManager.BeforeStartUI.GetComponent<TMP_Text>().text = "2";
        yield return new WaitForSeconds(1.5f);
        y_uiManager.BeforeStartUI.GetComponent<TMP_Text>().text = "1";
        yield return new WaitForSeconds(1.5f);
        y_uiManager.BeforeStartUI.GetComponent<TMP_Text>().text = "0";
        yield return new WaitForSeconds(0.2f);
        y_uiManager.BeforeStartUI.GetComponent<TMP_Text>().text = "Start!!";
        JSWSoundManager.Get().PlayEftSound(JSWSoundManager.ESoundType.EFT_GAMESTART);
        yield return new WaitForSeconds(1f);
        JSWSoundManager.Get().PlayBgmSound(JSWSoundManager.EBgmType.BGM_Playing);
        y_uiManager.BeforeStartUI.SetActive(false);
        y_timerUI.allReadyGo = true;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        
        if (pv.IsMine && avatarSettings != null)
        {
            pv.RPC(nameof(SettingAvatar), RpcTarget.AllBuffered, avatarSettings,nickName);
        }
    }
}
