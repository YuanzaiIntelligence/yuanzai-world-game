using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "SceneData/SceneData")]
public class SceneData : ScriptableObject
{
    public List<SceneInfo> sceneInfoList = new();
}

// �ڵ���Ϣ�洢��
[Serializable]
public class SceneInfo
{
    public string Name;
    public float X;
    public float Y;
    public List<string> linkScene = new();
}

