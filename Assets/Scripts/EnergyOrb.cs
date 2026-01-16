using System.Resources;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EnergyOrb : MonoBehaviour
{
    private static int energyValue = 10;
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

        Debug.Log("Energy has been picked up! Total energy is now: " + ResourceManager.Instance.energy);


        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        

        GameObject particles = Instantiate(
            pickupParticles,
            transform.position + Vector3.up * 0.3f,
            Quaternion.identity
        );

        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
        ps.Play();

        Destroy(particles, 2f); 
        ResourceManager.Instance.AddEnergy(energyValue);
        Destroy(gameObject, 0.05f);

    }
}
