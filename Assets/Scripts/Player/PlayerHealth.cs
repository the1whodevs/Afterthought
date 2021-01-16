using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int startingHealth;
    [FormerlySerializedAs("handsDeactivation")] [SerializeField] private GameObject handsObject;
    
    private Slider healthBar;

    private UIManager UiManager;

    private bool isDead;
    
    private void Start()
    {
        UiManager = UIManager.Instance;

        var talent = Player.Instance.Equipment.HasIncreasedMaxHealth();
        currentHealth = startingHealth * (int)(talent ? talent.value : 1.0f);

        healthBar = UiManager.HealthBar.GetComponent<Slider>();

        healthBar.value = currentHealth;
    }

    public void DamagePlayer(int DamageAmount)
    {
        if (isDead) return;
        
        currentHealth -= DamageAmount;
        SetHealth();
        
        if (currentHealth <= 0)
        {
            isDead = true;
            PlayerDeath();
            UiManager.ToggleHealthBar(false);
        }
    }
    
    public void SetHealth()
    {
        healthBar.maxValue = startingHealth;
        healthBar.value = currentHealth;
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
