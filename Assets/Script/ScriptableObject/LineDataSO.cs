using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineDataSO", menuName = "SceneData/LineDataSO")]
public class LineDataSO : ScriptableObject
{
    public List<LineInfo> linesList;
}

[Serializable]
public class LineInfo
{
    public LinePos startPos;
    public LinePos endPos;
}

[Serializable]
public class LinePos
{
    public float x, y;
}
