using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Veejay/Weapon", fileName = "Weapon")]
public class WeaponData : ScriptableObject
{
    // TODO: Weapon image, name, description.
    
    [FormerlySerializedAs("rightHandPrefab")] 
    [Header("This is the main prefab to be used, and should never be null.")]
    public GameObject wepPrefab; 
}