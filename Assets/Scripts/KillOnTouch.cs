using UnityEngine;

public class KillOnTouch : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player playerScript))
        {
            playerScript.Kill();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
