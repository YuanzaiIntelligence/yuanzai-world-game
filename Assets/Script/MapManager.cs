using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;



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

    [Header("事件广播")]
    public StringEventSO firstLoadMapEvent;

    private string mapInfo;

    protected override void Awake()
    {
        base.Awake();

        if(sceneData.sceneInfoList.Count == 0)
            StartCoroutine(GetMapRelationInfo(world_id));
        else
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

        yield return StartCoroutine(GameManager.Instance.PostWebRequestSync(apiUrl, jsonData));

        try
        {
            responseData = JsonUtility.FromJson<MapRelationResponseData>(mapInfo);

            // 检查是否成功解析
            if (responseData == null)
            {
                Debug.LogError("地图数据解析为空");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"解析JSON失败: {e.Message}\n原始JSON: {mapInfo}");
        }
        

        StoreSceneLayout();

    }


    /// <summary>
    /// 监听事件
    /// </summary>
    /// <param name="newInfo"></param>
    public void OnGetNewInfoEvent(string newInfo)
    {
        mapInfo = newInfo;
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

    }

    /// <summary>
    /// 暂时
    /// </summary>
    private void SetFirstScene()
    {
        if (sceneData.sceneInfoList.Count != 0)
        {
            firstLoadMapEvent.RaisEvent(sceneData.sceneInfoList[0].Name, this);
        }
            
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



