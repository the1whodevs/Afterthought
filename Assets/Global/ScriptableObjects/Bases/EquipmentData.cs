using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Veejay/Equipment", fileName = "Equipment")]
public class EquipmentData : ScriptableObject
{
    public new string name = "Amazing Potato";
    
    public string description = "Grenade that looks like a potato.";
    
    public GameObject prefab;
    
    public Image icon;
}
