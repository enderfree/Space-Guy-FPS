using UnityEngine;
using System.Collections.Generic;

public class TurretTrigger : MonoBehaviour
{
    [SerializeField] Turret turret;

    private List<GameObject> detected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        detected = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<ITriggerTurrets>(out ITriggerTurrets iShouldNotNeedThis))
        {
            turret.StartShooting();
            detected.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<ITriggerTurrets>(out ITriggerTurrets iShouldNotNeedThis))
        {
            detected.Remove(other.gameObject);

            if (detected.Count < 1)
            {
                turret.StopShooting();
            }
        }
    }
}
