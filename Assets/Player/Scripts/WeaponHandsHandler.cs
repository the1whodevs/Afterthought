using System;
using UnityEngine;

public class WeaponHandsHandler : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    public void AdjustHands(PlayerEquipment.WeaponHand handsUsed)
    {
        switch (handsUsed)
        {
            case PlayerEquipment.WeaponHand.Left:
                leftHand.SetActive(true);
                rightHand.SetActive(false);
                break;
            case PlayerEquipment.WeaponHand.Right:
                leftHand.SetActive(false);
                rightHand.SetActive(true);
                break;
            case PlayerEquipment.WeaponHand.Both:
                leftHand.SetActive(true);
                rightHand.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(handsUsed), handsUsed, null);
        }
    }
}
