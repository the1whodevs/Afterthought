using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay<T> : MonoBehaviour where T : IDisplayableItem
{
    [SerializeField] protected TextMeshProUGUI itemNameField;

    [SerializeField] protected Image itemIconImage;

    protected T itemToDisplay;

    public void UpdateDisplay()
    {
        itemNameField.text = itemToDisplay.name;
        itemIconImage.sprite = itemToDisplay.icon;
    }

    public void SetItemToDisplay(T itemToSet)
    {
        itemToDisplay = itemToSet;
    }
}

public class IDisplayableItem : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite icon;
}
