using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth;
    [SerializeField] private int startingHealth;

    [SerializeField] private float fillSpeed = 2.0f;

    [FormerlySerializedAs("handsDeactivation")] [SerializeField] private GameObject handsObject;

    private int maxHealth;

    private float targetFill = 1.0f;
    private float currentFill = 1.0f;
    
    private bool isDead;

    private Image healthBar;

    private UIManager UiManager;


    private void Start()
    {
        UiManager = UIManager.Instance;

        var talent = Player.Instance.Equipment.HasIncreasedMaxHealth();
        maxHealth = startingHealth * (int)(talent ? talent.value : 1.0f);
        currentHealth = maxHealth;
        Debug.LogFormat("Starting: {0} Current: {1} Max: {2}", startingHealth, currentHealth, maxHealth);

        healthBar = UiManager.HealthBar.GetComponentInChildren<Image>();

        UpdateHealthUI();
    }

    private void Update()
    {
        if (!healthBar || isDead) return;

        var t = Time.deltaTime * fillSpeed;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetFill, t);
    }

    public void DamagePlayer(int DamageAmount)
    {
        if (isDead) return;

        Debug.LogFormat("DamageReceived! {0}", DamageAmount);
        currentHealth -= DamageAmount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            isDead = true;
            PlayerDeath();
            UiManager.ToggleHealthBar(false);
        }
    }
    
    public void UpdateHealthUI()
    {
        targetFill = (float)currentHealth / maxHealth;
        //Debug.LogFormat("Current: {0} Max: {1} Div: {2}", currentHealth, maxHealth, ((float)currentHealth/maxHealth));
        //healthBar.fillAmount = ((float)currentHealth / maxHealth); ;
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
