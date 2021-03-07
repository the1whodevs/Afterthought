using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        this.Data = SaveSystem.Load(index);

        saveTitle.text = index == -1 ? "QUICK SAVE" : $"Save #{index}";

        saveInfo.text = $"{SceneManager.GetSceneByBuildIndex(Data.level).name} - Level {Data.playerLevel} - {Data.objectiveIndex}";

        saveScreenshot.sprite = SaveSystem.GetScreenshot(index);
    }

    public void Select()
    {
        SaveManager.Active.SelectSave(myIndex);
        UIManager.Active.SelectSave(saveInfo.text, saveScreenshot.sprite);
    }
}
