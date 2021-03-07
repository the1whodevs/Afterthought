﻿using TMPro;
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

        if (SaveManager.Active.NumOfSaves == index)
        {
            saveTitle.text = "NEW SAVE";
            saveInfo.text = "";
            saveScreenshot.sprite = null;
        }
        else
        {
            this.Data = SaveSystem.Load(index);

            saveTitle.text = index == -1 ? "QUICK SAVE" : $"Save #{index}";

            var scene = LoadingManager.Active.GetSceneName(Data.level);

            saveInfo.text = $"{scene} - Level {Data.playerLevel} - {Data.objectiveIndex}";

            saveScreenshot.sprite = SaveSystem.GetScreenshot(index);
        }
    }

    public void Select()
    {
        SaveManager.Active.SelectSave(myIndex);
        UIManager.Active.SelectSave(saveInfo.text, saveScreenshot.sprite);
    }
}
