using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WaveStartButton : MonoBehaviour
{
    public WaveManager waveManager;

    private XRGrabInteractable grab;
    private bool hasStarted = false;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (hasStarted) return;

        hasStarted = true;
        Debug.Log("Button grabbed → starting waves");

        waveManager.StartWaves();
    }
}
