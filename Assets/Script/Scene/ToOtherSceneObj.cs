using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToOtherSceneObj : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI nameText;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(nameText.text == "返回地图")
        {
            var curSceneName = SceneTranslateManager.Instance.curSceneInfo.Name;
            //Debug.Log(curSceneName);
            SceneTranslateManager.Instance.SceneTransitionMap();
            CameraController.Instance.MoveCamera(curSceneName);
            return;
        }
        SceneTranslateManager.Instance.SceneTransitionScene(nameText.text);

    }
}
