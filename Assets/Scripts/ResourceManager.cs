using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int energy = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddEnergy(int amount)
    {
        energy += amount;
        UIManager.Instance.UpdateEnergyText(energy);
    }
}
