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

    // TODO: General data like the race the player is, should be moved in a general 'Player' script singleton.
    /// <summary>
    /// If the saved value is 0, player is cyborg.
    /// If the saved value is 1, player is human.
    /// </summary>
    public const string PLAYER_SKIN_KEY = "PLAYER_SELECTED_SKIN";
    
    public void Init()
    {
        var playerSkin = PlayerPrefs.GetInt(PLAYER_SKIN_KEY, 0);
        var playerIsCyborg = playerSkin == 0;
        
        playerAnimator = playerIsCyborg ? cyborgAnimator : humanoidAnimator;
        playerAnimatorForShadows = playerIsCyborg ? cyborgAnimatorForShadows : humanoidAnimatorForShadows;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        throw new NotImplementedException();
    }
}
