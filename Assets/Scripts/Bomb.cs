using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Bomb : MonoBehaviour
{
    [Header("Impact Layers")]
    public LayerMask groundLayers;

    [Header("Explosion")]
    public float radius = 2.5f;
    public float damage = 10f;
    public LayerMask enemyLayers;

    [Header("Impact")]
    [Tooltip("Minimum impact speed required to explode (meters/sec).")]
    public float minImpactSpeed = 0.5f;

    [Header("Arming")]
    [Tooltip("Bomb will ONLY explode after it has been released by the player at least once.")]
    public bool requireReleaseToArm = true;

    [Tooltip("Optional: require at least this release speed to count as 'thrown'. Set to 0 to arm on any release (drop OR throw).")]
    public float minReleaseSpeedToArm = 0.0f;

    [Header("VFX")]
    public ParticleSystem explosionVfxPrefab;
    public float vfxLifetime = 2f;

    private bool exploded;
    private Rigidbody rb;
    private XRGrabInteractable grab;

    // This is the key state:
    private bool armed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        // When grabbed: disarm (prevents exploding while being held / before any throw)
        grab.selectEntered.AddListener(OnGrabbed);
        // When released: arm (only if released fast enough)
        grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Reset arming when picked up
        armed = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (!requireReleaseToArm)
        {
            armed = true;
            return;
        }

        // Arm only if release speed is high enough
        float releaseSpeed = rb.linearVelocity.magnitude;
        if (releaseSpeed >= minReleaseSpeedToArm)
            armed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;

        // Only explode after release/throw
        if (requireReleaseToArm && !armed) return;

        // Only explode on GROUND contact
        int hitLayerMask = 1 << collision.gameObject.layer;
        bool hitGround = (groundLayers.value & hitLayerMask) != 0;
        if (!hitGround) return;

        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < minImpactSpeed) return;

        Debug.Log($"Bomb hit ground: {collision.gameObject.name} (speed {impactSpeed:F2}) -> EXPLODE");
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
