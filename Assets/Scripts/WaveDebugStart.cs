using UnityEngine;
using UnityEngine.InputSystem;

public class WaveDebugStart : MonoBehaviour
{
    public WaveManager waveManager;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            waveManager.StartWaves();
        }
    }
}
