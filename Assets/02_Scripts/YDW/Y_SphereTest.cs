using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Y_SphereTest : MonoBehaviour
{
    public WebSocketPoseHandler conn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < conn.latestPoseList.landmarkList.Count; i++)
        {
            Vector3 localPos = new Vector3(
                    conn.latestPoseList.landmarkList[i].x,
                    conn.latestPoseList.landmarkList[i].y,
                    conn.latestPoseList.landmarkList[i].z);

            localPos = new Vector3(localPos.x, -localPos.y, -localPos.z);

            transform.GetChild(i).localPosition = localPos * 3;
        }
    }
}
