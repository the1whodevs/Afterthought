using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay<T> : MonoBehaviour where T : IDisplayableItem
{
    public GameObject AllItemDisplay => allItemDisplay;

    [SerializeField] protected TextMeshProUGUI itemNameField;

    [SerializeField] protected Image itemIconImage;

    [SerializeField] protected GameObject lockedOverlay;
    [SerializeField] protected GameObject allItemDisplay;

    protected T itemToDisplay;

    public void UpdateDisplay()
    {
        if (lockedOverlay) lockedOverlay.SetActive(!itemToDisplay.isUnlocked);

        itemNameField.text = itemToDisplay.name;
        itemIconImage.sprite = itemToDisplay.icon;
    }

    public void SetItemToDisplay(T itemToSet)
    {
        itemToDisplay = itemToSet;
        UpdateDisplay();
    }

    public void ShowAllItemDisplay()
    {
        allItemDisplay.SetActive(true);
    }

    public void HideAllItemDisplay()
    {
        allItemDisplay.SetActive(false);
    }
}

public enum UnlockType { Level, Loot, AlwaysUnlocked }

public class IDisplayableItem : ScriptableObject
{
    public bool isUnlocked
    {
        get
        {
            switch (unlockType)
            {
                case UnlockType.AlwaysUnlocked:
                    return true;
                case UnlockType.Level:
                    return Player.Active.Experience.Level >= levelRequired;
                case UnlockType.Loot:
                    return isLooted;
                default:
                    return false;
            }
        }
    }

    public new string name;
    public string description;
    public Sprite icon;

    public UnlockType unlockType = UnlockType.AlwaysUnlocked;

    public int levelRequired = 1;
    public bool isLooted;
}
