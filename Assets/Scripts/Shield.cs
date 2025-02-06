using UnityEngine;

public class Shield : MonoBehaviour
{
    private int health = 2;

   
    void OnCollisionEnter(Collision collision)
    {
        
        
        
        if (collision.collider.CompareTag("Enemy Bullet"))
        {
            for (int i = 0; i < health; i++)
            {
                Destroy(gameObject);
            }
                
        }
    }
}

