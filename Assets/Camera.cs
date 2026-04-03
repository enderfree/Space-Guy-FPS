using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    private InputSystem_Actions inputAction;

    private void Awake()
    {
        inputAction = new InputSystem_Actions();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
