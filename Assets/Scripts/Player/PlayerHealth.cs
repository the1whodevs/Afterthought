using System.Collections;
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
        UiManager = UIManager.Active;

        var talent = Player.Active.Loadout.HasIncreasedMaxHealth();
        maxHealth = startingHealth * (int)(talent ? talent.value : 1.0f);
        currentHealth = maxHealth;

        healthBar = UiManager.HealthBar.transform.Find("Fill").GetComponent<Image>();

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
    }

    private void PlayerDeath()
    {
        Player.Active.PostProcessing.Death();
        handsObject.SetActive(false);
        MouseCamera.Instance.enabled = false;
        Player.Active.Controller.enabled = false;
        CameraAnimation.Instance.DeathAnimation();
        StartCoroutine(ShowDeathMenuDelay());
    }

    private IEnumerator ShowDeathMenuDelay()
    {
        yield return new WaitForSeconds(1.0f);
        DeathMenu.Instance.ShowDeathMenu();
    }
}
