using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private class SaveData
    {
        public readonly int level;
        public readonly int playerLevel;
        public readonly int playerXP;
        public readonly int playerHP;

        public readonly float playerPosition_X;
        public readonly float playerPosition_Y;
        public readonly float playerPosition_Z;

        public readonly float playerRotation_X;
        public readonly float playerRotation_Y;
        public readonly float playerRotation_Z;
        public readonly float playerRotation_W;

        public readonly int objectiveIndex;

        // 0 is Equipped, 1 to 5 is Loadouts 1 to 5
        public readonly int[] loadoutsWepAindex;
        public readonly int[] loadoutsWepBindex;

        public readonly int[] loadoutsEqAindex;
        public readonly int[] loadoutsEqBindex;

        public readonly int[] loadoutsTalAindex;
        public readonly int[] loadoutsTalBindex;
        public readonly int[] loadoutsTalCindex;

        public readonly int[] weaponsLootStatus;
        public readonly int[] weaponsAmmoInMag;
        public readonly int[] ammoTypesCurrentAmmo;
        public readonly int[] equipmentAmmo;
        // -- Done ---^


        // -- TODO --v
        public readonly int[] lootablesLootStatus;
        public readonly int[] aiHP;
        public readonly int[] aiState;

        public readonly float[] aiPosition_X;
        public readonly float[] aiPosition_Y;
        public readonly float[] aiPosition_Z;

        public readonly float[] aiRotation_X;
        public readonly float[] aiRotation_Y;
        public readonly float[] aiRotation_Z;
        public readonly float[] aiRotation_W;

        public SaveData(int currentLevel, int playerLevel, int playerXP, int playerHP, Vector3 playerPos, Quaternion playerRot, int objectiveIndex, LoadoutData[] allLoadouts, WeaponData[] allWeapons, AmmoData[] allAmmo,             EquipmentData[] allEquipment, TalentData[] allTalents, EmeraldAI.EmeraldAISystem[] allAIs, ILootable[] lootables)
        {
            this.level = currentLevel;
            this.playerLevel = playerLevel;
            this.playerXP = playerXP;
            this.playerHP = playerHP;
            this.playerPosition_X = playerPos.x;
            this.playerPosition_Y = playerPos.y;
            this.playerPosition_Z = playerPos.z;
            this.playerRotation_X = playerRot.x;
            this.playerRotation_Y = playerRot.y;
            this.playerRotation_Z = playerRot.z;
            this.playerRotation_W = playerRot.w;
            this.objectiveIndex = objectiveIndex;

            allWeapons = Sorter.SortWeaponDataAZ(allWeapons);
            allEquipment = Sorter.SortEquipmentDataAZ(allEquipment);
            allTalents = Sorter.SortTalentDataAZ(allTalents);

            this.loadoutsWepAindex = new int[]
            {
                WeaponToIndex(allLoadouts[0].Weapons[0], allWeapons),
                WeaponToIndex(allLoadouts[1].Weapons[0], allWeapons),
                WeaponToIndex(allLoadouts[2].Weapons[0], allWeapons),
                WeaponToIndex(allLoadouts[3].Weapons[0], allWeapons),
                WeaponToIndex(allLoadouts[4].Weapons[0], allWeapons),
                WeaponToIndex(allLoadouts[5].Weapons[0], allWeapons),
            };

            this.loadoutsWepBindex = new int[]
           {
                WeaponToIndex(allLoadouts[0].Weapons[1], allWeapons),
                WeaponToIndex(allLoadouts[1].Weapons[1], allWeapons),
                WeaponToIndex(allLoadouts[2].Weapons[1], allWeapons),
                WeaponToIndex(allLoadouts[3].Weapons[1], allWeapons),
                WeaponToIndex(allLoadouts[4].Weapons[1], allWeapons),
                WeaponToIndex(allLoadouts[5].Weapons[1], allWeapons),
           };

            this.loadoutsEqAindex = new int[]
            {
                EquipmentToIndex(allLoadouts[0].Equipment[0], allEquipment),
                EquipmentToIndex(allLoadouts[1].Equipment[0], allEquipment),
                EquipmentToIndex(allLoadouts[2].Equipment[0], allEquipment),
                EquipmentToIndex(allLoadouts[3].Equipment[0], allEquipment),
                EquipmentToIndex(allLoadouts[4].Equipment[0], allEquipment),
                EquipmentToIndex(allLoadouts[5].Equipment[0], allEquipment),
            };

            this.loadoutsEqBindex = new int[]
           {
                EquipmentToIndex(allLoadouts[0].Equipment[1], allEquipment),
                EquipmentToIndex(allLoadouts[1].Equipment[1], allEquipment),
                EquipmentToIndex(allLoadouts[2].Equipment[1], allEquipment),
                EquipmentToIndex(allLoadouts[3].Equipment[1], allEquipment),
                EquipmentToIndex(allLoadouts[4].Equipment[1], allEquipment),
                EquipmentToIndex(allLoadouts[5].Equipment[1], allEquipment),
           };

            this.loadoutsTalAindex = new int[]
            {
                TalentToIndex(allLoadouts[0].Talents[0], allTalents),
                TalentToIndex(allLoadouts[1].Talents[0], allTalents),
                TalentToIndex(allLoadouts[2].Talents[0], allTalents),
                TalentToIndex(allLoadouts[3].Talents[0], allTalents),
                TalentToIndex(allLoadouts[4].Talents[0], allTalents),
                TalentToIndex(allLoadouts[5].Talents[0], allTalents),
            };

            this.loadoutsTalBindex = new int[]
           {
                TalentToIndex(allLoadouts[0].Talents[1], allTalents),
                TalentToIndex(allLoadouts[1].Talents[1], allTalents),
                TalentToIndex(allLoadouts[2].Talents[1], allTalents),
                TalentToIndex(allLoadouts[3].Talents[1], allTalents),
                TalentToIndex(allLoadouts[4].Talents[1], allTalents),
                TalentToIndex(allLoadouts[5].Talents[1], allTalents),
           };

            this.loadoutsTalCindex = new int[]
           {
                TalentToIndex(allLoadouts[0].Talents[2], allTalents),
                TalentToIndex(allLoadouts[1].Talents[2], allTalents),
                TalentToIndex(allLoadouts[2].Talents[2], allTalents),
                TalentToIndex(allLoadouts[3].Talents[2], allTalents),
                TalentToIndex(allLoadouts[4].Talents[2], allTalents),
                TalentToIndex(allLoadouts[5].Talents[2], allTalents),           
           };

            var wepLootTemp = new List<int>();
            var wepAmmoInMagTemp = new List<int>();
            for (var i = 0; i < allWeapons.Length; i++)
            {
                wepLootTemp.Add(allWeapons[i].isLooted ? 1 : 0);
                wepAmmoInMagTemp.Add(allWeapons[i].ammoInMagazine);
            }

            var ammoTypesCurrentAmmoTemp = new List<int>();
            for (var i = 0; i < allAmmo.Length; i++)
                ammoTypesCurrentAmmoTemp.Add(allAmmo[i].currentAmmo);

            var equipmentCurrentAmmoTemp = new List<int>();
            for (var i = 0; i < allEquipment.Length; i++)
                equipmentCurrentAmmoTemp.Add(allEquipment[i].currentAmmo);

        }

        private int WeaponToIndex(WeaponData data, WeaponData[] allWeapons)
        {
            for (var i = 0; i < allWeapons.Length; i++)
                if (allWeapons[i] == data) return i;

            return -1;
        }

        private int EquipmentToIndex(EquipmentData data, EquipmentData[] allEquipment)
        {
            for (var i = 0; i < allEquipment.Length; i++)
                if (allEquipment[i] == data) return i;

            return -1;
        }

        private int TalentToIndex(TalentData data, TalentData[] allEquipment)
        {
            for (var i = 0; i < allEquipment.Length; i++)
                if (allEquipment[i] == data) return i;

            return -1;
        }
    }
}
