using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ShopManager : MonoBehaviour
{
    public enum ItemType { None, Bomb }

    [Header("Currency")]
    public int energy = 50;

    [Header("Costs")]
    public int bombCost = 10;

    [Header("UI References")]
    public TMP_Text energyText;
    public TMP_Text equippedText;

    [Header("Item Prefabs")]
    public GameObject bombPrefab;

    [Header("Spawn / Equip")]
    public Transform handSocket;

    [Tooltip("The hand interactor that should auto-grab the spawned bomb (usually RightHand XR Direct Interactor).")]
    public XRBaseInteractor handInteractor;

    [Tooltip("Your scene's XR Interaction Manager.")]
    public XRInteractionManager interactionManager;

    public ItemType Equipped { get; private set; } = ItemType.None;

    private GameObject currentHeldItem;

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (energyText) energyText.text = $"Energy: {energy}";
        if (equippedText) equippedText.text = $"Equipped: {Equipped}";
    }

    private bool TrySpend(int cost)
    {
        Debug.Log($"[ShopManager] TrySpend called. energy={energy}, cost={cost}", this);

        if (energy < cost)
        {
            Debug.Log("Not enough Energy!", this);
            return false;
        }

        energy -= cost;
        return true;
    }

    private void EquipItem(GameObject prefab, ItemType type)
    {
        if (prefab == null)
        {
            Debug.LogError($"EquipItem failed: prefab for {type} is not assigned.", this);
            return;
        }

        if (handSocket == null)
        {
            Debug.LogError("EquipItem failed: handSocket is not assigned.", this);
            return;
        }

        if (handInteractor == null)
        {
            Debug.LogError("EquipItem failed: handInteractor is not assigned (XR Direct Interactor).", this);
            return;
        }

        if (interactionManager == null)
        {
            Debug.LogError("EquipItem failed: interactionManager is not assigned.", this);
            return;
        }

        // If we already have something "equipped", delete it (simple one-item-at-a-time behavior)
        if (currentHeldItem != null)
        {
            Destroy(currentHeldItem);
            currentHeldItem = null;
        }

        // Spawn the item at the hand socket position/rotation.
        // IMPORTANT: do NOT parent it to the hand. XR Grab will handle attachment.
        GameObject spawned = Instantiate(prefab, handSocket.position, handSocket.rotation);

        // Ensure it's grabbable
        XRGrabInteractable grab = spawned.GetComponent<XRGrabInteractable>();
        if (grab == null)
        {
            Debug.LogError("Spawned item has no XRGrabInteractable. Add it to the prefab.", spawned);
            Destroy(spawned);
            return;
        }

        // Ensure it has physics
        Rigidbody rb = spawned.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Spawned item has no Rigidbody. Add it to the prefab.", spawned);
            Destroy(spawned);
            return;
        }

        // Make sure physics is active (so when released it can drop/throw)
        rb.isKinematic = false;
        rb.useGravity = true;

        // --- FIX: Use new interface-based SelectEnter overload ---
        IXRSelectInteractor selectInteractor = handInteractor as IXRSelectInteractor;
        IXRSelectInteractable selectInteractable = grab as IXRSelectInteractable;

        if (selectInteractor == null)
        {
            Debug.LogError("handInteractor does not implement IXRSelectInteractor. Assign an XR Direct Interactor component.", this);
            Destroy(spawned);
            return;
        }

        if (selectInteractable == null)
        {
            Debug.LogError("XRGrabInteractable does not implement IXRSelectInteractable (unexpected).", spawned);
            Destroy(spawned);
            return;
        }

        interactionManager.SelectEnter(selectInteractor, selectInteractable);
        // --- end fix ---

        currentHeldItem = spawned;
        Equipped = type;

        Debug.Log($"Equipped: {type}", this);
        RefreshUI();
    }

    // --- Button hook ---
    public void BuyBomb()
    {
        if (!TrySpend(bombCost)) return;

        Debug.Log("Bought: Bomb", this);
        EquipItem(bombPrefab, ItemType.Bomb);
    }
}
