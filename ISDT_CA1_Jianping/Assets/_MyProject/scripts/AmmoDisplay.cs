using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI ammoText;
    [SerializeField] private Gun gun;

    private void Start()
    {
        if (gun == null)
        {
            Debug.LogError("Gun reference not set in AmmoDisplay!");
            return;
        }

        UpdateAmmoDisplay(0); // Initialize with 0
    }

    public void UpdateAmmoDisplay(int currentAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"Ammo: {currentAmmo}";
        }
    }
}
