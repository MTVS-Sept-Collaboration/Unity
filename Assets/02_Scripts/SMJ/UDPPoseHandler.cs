using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct PoseData
{
    public float x;
    public float y;
    public float z;
}


[System.Serializable]
public struct Data
{
    public List<PoseData> pose_landmarks;
    //보내는 데이터가 Base64 -> 문자열로 받고 ReceiveData에서 Base64로 바꿈
    public string image;

}

public enum PoseName
{
    nose,
    left_eye_inner,
    left_eye,
    left_eye_outer,
    right_eye_inner,
    right_eye,
    right_eye_outer,
    left_ear,
    right_ear,
    mouth_left,
    mouth_right,
    left_shoulder,
    right_shoulder,
    left_elbow,
    right_elbow,
    left_wrist,
    right_wrist,
    left_pinky,
    right_pinky,
    left_index,
    right_index,
    left_thumb,
    right_thumb,
    left_hip,
    right_hip,
    left_knee,
    right_knee,
    left_ankle,
    right_ankle,
    left_heel,
    right_heel,
    left_foot_index,
    right_foot_index
}
public class UDPPoseHandler : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    private int port = 8764;
    public bool startReceiving = true;
    public bool printToConsole = false;
    public string data;

    public List<PoseData> latestPoseList;
    public Texture2D latestImageTexture;
    private byte[] imageToProcess = null;

    public RawImage displayWebCam;

    private void Start()
    {
        InitializeUDP();
    }

    private void InitializeUDP()
    {
        client = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        
    }

    Data _data;

    private void ReceiveData()
    {
        while (startReceiving)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref ip);
                string jsonData = Encoding.UTF8.GetString(dataByte);
               // print(jsonData);

                _data = JsonUtility.FromJson<Data>(jsonData);
                lock (this)
                {
                    
                    latestPoseList = _data.pose_landmarks;
                    //string을 Base64로바꿈
                    imageToProcess = Convert.FromBase64String(_data.image); ; // 나중에 처리할 이미지들 저장 
                }

            }

            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        
    }

    private void Update()
    {
        if (imageToProcess != null)
        {
            lock (this)
            {
                latestImageTexture = DecodeImage(imageToProcess);
                imageToProcess = null; // 처리 끝난 이후 초기화
                
                displayWebCam.texture = latestImageTexture;
            }
        }

    }

    private Texture2D DecodeImage(byte[] imageData)
    {
        Texture2D texture = new Texture2D(1100, 700);

        if (imageData != null && imageData.Length > 0)
        {
            texture.LoadImage(imageData);
        }
        return texture;
    }

    public void UDPClose()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }
        if (client != null)
        {
            client.Close();
        }
    }

    private void OnDestroy()
    {
        UDPClose();
    }

}
