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
        //Instantiate(pickupParticles, transform.position, Quaternion.identity);

        GameObject particles = Instantiate(
            pickupParticles,
            transform.position + Vector3.up * 0.3f,
            Quaternion.identity
        );

        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        ps.Play();

        Destroy(particles, 2f); //particles finish before orb is destroyed 

        ResourceManager.Instance.AddEnergy(energyValue);
        Destroy(gameObject, 0.05f);

    }
}
