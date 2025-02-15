using UnityEngine;


[CreateAssetMenu(fileName = "NewMagazine", menuName = "VR/Magazine Data")]
public class MagazineData : ScriptableObject
{
    public string magazineName;
    public int maxCapacity = 15;
    [SerializeField] private int _currentAmmo;

    public int currentAmmo
    {
        get => _currentAmmo;
        set => _currentAmmo = Mathf.Clamp(value, 0, maxCapacity);
    }

    private void OnEnable()
    {
        _currentAmmo = maxCapacity;
    }
}