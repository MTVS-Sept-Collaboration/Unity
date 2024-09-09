using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[System.Serializable]
public struct PoseData
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public struct PoseList
{
    public List<PoseData> landmarkList;
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

    public PoseList latestPoseList;

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

    private void ReceiveData()
    {
        while (startReceiving)
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref ip);
                data = Encoding.UTF8.GetString(dataByte);
                if (data.StartsWith("[") && data.EndsWith("]"))
                {
                    data = "{\"landmarkList\":" + data + "}";
                    latestPoseList = JsonUtility.FromJson<PoseList>(data);
                }
                else
                {
                    Debug.LogWarning($"Unrecognized message format: {data}");
                }
                if (printToConsole)
                {
                    print(data);
                }
            }
            catch(Exception ex)
            {
                print(ex.ToString());
            }
        }
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
