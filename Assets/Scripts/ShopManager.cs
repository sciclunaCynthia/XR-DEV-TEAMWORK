using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ShopManager : MonoBehaviour
{
    public enum ItemType { None, Bomb, Cactus }

    //[Header("Currency")]
    //public int energy = 50;

    [Header("Costs")]
    public int bombCost = 10;
    public int cactusCost = 15;

    [Header("UI References")]
    public TMP_Text energyText;
    public TMP_Text equippedText;

    [Header("Item Prefabs")]
    public GameObject bombPrefab;
    public GameObject cactusPrefab;

    [Header("Spawn Settings")]
    [Tooltip("How far in front of the player's view the item spawns (meters). 0.6–1.0 is typical.")]
    public float spawnDistance = 0.75f;

    [Tooltip("Optional: raise/lower spawn slightly relative to camera (meters).")]
    public float spawnHeightOffset = -0.10f;

    public ItemType Equipped { get; private set; } = ItemType.None;

    private GameObject currentSpawnedItem;

    private void Start()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (energyText)
            energyText.text = $"Energy: {ResourceManager.Instance.energy}";

        if (equippedText)
            equippedText.text = $"Equipped: {Equipped}";


    }

    private void OnEnable()
    {
        RefreshUI();
    }


    private bool TrySpend(int cost)
    {
        if (!ResourceManager.Instance.SpendEnergy(cost))
        {
            Debug.Log("Not enough Energy!", this);
            return false;
        }

        return true;
    }


    private void SpawnItemInFrontOfPlayer(GameObject prefab, ItemType type)
    {
        if (prefab == null)
        {
            Debug.LogError($"Spawn failed: prefab for {type} is not assigned.", this);
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Spawn failed: Camera.main is null. Make sure your XR camera is tagged MainCamera.", this);
            return;
        }

        // One-at-a-time behavior (optional, but keeps it simple)
        if (currentSpawnedItem != null)
        {
            Destroy(currentSpawnedItem);
            currentSpawnedItem = null;
        }

        // Use flattened forward so it spawns in front on the horizontal plane (prevents spawning upward/downward)
        Vector3 forwardFlat = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        if (forwardFlat.sqrMagnitude < 0.0001f) forwardFlat = cam.transform.forward;

        Vector3 spawnPos = cam.transform.position + forwardFlat * spawnDistance + Vector3.up * spawnHeightOffset;
        Quaternion spawnRot = Quaternion.LookRotation(forwardFlat, Vector3.up);

        GameObject spawned = Instantiate(prefab, spawnPos, spawnRot);

        // Optional checks so it's grabbable/throwable
        if (spawned.GetComponent<XRGrabInteractable>() == null)
            Debug.LogWarning($"Spawned {type} has no XRGrabInteractable (won't be grabbable).", spawned);

        if (spawned.GetComponent<Rigidbody>() == null)
            Debug.LogWarning($"Spawned {type} has no Rigidbody (won't be throwable).", spawned);

        currentSpawnedItem = spawned;
        Equipped = type;
        RefreshUI();
    }

    // --- Button hooks ---
    public void BuyBomb()
    {
        if (!TrySpend(bombCost)) return;
        SpawnItemInFrontOfPlayer(bombPrefab, ItemType.Bomb);
    }

    public void BuyCactus()
    {
        if (!TrySpend(cactusCost)) return;
        SpawnItemInFrontOfPlayer(cactusPrefab, ItemType.Cactus);
    }
}
