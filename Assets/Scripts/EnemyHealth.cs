using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public AudioClip bulletHitSound;
    public float maxHp = 50f;
    private float hp;

    private void Awake() => hp = maxHp;

    public void TakeDamage(float amount)
    {
        hp -= amount;
        AudioSource.PlayClipAtPoint(bulletHitSound, transform.position);
        Debug.Log($"{gameObject.name} took {amount} damage. HP now {hp}");
        if (hp <= 0f) Destroy(gameObject);
    }
}
