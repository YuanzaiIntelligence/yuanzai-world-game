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
    /// ����canvas��ʼ��
    /// </summary>
    private void InitSceneInfo()
    {
        textName.text = SceneTranslateManager.Instance.curSceneInfo.Name;

        var map = Instantiate(toOtherScenePrefab, toOtherScenePrefabTrans).GetComponent<ToOtherSceneObj>();
        map.nameText.text = "���ص�ͼ";

        foreach (var toOtherSceneName in SceneTranslateManager.Instance.curSceneInfo.linkScene)
        {
            var toOtherSceneObj = Instantiate(toOtherScenePrefab, toOtherScenePrefabTrans).GetComponent<ToOtherSceneObj>();
            toOtherSceneObj.nameText.text = toOtherSceneName;
        }
    }
}
