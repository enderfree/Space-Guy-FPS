using UnityEngine;

public class ArmCannon : MonoBehaviour
{
    [SerializeField] private GameObject bulletExhaust;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float timeBetweenShots;
    [SerializeField] private float range;
     private float timeBetweenShotsCounter = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (timeBetweenShotsCounter > 0)
        {
            timeBetweenShotsCounter -= Time.fixedDeltaTime;
        }

        if (timeBetweenShotsCounter <= 0)
        {
            GameObject instance = Instantiate(bullet, bulletExhaust.transform.position, bullet.transform.rotation);
            instance.GetComponent<Bullet>().FireBullet();
            timeBetweenShotsCounter = timeBetweenShots;
        }
    }
}
