using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToOtherSceneObj : MonoBehaviour, IPointerDownHandler
{
    public TextMeshProUGUI nameText;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(nameText.text == "���ص�ͼ")
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
