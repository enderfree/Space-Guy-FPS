using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player playerScript))
        {
            playerScript.lastCheckpoint = transform;
        }
    }
}
