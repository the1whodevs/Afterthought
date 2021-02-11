using System;
using System.Collections;
using UnityEngine;

public class TrainingManager : MonoSingleton<TrainingManager, ReportMissingInstanceError>
{
    [SerializeField] private LoadoutData trainingLoadout;

    public static LoadoutData GetTrainingLoadout()
    {
        return Active.trainingLoadout;
    }
}
