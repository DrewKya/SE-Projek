using UnityEngine;
public class GameHandler : MonoBehaviour
{
    public GameObject Player;
    public int health;
    public int Damage = 40;
    public int healthMax = 100;
    public HealthBar healthBar;

    public void Start()
    {
        health = healthMax;
        healthBar.SetMaxHealth(healthMax);
    }

   public void gotDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0) health = 0;

        healthBar.SetHealth(health);
    }
}
