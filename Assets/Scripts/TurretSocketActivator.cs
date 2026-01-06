/*
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class TurretSocketActivator : MonoBehaviour
{
    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnSnappedIn);
        socket.selectExited.AddListener(OnRemoved);
    }

    private void OnSnappedIn(SelectEnterEventArgs args)
    {
        // Ignore if scene is just starting
        if (!Application.isPlaying) return;

        Turret turret = args.interactableObject.transform.GetComponentInParent<Turret>();
        if (turret == null) return;

        turret.isPlaced = true;

        Rigidbody rb = turret.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // lock ONLY after snapping
        }
    }

    private void OnRemoved(SelectExitEventArgs args)
    {
        Turret turret = args.interactableObject.transform.GetComponentInParent<Turret>();
        if (turret == null) return;

        turret.isPlaced = false;

        Rigidbody rb = turret.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false; // unlock when removed
    }
}
*/
