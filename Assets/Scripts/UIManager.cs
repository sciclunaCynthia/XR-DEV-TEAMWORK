using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI waveText;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateEnergyText(int value)
    {
        energyText.text = "Energy: " + value;
    }

    public void UpdateWaveText()
    {
        energyText.text = "Wave Incoming!";
    }
}
