using UnityEngine;

public class DeathBox : MonoBehaviour
{
   


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(9999); 
            }
            else
            {
              
                Destroy(other.gameObject);
            }
        }
    }
}

