using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion")]
    public float radius = 2.5f;
    public float damage = 10f;
    public LayerMask enemyLayers;

    [Header("Impact")]
    public bool explodeOnAnyCollision = false; // if false: only explodes when it hits an enemy
    public float minImpactSpeed = 0.5f;        // prevents tiny bumps from exploding

    [Header("VFX")]
    public ParticleSystem explosionVfxPrefab;  // drag your particle prefab here
    public float vfxLifetime = 2f;             // how long until the spawned VFX is destroyed

    private bool exploded;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;

        // Optional: ignore super-slow impacts
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < minImpactSpeed) return;

        // If we only want to explode when hitting an enemy:
        if (!explodeOnAnyCollision)
        {
            // Check if the object we hit is on the enemy layer mask
            if (((1 << collision.gameObject.layer) & enemyLayers.value) == 0)
                return;
        }

        Debug.Log($"Bomb hit: {collision.gameObject.name} (speed {impactSpeed:F2}) -> EXPLODE");
        Explode();
    }

    private void Explode()
    {
        exploded = true;

        // Spawn explosion VFX at the bomb position BEFORE destroying the bomb
        if (explosionVfxPrefab != null)
        {
            ParticleSystem vfx = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
            Destroy(vfx.gameObject, vfxLifetime);
        }
        else
        {
            Debug.LogWarning("Bomb: explosionVfxPrefab not assigned.");
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayers);
        Debug.Log($"Bomb overlap hits: {hits.Length}");

        foreach (var h in hits)
        {
            var hp = h.GetComponentInParent<EnemyHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                Debug.Log($"Damaged {hp.gameObject.name} for {damage}");
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
