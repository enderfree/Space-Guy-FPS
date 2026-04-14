using UnityEngine;

public class BulletCanon : MonoBehaviour
{
    private Rigidbody rb;

    // I overcomplicated the logic, lets use this one instead temporarilly
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] float lifespan;

    // Unity
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable iDamageable))
        {
            iDamageable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    public void FireBullet()
    {
       
        Destroy(gameObject, lifespan);
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;
    }
}
