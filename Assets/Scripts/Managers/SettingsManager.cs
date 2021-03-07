using UnityEngine;

public class SettingsManager : MonoSingleton<SettingsManager>
{
    private void Awake()
    {
        if (Active && Active == this) DontDestroyOnLoad(this);
        else
        {
            Debug.LogError("Another SettingsManager instance has been found!");
            Destroy(this);
        }
    }
}
