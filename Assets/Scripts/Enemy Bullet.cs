using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 targetDirection;
    
    public float lifeTime = 5f; // Bullet destroys itself after 5 seconds

    private void Start()
    {
        FindNearestPlayer();
        Destroy(gameObject, lifeTime); // Destroy bullet after some time
    }

    private void Update()
    {
        MoveTowardsTarget();
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            // No player found, bullet moves forward by default
            targetDirection = transform.forward;
            return;
        }

        GameObject nearestPlayer = players[0];
        float shortestDistance = Vector3.Distance(transform.position, nearestPlayer.transform.position);

        // Loop through all players to find the closest one
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestPlayer = player;
            }
        }

        // Set direction towards nearest player
        targetDirection = (nearestPlayer.transform.position - transform.position).normalized;
    }

    void MoveTowardsTarget()
    {
        transform.position += targetDirection * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject); // Destroy bullet on impact
        }
        if (collision.collider.CompareTag("Shield"))
        {
            Destroy(gameObject);
        }
    }
}
