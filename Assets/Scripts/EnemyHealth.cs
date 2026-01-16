using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHp = 50f;
    float hp;

    [Header("Audio")]
    public AudioClip bulletHitSound;

    [Header("Death Drop")]
    [Tooltip("Energy orb prefab spawned when the enemy dies")]
    public GameObject energyOrbPrefab;

    [Tooltip("Vertical offset to prevent the orb from clipping into the ground")]
    public float dropHeightOffset = 0.5f;

    void Awake()
    {
        hp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;

        if (bulletHitSound != null)
            AudioSource.PlayClipAtPoint(bulletHitSound, transform.position);

        Debug.Log($"{gameObject.name} took {amount} damage. HP now {hp}");

        if (hp <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn energy orb slightly above ground to prevent clipping
        if (energyOrbPrefab != null)
        {
            Vector3 spawnPosition = transform.position + Vector3.up * dropHeightOffset;

            Instantiate(
                energyOrbPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
