using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform EnemyPosition1;
    [SerializeField] private Transform EnemyPosition2;
    [SerializeField] private GameObject EnemyBullet;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float shootInterval = 2f; // Time between shots
    [SerializeField] private GameObject FirePoint;
    public float health = 2f;


    private Transform targetPosition;
    private bool movingToPosition1 = false;

    private void Start()
    {
        targetPosition = EnemyPosition2; // Start moving towards Position2
        StartCoroutine(ShootRoutine());
    }

    private void Update()
    {
        MoveEnemy();
    }

    void MoveEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

        // If the enemy reaches the target, switch to the other position
        if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            targetPosition = movingToPosition1 ? EnemyPosition2 : EnemyPosition1;
            movingToPosition1 = !movingToPosition1;
        }
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            ShootBullet();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void ShootBullet()
    {
        Instantiate(EnemyBullet, FirePoint.transform.position, transform.rotation);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player Bullet"))
        {
            Debug.Log("Enemy Hit");
            for (int i = 0; i < health; i++)
            {
                Destroy(gameObject);
            }
        }
    }
}
