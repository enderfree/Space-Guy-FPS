using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public void StartGame()
    {
        Player.hasGameStarted = false;
    }

    public void EndGame()
    {
        Player.hasGameStarted = true;
    }
}