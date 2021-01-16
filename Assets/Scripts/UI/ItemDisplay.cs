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

public enum UnlockType { Level, Loot, Both }

public class IDisplayableItem : ScriptableObject
{
    public bool isUnlocked
    {
        get
        {
            switch (unlockType)
            {
                case UnlockType.Both:
                    return Player.Instance.Experience.Level >= levelRequired && isLooted;
                case UnlockType.Level:
                    return Player.Instance.Experience.Level >= levelRequired;
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

    public UnlockType unlockType = UnlockType.Both;

    public int levelRequired = 1;
    public bool isLooted;

    private string LOOT_STATUS => $"{name}_LOOT_STATUS";

    public virtual void SaveData()
    {
        PlayerPrefs.SetInt(LOOT_STATUS, isLooted ? 1 : 0);
    }

    public virtual void LoadData()
    {
        isLooted = PlayerPrefs.GetInt(LOOT_STATUS, 0) == 1;
    }
}
