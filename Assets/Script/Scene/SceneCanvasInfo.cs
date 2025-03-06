using TMPro;
using UnityEngine;

public class SceneCanvasInfo : MonoBehaviour
{
    public TextMeshProUGUI textName;

    public GameObject toOtherScenePrefab;

    public Transform toOtherScenePrefabTrans;

    private void Start()
    {
        InitSceneInfo();
    }

    /// <summary>
    /// 场景canvas初始化
    /// </summary>
    private void InitSceneInfo()
    {
        textName.text = SceneTranslateManager.Instance.curSceneInfo.Name;

        var map = Instantiate(toOtherScenePrefab, toOtherScenePrefabTrans).GetComponent<ToOtherSceneObj>();
        map.nameText.text = "返回地图";

        foreach (var toOtherSceneName in SceneTranslateManager.Instance.curSceneInfo.linkScene)
        {
            var toOtherSceneObj = Instantiate(toOtherScenePrefab, toOtherScenePrefabTrans).GetComponent<ToOtherSceneObj>();
            toOtherSceneObj.nameText.text = toOtherSceneName;
        }
    }
}
