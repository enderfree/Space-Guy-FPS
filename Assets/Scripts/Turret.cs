using System;
using UnityEngine;

public class Turret : MonoBehaviour, IDamageable 
{
    [Header("Indicator Materials")]
    [SerializeField] Material okMaterial;
    [SerializeField] Material dangerMaterial;

    [Header("Gun Parts")]
    [SerializeField] GameObject indicator;
    [SerializeField] GameObject bulletExhaust;
    [SerializeField] GameObject bullet;

    [Header("Gun Stats")]
    [SerializeField] float timeBetweenShots;
    [SerializeField] float range;

    private Renderer indicatorRenderer;

    private float timeBetweenShotsCounter = 0;
    private bool shooting = false;
    [SerializeField] private float MaxHealth;
    private float health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        indicatorRenderer = indicator.GetComponent<Renderer>();
        //StartShooting(); // for test only
        health = MaxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeBetweenShotsCounter > 0)
        {
            timeBetweenShotsCounter -= Time.fixedDeltaTime;
        }

        if (shooting && timeBetweenShotsCounter <= 0)
        {
            GameObject instance = Instantiate(bullet, bulletExhaust.transform.position, bullet.transform.rotation);
            instance.GetComponent<Bullet>().FireBullet();
            timeBetweenShotsCounter = timeBetweenShots;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void StartShooting()
    {
        indicatorRenderer.material = dangerMaterial;
        shooting = true;
    }

    public void StopShooting()
    {
        shooting = false;
        indicatorRenderer.material = okMaterial;
    }
}
