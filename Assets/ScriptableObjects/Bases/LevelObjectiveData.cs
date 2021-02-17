using System;
using UnityEngine;
using static ObjectiveData;

[CreateAssetMenu(menuName = "Veejay/Level Objective", fileName = "NewLevelObjectiveData")]
public class LevelObjectiveData : ScriptableObject
{
    public ObjectiveData[] objectiveData;

    public void Initialize()
    {
        SortObjectivesBasedOnOrder();
    }

    private void SortObjectivesBasedOnOrder()
    {
        Array.Sort(objectiveData, new ObjectiveDataComparer().Compare);
    }
}
