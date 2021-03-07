using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOfRuinsManager : MonoBehaviour
{
    [SerializeField] private LoadoutData battleOfRuinsData;

    [Header("OnObjectiveComplete Actions")]
    [SerializeField] private OnObjectiveCompleteWrapper[] onObjectiveCompleteActions;

    [Header("First Defence")]
    [SerializeField] private GameObject[] FirstDefenseTargets;

    [Header("Second Defence")]
    [SerializeField] private GameObject[] SecondDefenseTargets;

    [Header("Stairs Defence")]
    [SerializeField] private GameObject[] StairsDefenseTargets;

    [Header("Last Defence")]
    [SerializeField] private GameObject[] LastDefenseTargets;

}
