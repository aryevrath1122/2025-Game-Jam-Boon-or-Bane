using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform EnemyPosition1;
    [SerializeField] private Transform EnemyPosition2;
    [SerializeField] private GameObject EnemyBullet;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float shootInterval = 2f; // Time between shots
    public float EnemyHealth = 20f;

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
        Instantiate(EnemyBullet, transform.position + transform.forward, transform.rotation);
    }
}
