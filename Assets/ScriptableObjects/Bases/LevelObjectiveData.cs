using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Veejay/Objectives/Level Objective", fileName = "NewLevelObjectiveData")]
public class LevelObjectiveData : ScriptableObject
{
    public ObjectiveData[] objectiveData;

    public void Initialize()
    {
        SortObjectivesBasedOnOrder();
    }

    private void SortObjectivesBasedOnOrder()
    {
        Array.Sort(objectiveData, new ObjectiveData.ObjectiveDataComparer().Compare);
    }
}
