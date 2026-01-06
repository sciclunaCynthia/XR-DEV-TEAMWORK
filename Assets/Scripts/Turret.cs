using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Activation")]
    public bool isActive = true;

    [Header("Targeting")]
    public float range = 6f;
    public float turnSpeed = 6f;
    public LayerMask enemyLayers;

    [Header("Shooting")]
    public float fireRate = 3f;   // shots per second
    public float damage = 5f;
    public Transform shootPoint;

    [Header("Projectile")]
    public Bullet bulletPrefab;   // Drag your Bullet prefab here
    public float bulletSpeed = 18f;

    private float nextFireTime;

    private void OnEnable()
    {
        WaveManager.OnWavesStarted += Activate;
    }

    private void OnDisable()
    {
        WaveManager.OnWavesStarted -= Activate;
    }

    private void Activate()
    {
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        Transform target = FindClosestEnemy();
        if (target == null) return;

        // Rotate towards target
        Vector3 dir = target.position - transform.position;
        dir.y = 0f; // keep level (optional)

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnSpeed * Time.deltaTime);
        }

        // Shoot
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            Shoot(target);
        }
    }

    private Transform FindClosestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayers);

        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var h in hits)
        {
            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = h.transform;
            }
        }

        return best;
    }

    private void Shoot(Transform target)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Turret: bulletPrefab not assigned.");
            return;
        }

        Vector3 origin = shootPoint != null ? shootPoint.position : transform.position + Vector3.up * 0.5f;
        Vector3 aimPoint = target.position + Vector3.up * 0.3f;

        Quaternion rot = Quaternion.LookRotation((aimPoint - origin).normalized);

        Bullet b = Instantiate(bulletPrefab, origin, rot);
        b.Init(damage, enemyLayers, bulletSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
