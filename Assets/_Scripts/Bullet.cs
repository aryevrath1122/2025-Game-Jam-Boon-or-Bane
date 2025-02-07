using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    public float speed = 20f;

    private Rigidbody rb;
    public float lifetime = 10f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = -transform.right * speed;
        Destroy(gameObject, lifetime); // Destroy after 'lifetime' seconds
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
