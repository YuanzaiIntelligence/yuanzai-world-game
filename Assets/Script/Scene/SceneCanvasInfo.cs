using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneCanvasInfo : MonoBehaviour
{
    [Header("Api")]
    public string apiUrl = "https://yuanzaiorld-api-qpkopyhoeh.cn-wulanchabu.fcapp.run/api/query_characters_using_location_api";

    [Header("对象池")]
    public PoolTool poolTool;

    [Header("刷新间隔")]
    public float updateInterval;

    public TextMeshProUGUI textName;

    public GameObject toOtherScenePrefab;

    public Transform toOtherScenePrefabTrans;


    public CharacterResponseData characterResponseData;

    private string characterInfo;

    private List<GameObject> npcList = new();



    private void Start()
    {
        InitSceneInfo();
    }

    private void OnDestroy()
    {
        // 取消重复调用
        CancelInvoke("UpdateMapInfo");
    }

    /// <summary>
    /// 场景canvas初始化
    /// </summary>
    private void InitSceneInfo()
    {
        textName.text = SceneTranslateManager.Instance.curSceneInfo.Name;

        // 立即进行一次更新，然后每隔updateInterval秒重复一次
        InvokeRepeating("UpdateMapInfo", 0f, updateInterval);

        var map = Instantiate(toOtherScenePrefab, toOtherScenePrefabTrans).GetComponent<ToOtherSceneObj>();
        map.nameText.text = "返回地图";

        foreach (var toOtherSceneName in SceneTranslateManager.Instance.curSceneInfo.linkScene)
        {
            var toOtherSceneObj = Instantiate(toOtherScenePrefab, toOtherScenePrefabTrans).GetComponent<ToOtherSceneObj>();
            toOtherSceneObj.nameText.text = toOtherSceneName;
        }
    }

    private void UpdateMapInfo()
    {
        Debug.Log("开始获取角色信息");
        StartCoroutine(GetMapRelationInfo(MapManager.Instance.world_id, textName.text));
    }

    IEnumerator GetMapRelationInfo(int worldId, string location)
    {
        // 创建请求体
        SceneCharacterRequestData requestData = new SceneCharacterRequestData
        {
            world_id = worldId,
            location = location
        };

        // 将请求体序列化为JSON
        string jsonData = JsonUtility.ToJson(requestData);

        yield return StartCoroutine(GameManager.Instance.PostWebRequestSync(apiUrl, jsonData));

        try
        {
            characterResponseData = JsonUtility.FromJson<CharacterResponseData>(characterInfo);

            // 检查是否成功解析
            if (characterResponseData == null)
            {
                Debug.LogError("角色数据解析为空");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"解析JSON失败: {e.Message}\n原始JSON: {characterInfo}");
        }

        UpdateCharacter();
    }

    /// <summary>
    /// 更新场景中的角色
    /// </summary>
    private void UpdateCharacter()
    {
        foreach (var npc in npcList)
        {
            poolTool.ReleaseObjectPool(npc);
        }
        npcList.Clear();

        foreach (var character in characterResponseData.data)
        {
            var npc = poolTool.GetObjectFromPool();
            npc.GetComponent<CharacterInfo>().InitCharacterInfo(character.name, character.action.action);

            npcList.Add(npc);
        }
    }


    /// <summary>
    /// 监听事件
    /// </summary>
    /// <param name="newInfo"></param>
    public void OnGetNewInfoEvent(string newInfo)
    {
        characterInfo = newInfo;
    }
}

// 请求数据类
[System.Serializable]
public class SceneCharacterRequestData
{
    public int world_id;
    public string location;
}


// 与API响应匹配的数据类
[System.Serializable]
public class CharacterResponseData
{
    public string status;
    public string message;
    public List<Character> data;
}

[System.Serializable]
public class Character
{
    public CharacterAction action;
    public string appearance;
    public string force;
    public int id;
    public string location;
    public string name;
    public string position;
    public int world_id;
}

[System.Serializable]
public class CharacterAction
{
    public string HH;
    public string action;
    public string location;
}
