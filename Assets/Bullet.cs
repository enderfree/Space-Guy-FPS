using UnityEngine;

public class Bullet : MonoBehaviour
{
    //[SerializeField] float damage;
    //[SerializeField] float startSpeed; // NB. speed is divided by 5 like acceleration was
    //[SerializeField] float maxSpeed;
    //[SerializeField] float speedMltp;

    //private float speed;
    //private bool reachedMaxSpeed = false;

    private Rigidbody rb;
    //private Vector3 destination;

    // I overcomplicated the logic, lets use this one instead temporarilly
    [SerializeField] float damage;
    [SerializeField] float speed;
    [SerializeField] float lifespan;

    public bool isOgBullet = true;

    // Unity
    void Awake()
    {
        //speed = startSpeed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (destination != Vector3.zero) // don't fire the bullet we clone
        //{
        //    // if you reach the range, destroy the bullet
        //    if (transform.position == destination)
        //    {
        //        Destroy(gameObject);
        //        return;
        //    }

        //    // move bullet towards target
        //    rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, rb.transform.forward * maxSpeed, speed);
        //    //rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, destination, speed);

        //    // increase bullet speed until it reaches maxSpeed
        //    if (!reachedMaxSpeed)
        //    {
        //        speed *= speedMltp;

        //        if (speed > maxSpeed)
        //        {
        //            speed = maxSpeed;
        //            reachedMaxSpeed = true;
        //        }
        //    }
        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (destination != Vector3.zero) // don't destroy the bullet we clone
        //{
            if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable iDamageable))
            {
                iDamageable.TakeDamage(damage);
            }

            Destroy(gameObject);
        //}
    }

    public void FireBullet()
    {
        rb.AddForce(rb.transform.right * speed); //...my turret is not facing forward...
        Destroy(gameObject, lifespan);
    }

    //public Vector3 SetDestination { set { destination = value; } }
}
