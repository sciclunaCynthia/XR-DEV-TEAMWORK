using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public enum ItemType { None, Bomb, Sunflower, Mushroom }

    [Header("Currency")]
    public int energy = 50; // Start value for testing

    [Header("Costs")]
    public int bombCost = 10;

    [Header("UI References")]
    public TMP_Text energyText;     // e.g. "Energy: 50"
    public TMP_Text equippedText;   // e.g. "Equipped: Bomb"

    public ItemType Equipped { get; private set; } = ItemType.None;

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
        if (energy < cost)
        {
            Debug.Log("Not enough Energy!");
            return false;
        }

        energy -= cost;
        return true;
    }

    // --- Button hooks ---
    public void BuyBomb()
    {
        if (!TrySpend(bombCost)) return;
        Equipped = ItemType.Bomb;
        Debug.Log("Bought & equipped: Bomb");
        RefreshUI();
    }

}

