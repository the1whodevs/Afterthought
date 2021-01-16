using UnityEngine;

public class LoadoutEditor : MonoBehaviour
{
    public static LoadoutEditor Instance
    {
        get
        {
            if (_instance) return _instance;

            _instance = FindObjectOfType<LoadoutEditor>();

            if (!_instance) throw new System.Exception("LoadoutEditor instance not found!");

            return _instance;
        }
    }

    private static LoadoutEditor _instance;

    [SerializeField] private LoadoutData[] playerLoadouts;

    [SerializeField] private WeaponDisplay wepDisplayA;
    [SerializeField] private WeaponDisplay wepDisplayB;

    [SerializeField] private EquipmentDisplay equipDisplayA;
    [SerializeField] private EquipmentDisplay equipDisplayB;

    [SerializeField] private TalentDisplay talentDisplayA;
    [SerializeField] private TalentDisplay talentDisplayB;
    [SerializeField] private TalentDisplay talentDisplayC;

    private LoadoutData displayedLoadout;

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Debug.LogError("Another instance of loadout editor already exists!");
            Destroy(this);
        }
        else if (!_instance) _instance = this;

        HideWindow();
    }

    public void ShowWindow()
    {
        if (!displayedLoadout) displayedLoadout = playerLoadouts[0];

        UpdateDisplays();

        gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideWindow()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameObject.SetActive(false);
    }

    public void UpdateDisplays()
    {
        if (!displayedLoadout) throw new System.Exception("Update Displays called but displayedLoadout is null!");

        wepDisplayA.SetItemToDisplay(displayedLoadout.Weapons[0]);
        wepDisplayB.SetItemToDisplay(displayedLoadout.Weapons[1]);

        equipDisplayA.SetItemToDisplay(displayedLoadout.Equipment[0]);
        equipDisplayB.SetItemToDisplay(displayedLoadout.Equipment[1]);

        talentDisplayA.SetItemToDisplay(displayedLoadout.Talents[0]);
        talentDisplayB.SetItemToDisplay(displayedLoadout.Talents[1]);
        talentDisplayC.SetItemToDisplay(displayedLoadout.Talents[2]);

        wepDisplayA.UpdateDisplay();
        wepDisplayB.UpdateDisplay();

        equipDisplayA.UpdateDisplay();
        equipDisplayB.UpdateDisplay();

        talentDisplayA.UpdateDisplay();
        talentDisplayB.UpdateDisplay();
        talentDisplayC.UpdateDisplay();
    }

    public void ChangeLoadout(int loadoutIndex)
    {
        displayedLoadout = playerLoadouts[loadoutIndex];

        UpdateDisplays();
    }
}
