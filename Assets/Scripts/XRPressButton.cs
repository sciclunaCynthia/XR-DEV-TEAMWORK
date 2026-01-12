using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPressButton : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    public WaveManager waveManager;
    public float pressCooldown = 1f;

    private bool pressed;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (pressed) return;
        pressed = true;

        Debug.Log("Button pressed ? starting waves");
        waveManager.StartWaves();

        Invoke(nameof(ResetPress), pressCooldown);
    }

    private void ResetPress()
    {
        pressed = false;
    }
}
