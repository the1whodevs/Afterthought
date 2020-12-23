using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int startingHealth;
    [FormerlySerializedAs("handsDeactivation")] [SerializeField] private GameObject handsObject;

    private void Start()
    {
        currentHealth = startingHealth;
    }
    
    public void DamagePlayer(int DamageAmount)
    {
        currentHealth -= DamageAmount;
        
        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Player.Instance.PostProcessing.Death();
        handsObject.SetActive(false);
        CameraAnimation.Instance.DeathAnimation();
        MouseCamera.Instance.enabled = false;
        Player.Instance.Controller.enabled = false;
    }
}
