using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlantPlacement : MonoBehaviour
{
    private GridTile currentTile;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    public Material validMat;
    public Material invalidMat;

    private Renderer rend;

    void Start()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rend = GetComponent<Renderer>();
        grab.selectExited.AddListener(OnRelease);
    }

    void OnTriggerEnter(Collider other)
    {
        GridTile tile = other.GetComponent<GridTile>();

        if (tile != null && !tile.isOccupied)
        {
            currentTile = tile;
            rend.material = validMat;
        }
        else
        {
            rend.material = invalidMat;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GridTile>() == currentTile)
        {
            currentTile = null;
            rend.material = invalidMat;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        if (currentTile != null && !currentTile.isOccupied)
        {
            transform.position = currentTile.transform.position;
            currentTile.isOccupied = true;

            GetComponent<Rigidbody>().isKinematic = true;
            grab.enabled = false; // lock placement
        }
    }
}
