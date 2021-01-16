using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Equipment", fileName = "Equipment")]
public class EquipmentData : IDisplayableItem
{    
    public GameObject prefab;
    public GameObject prefabToThrow;
    public GameObject explosionPrefab;
    public GameObject explosionDecal;

    public int currentAmmo = 3;
    public int magazineCapacity = 3;

    public float damage = 100.0f;
    public float minRange = 2.5f;
    public float maxRange = 7.5f;
    public float mobilityPenalty = 0.0f;
    public float throwForce = 50.0f;
    public float fuseTime = 3.0f;
    public float ragdollForce = 150.0f;

    private string AMMO_IN_MAG => $"{name}_AMMOinMAG";

    public override void SaveData()
    {
        base.SaveData();
        PlayerPrefs.SetInt(AMMO_IN_MAG, currentAmmo);
    }

    public override void LoadData()
    {
        base.LoadData();
        currentAmmo = PlayerPrefs.GetInt(AMMO_IN_MAG, magazineCapacity);
    }
}
