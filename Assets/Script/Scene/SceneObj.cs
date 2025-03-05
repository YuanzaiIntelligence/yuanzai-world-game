using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneObj : MonoBehaviour, IPointerDownHandler
{
    public TextMeshPro nameText;

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(nameText.text);
        SceneTranslateManager.Instance.MapTransitionScene(nameText.text);
        CameraController.Instance.MoveCamera(string.Empty);
    }
}
