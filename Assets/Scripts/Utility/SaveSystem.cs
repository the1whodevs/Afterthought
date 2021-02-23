using UnityEngine;

public static class SaveSystem
{
    private class SaveData
    {
        public readonly int level;
        public readonly int playerLevel;
        public readonly int playerXP;

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

        public readonly int[] weaponsUnlockStatus;
        public readonly int[] weaponsAmmoInMag;
        public readonly int[] ammoTypesCurrentAmmo;
        public readonly int[] equipmentAmmo;

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

        public SaveData(int currentLevel, int playerLevel, int playerXP, 
            Vector3 playerPos, Quaternion playerRot, int objectiveIndex,
            LoadoutData[] allLoadouts, WeaponData[] allWeapons, AmmoData[] allAmmo, 
            EquipmentData[] allEquipment, EmeraldAI.EmeraldAISystem[] allAIs)
        {
            // TODO: Add ILootable with an isLooted bool, and pass the array here.
        }
    }
}
