using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int startingHealth;

    private void Start()
    {
        currentHealth = startingHealth;
    }
    
    public void DamagePlayer(int DamageAmount)
    {
        currentHealth -= DamageAmount;
        
        if (currentHealth<=0)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        Debug.Log("Player Died");
    }
}
