using NetGame.Client;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            ClientSend.SendKey(KeyCode.F);
        }
    }
}
