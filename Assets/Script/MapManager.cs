using Dagre;
using System.Collections.Generic;
using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Xml.Linq;

/// <summary>
/// ���������б�
/// </summary>
[Serializable]
public class MapRelation
{
    public List<NodeConnection> map_relation;
}

/// <summary>
/// source - target���ӹ�ϵ
/// </summary>
[Serializable]
public class NodeConnection
{
    public string source;
    public string target;
}



public class MapManager : Singleton<MapManager>
{
    [Header("mapJSON����")]
    [TextArea(5, 10)]
    public string mapJSONData;

    [Header("�������map����")]
    public MapRelation mapDate;

    [Header("���ɵĳ�����Ϣ")]
    public List<SceneInfo> sceneInfoList = new();

    [Header("����")]
    public bool IsVertical;

    [Header("��������")]
    public SceneData sceneData;
    public LineDataSO lineDataSO;

    // ���нڵ�
    public HashSet<string> nodeNames = new();

    // �洢�����Ľڵ㣬�Ա��ں�������
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

    [ContextMenu("�������")]
    public void ClearData()
    {
        sceneData.sceneInfoList = null;
        lineDataSO.linesList = null;

    }


    /// <summary>
    /// ����mapJson���ݣ�����map����
    /// </summary>
    private void CreateGraphFromJson()
    {
        mapDate = JsonConvert.DeserializeObject<MapRelation>(mapJSONData);

        // �ռ����нڵ�����
        foreach (var connection in mapDate.map_relation)
        {
            nodeNames.Add(connection.source);
            nodeNames.Add(connection.target);
        }


        // ����ͼʵ��
        DagreInputGraph dg = new DagreInputGraph();
        dg.VerticalLayout = IsVertical;


        // ���ȴ������нڵ�
        foreach (string nodeName in nodeNames)
        {
            // �����ڵ㲢�洢���ֵ��У��Ա��������
            var node = dg.AddNode(new { Name = nodeName }, 200, 200);
            nodeDict[nodeName] = node;
        }

        //������еı�
        foreach (var connection in mapDate.map_relation)
        {
            
            // ��ȡԴ�ڵ��Ŀ��ڵ�
            if(nodeDict.TryGetValue(connection.source, out var sourceNode) && nodeDict.TryGetValue(connection.target, out var targetNode))
            {
                dg.AddEdge(sourceNode, targetNode);
            }
            else
            {
                Debug.LogWarning($"�޷���ӱ�: {connection.source} -> {connection.target}���ڵ㲻����");
            }

        }

        try
        {
            // ���㲼��
            dg.Layout();
            
        }
        catch (Exception ex)
        {
            Debug.LogError($"���ּ���ʧ��: {ex.Message}\n{ex.StackTrace}");
        }

        // �洢λ����Ϣ
        StoreSceneLayout();
    }

    /// <summary>
    /// �洢λ����Ϣ
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



