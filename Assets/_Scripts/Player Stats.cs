using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;  // Singleton pattern to easily access the shared stats

    public float health = 100f;
    public float maxHealth = 100f;
    public float stamina = 100f;
    public float maxStamina = 100f;

    public Slider healthSlider;
    public Slider staminaSlider;

    private void Awake()
    {
        // Ensure there's only one instance of PlayerStats
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // Update the sliders with current health and stamina values
        if (healthSlider != null)
            healthSlider.value = health / maxHealth;

        if (staminaSlider != null)
            staminaSlider.value = stamina / maxStamina;
    }

    // Method to modify health and stamina from other scripts
    public void ModifyHealth(float amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
        if (health <= 0) health = 0;
    }

    public void ModifyStamina(float amount)
    {
        stamina += amount;
        if (stamina > maxStamina) stamina = maxStamina;
        if (stamina < 0) stamina = 0;
    }
}
