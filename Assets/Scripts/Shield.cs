using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private int health = 2; // Initial health for the shield

    void OnCollisionEnter(Collision collision)
    {
        // Check if the shield collides with an enemy bullet
        if (collision.collider.CompareTag("Enemy Bullet"))
        {
            Debug.Log("Bullet Hit Shield");
            health--;
            if (health <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}