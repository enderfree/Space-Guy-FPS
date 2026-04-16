using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour

{
    [SerializeField] private string endingSceneName = "EndingScene";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(endingSceneName);
        }
    }
}