using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Weapons/Weapon Type", fileName = "WeaponTypeData")]
public class WeaponTypeData : ScriptableObject
{
    public Sprite crosshair;

    public Vector2 crosshairDefaultXY = new Vector2(512, 512);
    public Vector2 maxRecoilCrosshairXY = new Vector2(512, 512);

    public AmmoData ammoType;
}
