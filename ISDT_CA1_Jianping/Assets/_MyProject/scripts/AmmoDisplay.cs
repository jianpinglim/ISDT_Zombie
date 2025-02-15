using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private Gun gun;

    private void Start()
    {
        if (gun == null)
        {
            Debug.LogError("Gun reference not set in AmmoDisplay!");
            return;
        }
        
        // Don't initialize with 0, wait for actual magazine data
        UpdateAmmoDisplay(0, 0);
    }

    public void UpdateAmmoDisplay(int currentAmmo, int maxCapacity)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}";
        }
    }

    public void ClearDisplay()
    {
        if (ammoText != null)
        {
            ammoText.text = "0";
        }
    }
}