using System;
using UnityEngine;

public class Turret : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        indicatorRenderer = indicator.GetComponent<Renderer>();
        StartShooting(); // for test only
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
