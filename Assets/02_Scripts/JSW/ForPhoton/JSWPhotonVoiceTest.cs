using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.PUN;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class JSWPhotonVoiceTest : MonoBehaviourPunCallbacks, IPunObservable
{
    public RawImage voiceIcon;
    PhotonVoiceView voiceView;
    PhotonView pv; 
    bool isTalking = false;
    private string[] avatarSettings;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        voiceView = GetComponent<PhotonVoiceView>();
        gameObject.transform.position = PlaySceneManager.instance.playerPositions[photonView.Owner.ActorNumber - 1].position;
        //AvatarInfo.instance.SettingAvatarInPlay(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            // 현재 말을 하고 있다면 보이스 아이콘을 활성화한다.
            voiceIcon.gameObject.SetActive(voiceView.IsRecording);
        }
        else
        {
            voiceIcon.gameObject.SetActive(isTalking);
        }
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만일, 데이터를 서버에 전송(Photonview.IsMine == true)하는 상태라면...
        if (stream.IsWriting && voiceView != null)
        {
            stream.SendNext(voiceView.IsRecording);
        }
        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면...
        else if (stream.IsReading)
        {
            isTalking = (bool)stream.ReceiveNext();
        }
    }

    public void SettingAvatar_RPC(string[] avatarsetting)
    {
        pv = GetComponent<PhotonView>();
        pv.RPC(nameof(SettingAvatar), RpcTarget.AllBuffered, avatarsetting);
    }

    [PunRPC]
    public void SettingAvatar(string[] avatarsetting)
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
        avatarSettings = avatarsetting;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        
        if (pv.IsMine && avatarSettings != null)
        {
            pv.RPC(nameof(SettingAvatar), RpcTarget.AllBuffered, avatarSettings);
        }
    }
}
