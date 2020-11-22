using System;
using UnityEngine;

/// <summary>
/// This should only be access through Player.instance.Animator!
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator cyborgAnimator;
    [SerializeField] private Animator cyborgAnimatorForShadows;
    
    [SerializeField] private Animator humanoidAnimator;
    [SerializeField] private Animator humanoidAnimatorForShadows;

    private Animator playerAnimator;
    private Animator playerAnimatorForShadows;

    public void Init(Player.PlayerRace race)
    {
        var playerIsCyborg = race == Player.PlayerRace.Cyborg;
        
        playerAnimator = playerIsCyborg ? cyborgAnimator : humanoidAnimator;
        playerAnimatorForShadows = playerIsCyborg ? cyborgAnimatorForShadows : humanoidAnimatorForShadows;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        throw new NotImplementedException();
    }
}
