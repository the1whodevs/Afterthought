using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int startingHealth;
    [FormerlySerializedAs("handsDeactivation")] [SerializeField] private GameObject handsObject;

    private bool isDead;
    
    private void Start()
    {
        currentHealth = startingHealth;
    }
    
    public void DamagePlayer(int DamageAmount)
    {
        if (isDead) return;
        
        currentHealth -= DamageAmount;
        
        if (currentHealth <= 0)
        {
            isDead = true;
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Player.Instance.PostProcessing.Death();
        handsObject.SetActive(false);
        MouseCamera.Instance.enabled = false;
        Player.Instance.Controller.enabled = false;
        CameraAnimation.Instance.DeathAnimation();
    }
}
