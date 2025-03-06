using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;

///// <summary>
///// 所有连接列表
///// </summary>
//[Serializable]
//public class MapRelation
//{
//    public List<NodeConnection> map_relation;
//}

///// <summary>
///// source - target连接关系
///// </summary>
//[Serializable]
//public class NodeConnection
//{
//    public string source;
//    public string target;
//}



public class MapManager : Singleton<MapManager>
{
    [Header("Api")]
    public string apiUrl = "https://yuanzaiorld-api-qpkopyhoeh.cn-wulanchabu.fcapp.run/api/get_map_relation_info_api";
    public int world_id = 4;

    [Header("收到的数据")]
    public MapRelationResponseData responseData;

    [Header("生成的场景信息")]
    public List<SceneInfo> sceneInfoList = new();

    [Header("场景数据")]
    public SceneData sceneData;
    public LineDataSO lineDataSO;

    protected override void Awake()
    {
        base.Awake();

        if(sceneData.sceneInfoList.Count == 0)
            StartCoroutine(GetMapRelationInfo(world_id));


        //暂时
        SetFirstScene();

    }


    [ContextMenu("清空数据")]
    public void ClearData()
    {
        sceneData.sceneInfoList = null;
        lineDataSO.linesList = null;

    }



    IEnumerator GetMapRelationInfo(int worldId)
    {
        // 创建请求体
        MapRelationRequestData requestData = new MapRelationRequestData
        {
            world_id = worldId
        };

        // 将请求体序列化为JSON
        string jsonData = JsonUtility.ToJson(requestData);

        // 创建UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");

        // 发送请求
        yield return request.SendWebRequest();

        // 处理响应
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("API请求错误: " + request.error);
        }
        else
        {
            // 成功获取响应
            Debug.Log("API响应: " + request.downloadHandler.text);

            // 解析JSON响应
            responseData = JsonUtility.FromJson<MapRelationResponseData>(request.downloadHandler.text);

            StoreSceneLayout();
        }
    }



    /// <summary>
    /// 存储位置信息
    /// </summary>
    private void StoreSceneLayout()
    {
        foreach (var node in responseData.data.nodes)
        {
            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.Name = node.name;
            sceneInfo.X = node.x * 5;
            sceneInfo.Y = node.y * 5;

            CameraController.Instance.UpdateMaxMin(sceneInfo.X, sceneInfo.Y);

            foreach (var connection in responseData.data.map_relation)
            {
                if (node.name == connection.source)
                {
                    sceneInfo.linkScene.Add(connection.target);
                }
                else if (node.name == connection.target)
                {
                    sceneInfo.linkScene.Add(connection.source);
                }
            }

            sceneInfoList.Add(sceneInfo);
        }

        sceneData.sceneInfoList = sceneInfoList;

        //暂时
        SetFirstScene();

        CameraController.Instance.canFreeMove = true;
        SceneTranslateManager.Instance.Transition(string.Empty, SceneName.Map.ToString());
    }

    /// <summary>
    /// 暂时
    /// </summary>
    private void SetFirstScene()
    {
        if (sceneData.sceneInfoList.Count != 0)
            SceneTranslateManager.Instance.lookScene = sceneData.sceneInfoList[0].Name;
    }

}


// 请求数据类
[System.Serializable]
public class MapRelationRequestData
{
    public int world_id;
}

// 与API响应匹配的数据类
[System.Serializable]
public class MapRelationResponseData
{
    public string status;
    public string message;
    public MapData data;
}

[System.Serializable]
public class MapData
{
    public List<MapRelation> map_relation;
    public List<MapNode> nodes;
}

[System.Serializable]
public class MapRelation
{
    public string source;
    public string target;
}

[System.Serializable]
public class MapNode
{
    public string name;
    public float x;
    public float y;
}



