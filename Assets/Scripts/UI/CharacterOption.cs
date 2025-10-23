using UnityEngine;

public class CharacterOption : MonoBehaviour
{
    [SerializeField] int index;                     // 0,1,2...
    [SerializeField] MainMenuController menu;       // drag MainMenuController here
    [SerializeField] GameObject selectedMark;       // optional highlight overlay (can be left null)

    public int Index => index;

    // Hook this to the Button's OnClick() in the Inspector
    public void OnClick()
    {
        if (menu != null)
            menu.SelectCharacter(index);
    }

    public void SetSelected(bool on)
    {
        if (selectedMark) selectedMark.SetActive(on);
    }
}
