using TMPro;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI characterAction;

    public void InitCharacterInfo(string name, string action)
    {
        characterName.text = name;
        characterAction.text = action;
    }
}
