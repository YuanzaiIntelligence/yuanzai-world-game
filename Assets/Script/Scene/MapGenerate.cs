using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public class MapGenerate : MonoBehaviour
{
    [Header("��������")]
    public SceneData sceneData;
    public LineDataSO lineDataSO;

    [Header("Ԥ����")]
    public GameObject scenePrefab;
    public LineRenderer mapLinePrefab;

    [Header("����λ")]
    public Transform scenesTrans;
    public Transform linesTrans;

    private Dictionary<string,SceneObj> scenesDic = new();



    private void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// ����scenedata�����ڵ�ͼ�ϴ���prefab
    /// </summary>
    private void GenerateMap()
    {
        foreach (var sceneInfo in sceneData.sceneInfoList)
        {
            var scene = Instantiate(scenePrefab, new Vector2(sceneInfo.X, sceneInfo.Y), Quaternion.identity, scenesTrans).GetComponent<SceneObj>();
            
            scene.name = sceneInfo.Name;
            scene.nameText.text = sceneInfo.Name;

            scenesDic.Add(sceneInfo.Name, scene);
        }

        if(lineDataSO.linesList.Count == 0)
        {
            FirstGenerateLine();
        }
        else
        {
            GenerateLine();
        }
        

    }

    /// <summary>
    /// �״�������
    /// </summary>
    private void FirstGenerateLine()
    {
        var linesList = new List<LineInfo>();
        foreach (var connection in MapManager.Instance.mapDate.map_relation)
        {
            if (scenesDic.TryGetValue(connection.source, out var sourceObj) && scenesDic.TryGetValue(connection.target, out var targetObj))
            {
                var line = Instantiate(mapLinePrefab, linesTrans);
                line.SetPosition(0, sourceObj.transform.position);
                line.SetPosition(1, targetObj.transform.position);

                var linePos = new LineInfo
                {
                    startPos = new LinePos { x = sourceObj.transform.position.x, y = sourceObj.transform.position.y },
                    endPos = new LinePos { x = targetObj.transform.position.x, y = targetObj.transform.position.y }
                };

                linesList.Add(linePos);
            }
        }
        lineDataSO.linesList = linesList;
    }

    /// <summary>
    /// �����ߵ�����������
    /// </summary>
    private void GenerateLine()
    {
        foreach (var connection in lineDataSO.linesList)
        {

            var line = Instantiate(mapLinePrefab, linesTrans);
            line.SetPosition(0, new Vector2(connection.startPos.x, connection.startPos.y));
            line.SetPosition(1, new Vector2(connection.endPos.x, connection.endPos.y));

        }
    }
}
