using System.Resources;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EnergyOrb : MonoBehaviour
{
    public int energyValue = 1;
    private XRGrabInteractable grab;
    public AudioClip pickupSound;
    public GameObject pickupParticles;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        Instantiate(pickupParticles, transform.position, Quaternion.identity);
        ResourceManager.Instance.AddEnergy(energyValue);
        Destroy(gameObject, 0.05f);

    }
}
