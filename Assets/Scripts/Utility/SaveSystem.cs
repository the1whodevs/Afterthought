using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveSystem
{
    public static string SAVES_DIR => $"{Application.persistentDataPath}/";
    public static string QUICKSAVE_DIR => $"{SAVES_DIR}afterthought_-1{SAVE_EXT}";
    public static string SAVE_EXT = ".atsav";
    public static string SCREENSHOT_EXT = ".png";

    public static void Save(int saveIndex, int currentLevel, int playerLevel, int playerXP, int playerHP, Vector3 playerPos, Quaternion playerRot, float cameraRotY, float bodyRotX, Vector3 camPlayerOffset, int objectiveIndex, LoadoutData[] allLoadouts, WeaponData[] allWeapons, AmmoData[] allAmmo, EquipmentData[] allEquipment, TalentData[] allTalents, EmeraldAI.EmeraldAISystem[] allAIs, ILootable[] lootables)
    {
        var data = new SaveData(currentLevel, playerLevel, playerXP, playerHP, playerPos, playerRot, cameraRotY, bodyRotX, camPlayerOffset, objectiveIndex, allLoadouts, allWeapons, allAmmo, allEquipment, allTalents, allAIs, lootables);

        var bf = new BinaryFormatter();
        var pathNoExt = $"{SAVES_DIR}afterthought_{saveIndex}";
        var path = $"{pathNoExt}{SAVE_EXT}";
        var screenshotPath = $"{pathNoExt}{SCREENSHOT_EXT}";

        TakeScreenshot(screenshotPath);

        var filestream = new FileStream(path, FileMode.Create);

        bf.Serialize(filestream, data);
        filestream.Close();
    }

    public static void DeleteAt(int index)
    {
        var path = $"{SAVES_DIR}afterthought_{index}{SAVE_EXT}";
        var screenshotPath = $"{SAVES_DIR}afterthought_{index}{SCREENSHOT_EXT}";

        if (File.Exists(path)) File.Delete(path);
        if (File.Exists(screenshotPath)) File.Delete(screenshotPath);
    }

    public static void TakeScreenshot(string path)
    {
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log(path);
    }

    public static Sprite GetScreenshot(int saveIndex)
    {
        var path = $"{SAVES_DIR}afterthought_{saveIndex}{SCREENSHOT_EXT}";

        return LoadNewSprite(path);
    }

    public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
    {
        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        Texture2D SpriteTexture = LoadTexture(FilePath);
        Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);

        return NewSprite;
    }

    public static Texture2D LoadTexture(string FilePath)
    {
        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath))
        {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    public static SaveData Load(int saveIndex)
    {
        var path = $"{Application.persistentDataPath}/afterthought_{saveIndex}{SAVE_EXT}";

        if (File.Exists(path))
        {
            var bf = new BinaryFormatter();
            var filestream = new FileStream(path, FileMode.Open);

            var data = bf.Deserialize(filestream) as SaveData;

            filestream.Close();

            return data;
        }

        return null;
    }

    [System.Serializable]
    public class SaveData
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

        public readonly float cameraRotationY;
        public readonly float bodyRotationX;
        public readonly float cameraPlayerOffset_X;
        public readonly float cameraPlayerOffset_Y;
        public readonly float cameraPlayerOffset_Z;

        public readonly int objectiveIndex;

        // 0 is Equipped, 1 to 5 is Loadouts 1 to 5
        public readonly int[] loadoutsWepAindex;
        public readonly int[] loadoutsWepBindex;

        public readonly int[] loadoutsEqAindex;
        public readonly int[] loadoutsEqBindex;

        public readonly int[] loadoutsTalAindex;
        public readonly int[] loadoutsTalBindex;
        public readonly int[] loadoutsTalCindex;

        public readonly int[] weaponsAmmoInMag;
        public readonly int[] ammoTypesCurrentAmmo;
        public readonly int[] equipmentAmmo;

        public readonly bool[] weaponsLootStatus;
        public readonly bool[] lootablesLootStatus;
        public readonly bool[] lootablesObjectStatus;

        public readonly bool[] aiObjectStatus;
        public readonly int[] aiHP;

        public readonly float[] aiPosition_X;
        public readonly float[] aiPosition_Y;
        public readonly float[] aiPosition_Z;

        public readonly float[] aiRotation_X;
        public readonly float[] aiRotation_Y;
        public readonly float[] aiRotation_Z;
        public readonly float[] aiRotation_W;

        public SaveData(int currentLevel, int playerLevel, int playerXP, int playerHP, Vector3 playerPos, Quaternion playerRot, float camRotY, float bodyRotX, Vector3 cameraPlayerOffset, int objectiveIndex, LoadoutData[] allLoadouts, WeaponData[] allWeapons, AmmoData[] allAmmo, EquipmentData[] allEquipment, TalentData[] allTalents, EmeraldAI.EmeraldAISystem[] allAIs, ILootable[] lootables)
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

            this.cameraRotationY = camRotY;
            this.bodyRotationX = bodyRotX;

            this.cameraPlayerOffset_X = cameraPlayerOffset.x;
            this.cameraPlayerOffset_Y = cameraPlayerOffset.y;
            this.cameraPlayerOffset_Z = cameraPlayerOffset.z;

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

            var wepLootTemp = new List<bool>();
            var wepAmmoInMagTemp = new List<int>();

            for (var i = 0; i < allWeapons.Length; i++)
            {
                wepLootTemp.Add(allWeapons[i].isLooted);
                wepAmmoInMagTemp.Add(allWeapons[i].ammoInMagazine);
            }

            weaponsLootStatus = wepLootTemp.ToArray();
            weaponsAmmoInMag = wepAmmoInMagTemp.ToArray();

            var ammoTypesCurrentAmmoTemp = new List<int>();
            for (var i = 0; i < allAmmo.Length; i++)
                ammoTypesCurrentAmmoTemp.Add(allAmmo[i].currentAmmo);

            ammoTypesCurrentAmmo = ammoTypesCurrentAmmoTemp.ToArray();

            var equipmentCurrentAmmoTemp = new List<int>();
            for (var i = 0; i < allEquipment.Length; i++)
                equipmentCurrentAmmoTemp.Add(allEquipment[i].currentAmmo);

            equipmentAmmo = equipmentCurrentAmmoTemp.ToArray();

            lootablesLootStatus = new bool[lootables.Length];
            lootablesObjectStatus = new bool[lootables.Length];

            for (var i = 0; i < lootablesLootStatus.Length; i++)
            {
                lootablesLootStatus[i] = lootables[i].IsLooted;
                lootablesObjectStatus[i] = lootables[i].gameObject.activeSelf;
            }

            aiHP = new int[allAIs.Length];

            aiPosition_X = new float[allAIs.Length];
            aiPosition_Y = new float[allAIs.Length];
            aiPosition_Z = new float[allAIs.Length];

            aiRotation_X = new float[allAIs.Length];
            aiRotation_Y = new float[allAIs.Length];
            aiRotation_Z = new float[allAIs.Length];
            aiRotation_W = new float[allAIs.Length];

            aiObjectStatus = new bool[allAIs.Length];

            for (var i = 0; i < allAIs.Length; i++)
            {
                aiObjectStatus[i] = allAIs[i].gameObject.activeSelf;

                aiHP[i] = allAIs[i].CurrentHealth;

                var pos = allAIs[i].transform.position;
                aiPosition_X[i] = pos.x;
                aiPosition_Y[i] = pos.y;
                aiPosition_Z[i] = pos.z;

                var rot = allAIs[i].transform.rotation;
                aiRotation_X[i] = rot.x;
                aiRotation_Y[i] = rot.y;
                aiRotation_Z[i] = rot.z;
                aiRotation_W[i] = rot.w;
            }
        }

        public static int WeaponToIndex(WeaponData data, WeaponData[] allWeapons)
        {
            for (var i = 0; i < allWeapons.Length; i++)
                if (allWeapons[i] == data) return i;

            return -1;
        }

        public static int EquipmentToIndex(EquipmentData data, EquipmentData[] allEquipment)
        {
            for (var i = 0; i < allEquipment.Length; i++)
                if (allEquipment[i] == data) return i;

            return -1;
        }

        public static int TalentToIndex(TalentData data, TalentData[] allEquipment)
        {
            for (var i = 0; i < allEquipment.Length; i++)
                if (allEquipment[i] == data) return i;

            return -1;
        }
    }
}