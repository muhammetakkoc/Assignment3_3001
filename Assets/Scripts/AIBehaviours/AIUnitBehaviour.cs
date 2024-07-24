//This is the only MonoBehaviour-derived class you may modify for this challenge
//By Joss Moo-Young
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnitBehaviour : MonoBehaviour
{
    public float speed = 5f;
    public float DetectionRange = 100f;
    public LayerMask enemyLayer;
    public Transform turret;
    public Transform gun;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 20f;

    private GameObject target = null;
    public string teamTag; 

    private float nextFireTime = 0f;

    void Start()
    {
        //if (turret == null || gun == null)
        //{
        //    Debug.LogError("Turret and Gun references are required.");
        //}
    }

    void Update()
    {
        DetectEnemies();

        if (target != null)
        {
            Attack();
        }
    }

    void DetectEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRange, enemyLayer);
        float closestDistance = DetectionRange;
        GameObject closestEnemy = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != this.gameObject && hitCollider.tag != this.tag && hitCollider.tag != teamTag)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distanceToEnemy < closestDistance && HasLineOfSight(hitCollider.transform))
                {
                    closestDistance = distanceToEnemy;
                    closestEnemy = hitCollider.gameObject;
                }
            }
        }

        target = closestEnemy;
    }

    bool HasLineOfSight(Transform targetTransform)
    {
        Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
        Ray ray = new Ray(transform.position, directionToTarget);
        if (Physics.Raycast(ray, out RaycastHit hit, DetectionRange))
        {
            if (hit.transform == targetTransform)
            {
                return true;
            }
        }
        return false;
    }

    void Attack()
    {
        if (target == null) return;

        
        Vector3 targetDirection = target.transform.position - turret.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime * speed);

        
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }

        if (!HasLineOfSight(target.transform))
        {
            target = null;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
    }
}

