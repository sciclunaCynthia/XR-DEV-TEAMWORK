using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 18f;
    public float damage = 2f;
    public float lifeTime = 3f;
    public LayerMask enemyLayers;
    //public AudioClip bulletHitSound;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Destroy(gameObject, lifeTime);
    }

    // Called right after Instantiate
    public void Init(float dmg, LayerMask layers, float bulletSpeed)
    {
        damage = dmg;
        enemyLayers = layers;
        speed = bulletSpeed;
    }

    private void FixedUpdate()
    {
        // move forward constantly
        rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only damage enemies
        if (((1 << other.gameObject.layer) & enemyLayers.value) == 0)
            return;

        var hp = other.GetComponentInParent<EnemyHealth>();
        if (hp != null)
            hp.TakeDamage(damage);
        //AudioSource.PlayClipAtPoint(bulletHitSound, transform.position);
        Destroy(gameObject);

    }
}
