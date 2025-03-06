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
            SceneTranslateManager.Instance.SceneTransitionMap();
            return;
        }
        SceneTranslateManager.Instance.SceneTransitionScene(nameText.text);

    }
}
