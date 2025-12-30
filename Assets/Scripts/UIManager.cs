using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI energyText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateEnergyText(int value)
    {
        energyText.text = "Energy: " + value;
    }
}
