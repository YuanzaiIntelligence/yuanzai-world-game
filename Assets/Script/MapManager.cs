using Dagre;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Xml.Linq;

/// <summary>
/// 所有连接列表
/// </summary>
[Serializable]
public class MapRelation
{
    public List<NodeConnection> map_relation;
}

/// <summary>
/// source - target连接关系
/// </summary>
[Serializable]
public class NodeConnection
{
    public string source;
    public string target;
}



public class MapManager : Singleton<MapManager>
{
    [Header("mapJSON数据")]
    [TextArea(5, 10)]
    public string mapJSONData;

    [Header("解析后的map数据")]
    public MapRelation mapDate;

    [Header("生成的场景信息")]
    public List<SceneInfo> sceneInfoList = new();

    [Header("布局")]
    public bool IsVertical;

    [Header("场景数据")]
    public SceneData sceneData;
    public LineDataSO lineDataSO;

    // 所有节点
    public HashSet<string> nodeNames = new();

    // 存储创建的节点，以便于后续引用
    private Dictionary<string, DagreInputNode> nodeDict = new();

    private string firstScene;

    protected override void Awake()
    {
        base.Awake();

        //ClearData();
        if (sceneData.sceneInfoList.Count == 0)
            CreateGraphFromJson();
    }


    private void Start()
    {
        firstScene = sceneData.sceneInfoList[0].Name;
        CameraController.Instance.MoveCamera(firstScene);
    }

    [ContextMenu("清空数据")]
    public void ClearData()
    {
        sceneData.sceneInfoList = null;
        lineDataSO.linesList = null;

    }


    /// <summary>
    /// 解析mapJson数据，创建map布局
    /// </summary>
    private void CreateGraphFromJson()
    {
        mapDate = JsonConvert.DeserializeObject<MapRelation>(mapJSONData);

        // 收集所有节点名称
        foreach (var connection in mapDate.map_relation)
        {
            nodeNames.Add(connection.source);
            nodeNames.Add(connection.target);
        }


        // 创建图实例
        DagreInputGraph dg = new DagreInputGraph();
        dg.VerticalLayout = IsVertical;


        // 首先创建所有节点
        foreach (string nodeName in nodeNames)
        {
            // 创建节点并存储在字典中，以便后续引用
            var node = dg.AddNode(new { Name = nodeName }, 400, 200);
            nodeDict[nodeName] = node;
        }

        //添加所有的边
        foreach (var connection in mapDate.map_relation)
        {
            
            // 获取源节点和目标节点
            if(nodeDict.TryGetValue(connection.source, out var sourceNode) && nodeDict.TryGetValue(connection.target, out var targetNode))
            {
                dg.AddEdge(sourceNode, targetNode);
            }
            else
            {
                Debug.LogWarning($"无法添加边: {connection.source} -> {connection.target}，节点不存在");
            }

        }

        try
        {
            // 计算布局
            dg.Layout();
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"布局计算失败: {ex.Message}\n{ex.StackTrace}");
        }

        // 存储位置信息
        StoreSceneLayout();
    }

    /// <summary>
    /// 存储位置信息
    /// </summary>
    private void StoreSceneLayout()
    {
        foreach (var node in nodeDict)
        {
            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.Name = node.Key;
            sceneInfo.X = node.Value.X / 100;
            sceneInfo.Y = node.Value.Y / 100;

            foreach (var connection in mapDate.map_relation)
            {
                if (node.Key == connection.source)
                {
                    sceneInfo.linkScene.Add(connection.target);
                }
                else if (node.Key == connection.target)
                {
                    sceneInfo.linkScene.Add(connection.source);
                }
            }

            sceneInfoList.Add(sceneInfo);
        }

        sceneData.sceneInfoList = sceneInfoList;
    }


}



