using System;
using System.Collections.Generic;

public class Sorter : IComparer<WeaponData>, IComparer<EquipmentData>, IComparer<TalentData>
{
    public static WeaponData[] SortWeaponDataAZ(WeaponData[] toSort)
    {
        Array.Sort(toSort, new Sorter().CompareAZ);
        return toSort;
    }

    public static EquipmentData[] SortEquipmentDataAZ(EquipmentData[] toSort)
    {
        Array.Sort(toSort, new Sorter().CompareAZ);
        return toSort;
    }

    public static TalentData[] SortTalentDataAZ(TalentData[] toSort)
    {
        Array.Sort(toSort, new Sorter().CompareAZ);
        return toSort;
    }

    public static WeaponData[] SortWeaponData(WeaponData[] toSort)
    {
        Array.Sort(toSort, new Sorter().Compare);
        return toSort;
    }

    public static EquipmentData[] SortEquipmentData(EquipmentData[] toSort)
    {
        Array.Sort(toSort, new Sorter().Compare);
        return toSort;
    }

    public static TalentData[] SortTalentData(TalentData[] toSort)
    {
        Array.Sort(toSort, new Sorter().Compare);
        return toSort;
    }

    public int Compare(TalentData x, TalentData y)
    {
        if (x.isUnlocked && y.isUnlocked) return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);

        if (x.isUnlocked) return -1;
        if (y.isUnlocked) return 1;

        // If the talents unlock the same way...
        if (x.unlockType.Equals(y.unlockType))
        {
            switch (x.unlockType)
            {
                case UnlockType.Level:
                    if (x.levelRequired < y.levelRequired) return -1;
                    if (x.levelRequired > y.levelRequired) return 1;
                    return 0;

                case UnlockType.Loot:
                    // Compare based on name, Aa to Zz
                    return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);

                case UnlockType.AlwaysUnlocked:                    
                    // Compare based on name, Aa to Zz
                    return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        // Since the two are not the same type...

        // ...if X is 'AlwaysUnlocked' it should be higher than y.
        if (x.unlockType.Equals(UnlockType.AlwaysUnlocked)) return -1;

        // ...and since x is NOT 'always unlocked', if Y is 'AlwaysUnlocked' it should be higher than x.
        if (y.unlockType.Equals(UnlockType.AlwaysUnlocked)) return 1;

        // ...if none of the two are 'AlwaysUnlocked', then 'LevelRequired' should be higher than loot.
        if (y.unlockType.Equals(UnlockType.Loot)) return -1;
        
        // Since the two are not the same, and none of them are 'AlwaysUnlocked', and y is not 'Loot', it means that x is 'Loot'
        return 1;
    }

    public int Compare(EquipmentData x, EquipmentData y)
    {
        if (x.isUnlocked && y.isUnlocked) return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);

        if (x.isUnlocked) return -1;
        if (y.isUnlocked) return 1;

        // If the talents unlock the same way...
        if (x.unlockType.Equals(y.unlockType))
        {
            switch (x.unlockType)
            {
                case UnlockType.Level:
                    if (x.levelRequired < y.levelRequired) return -1;
                    if (x.levelRequired > y.levelRequired) return 1;
                    return 0;

                case UnlockType.Loot:
                    // Compare based on name, Aa to Zz
                    return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);

                case UnlockType.AlwaysUnlocked:
                    // Compare based on name, Aa to Zz
                    return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        // Since the two are not the same type...

        // ...if X is 'AlwaysUnlocked' it should be higher than y.
        if (x.unlockType.Equals(UnlockType.AlwaysUnlocked)) return -1;

        // ...and since x is NOT 'always unlocked', if Y is 'AlwaysUnlocked' it should be higher than x.
        if (y.unlockType.Equals(UnlockType.AlwaysUnlocked)) return 1;

        // ...if none of the two are 'AlwaysUnlocked', then 'LevelRequired' should be higher than loot.
        if (y.unlockType.Equals(UnlockType.Loot)) return -1;

        // Since the two are not the same, and none of them are 'AlwaysUnlocked', and y is not 'Loot', it means that x is 'Loot'
        return 1;
    }

    public int Compare(WeaponData x, WeaponData y)
    {
        if (x.isUnlocked && y.isUnlocked) return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);

        if (x.isUnlocked) return -1;
        if (y.isUnlocked) return 1;

        // If the talents unlock the same way...
        if (x.unlockType.Equals(y.unlockType))
        {
            switch (x.unlockType)
            {
                case UnlockType.Level:
                    if (x.levelRequired < y.levelRequired) return -1;
                    if (x.levelRequired > y.levelRequired) return 1;
                    return 0;

                case UnlockType.Loot:
                    // Compare based on name, Aa to Zz
                    return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);

                case UnlockType.AlwaysUnlocked:
                    // Compare based on name, Aa to Zz
                    return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);
            }
        }

        // Since the two are not the same type...

        // ...if X is 'AlwaysUnlocked' it should be higher than y.
        if (x.unlockType.Equals(UnlockType.AlwaysUnlocked)) return -1;

        // ...and since x is NOT 'always unlocked', if Y is 'AlwaysUnlocked' it should be higher than x.
        if (y.unlockType.Equals(UnlockType.AlwaysUnlocked)) return 1;

        // ...if none of the two are 'AlwaysUnlocked', then 'LevelRequired' should be higher than loot.
        if (y.unlockType.Equals(UnlockType.Loot)) return -1;

        // Since the two are not the same, and none of them are 'AlwaysUnlocked', and y is not 'Loot', it means that x is 'Loot'
        return 1;
    }

    public int CompareAZ(WeaponData x, WeaponData y)
    {
        return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);
    }

    public int CompareAZ(EquipmentData x, EquipmentData y)
    {
        return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);
    }

    public int CompareAZ(TalentData x, TalentData y)
    {
        return string.Compare(x.name, y.name, false, System.Globalization.CultureInfo.CurrentCulture);
    }
}
