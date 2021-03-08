using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveDisplay : MonoBehaviour
{
    public SaveSystem.SaveData Data { get; private set; }

    [SerializeField] private TextMeshProUGUI saveTitle;
    [SerializeField] private TextMeshProUGUI saveInfo;
    [SerializeField] private Image saveScreenshot;

    private int myIndex;

    public void Init(int index)
    {
        myIndex = index;

        if (SaveManager.Active.NumOfSaves == index)
        {
            saveTitle.text = "NEW SAVE";
            saveInfo.text = "";
        }
        else
        {
            this.Data = SaveSystem.Load(index);

            saveTitle.text = index == -1 ? "QUICK SAVE" : $"Save #{index}";

            var scene = LoadingManager.Active.GetSceneName(Data.level);
            var objectiveText = LoadingManager.Active.GetObjectiveDescription(Data.level, Data.objectiveIndex);

            saveInfo.text = $"{scene} - Level {Data.playerLevel} - {objectiveText}";

            saveScreenshot.sprite = SaveSystem.GetScreenshot(index);
        }
    }

    public void Select()
    {
        UISoundFXManager.Active.PlayClickSFX();
        SaveManager.Active.SelectSave(myIndex);
        UIManager.Active.SelectSave(saveInfo.text, saveScreenshot.sprite);
    }
}
