using UnityEngine;

public class Shield : MonoBehaviour
{
    private int health;

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

