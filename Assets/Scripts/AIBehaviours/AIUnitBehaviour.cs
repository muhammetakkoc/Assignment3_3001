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
    public string teamTag; // Tag to distinguish team

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

        // Aim turret at the target
        Vector3 targetDirection = target.transform.position - turret.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime * speed);

        // Move towards the target
        //Vector3 direction = (target.transform.position - transform.position).normalized;
        //transform.position += direction * speed * Time.deltaTime;

        // Fire at the target
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


//[Range(1, 15)]
//[SerializeField] private float showRadius = 11;
//[SerializeField] private float detectionCheckDelay = 0.1f;
//private Transform target = null;
//[SerializeField] private LayerMask playerLayerMask;
//[SerializeField] private LayerMask visibiltyLayer;

//public bool TargetVisible { get; private set; }

//private void Start()
//{
//    StartCoroutine(DetectionCoroutine());
//}
//public Transform Target
//{
//    get => target;
//    set
//    {
//        target = value;
//        TargetVisible = false;
//    }
//}

//private void Update()
//{
//    if (Target != null)
//        TargetVisible = CheckTargetVisible();
//}

//private bool CheckTargetVisible()
//{
//    var result = Physics2D.Raycast(transform.position, Target.position - transform.position, showRadius, visibiltyLayer);
//    if(result.collider != null)
//    {
//        return (playerLayerMask & (1 << result.collider.gameObject.layer)) != 0;
//    }
//    return false;
//}
//private void DetectTarget()
//{
//    if(Target== null)
//    {
//        CheckIfPlayerInRange();
//    }
//    else if (Target != null)
//    {
//        DetectIfOutOfRange();
//    }
//}

//private void DetectIfOutOfRange()
//{
//    if(Target== null || Target.gameObject.activeSelf == false || Vector2.Distance(transform.position, Target.position) > showRadius)
//    {
//        Target = null;
//    }
//}

//private void CheckIfPlayerInRange()
//{
//    Collider2D collision = Physics2D.OverlapCircle(transform.position, showRadius, playerLayerMask);
//    if(collision != null)
//    {
//        Target = collision.transform;
//    }
//}

//IEnumerator DetectionCoroutine()
//{
//    yield return new WaitForSeconds(detectionCheckDelay);
//    DetectTarget();
//    StartCoroutine(DetectionCoroutine());

//}

//private void OnDrawGizmos()
//{
//    Gizmos.DrawWireSphere(transform.position, showRadius);
//}



