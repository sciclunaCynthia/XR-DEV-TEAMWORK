using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{

    [Header("Impact Layers")]
   
    public LayerMask groundLayers;
    [Header("Explosion")]
    public float radius = 2.5f;
    public float damage = 10f;
    public LayerMask enemyLayers;

    [Header("Impact")]
    public bool explodeOnAnyCollision = false;
    public float minImpactSpeed = 0.5f;        

    [Header("VFX")]
    public ParticleSystem explosionVfxPrefab;  
    public float vfxLifetime = 2f;             

    private bool exploded;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    XRGrabInteractable grab;
    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(_ => rb.isKinematic = true);
        grab.selectExited.AddListener(_ => rb.isKinematic = false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;

        
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < minImpactSpeed) return;

        int hitLayerMask = 1 << collision.gameObject.layer;
        if (!explodeOnAnyCollision)
        {

            bool hitEnemy = (enemyLayers.value & hitLayerMask) != 0;
            bool hitGround = (groundLayers.value & hitLayerMask) != 0;

            if (!hitEnemy && !hitGround)
                return;
        }

        Debug.Log($"Bomb hit: {collision.gameObject.name} (speed {impactSpeed:F2}) -> EXPLODE");
        Explode();
    }

    private void Explode()
    {
        exploded = true;

        
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
