using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    public float speed = 20f;

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
