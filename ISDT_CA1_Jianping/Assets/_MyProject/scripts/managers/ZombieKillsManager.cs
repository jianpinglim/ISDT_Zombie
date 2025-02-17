using UnityEngine;

public class ZombieKillsManager : MonoBehaviour
{
    public static int totalKills = 0;
    public static event System.Action onZombieKilled;

    public static void ZombieKilled()
    {
        totalKills++;
        onZombieKilled?.Invoke(); 
        Debug.Log($"Zombie killed! Total kills: {totalKills}");
    }
}