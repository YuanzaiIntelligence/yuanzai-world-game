using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "SceneData/SceneData")]
public class SceneData : ScriptableObject
{
    public List<SceneInfo> sceneInfoList = new();
}

// 节点信息存储类
[Serializable]
public class SceneInfo
{
    public string Name;
    public float X;
    public float Y;
    public List<string> linkScene = new();
}

