using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    private static readonly string EncryptionKey = "YourSecretKey123";

    public static void SaveGame(Player player)
{
    // Find tablet in the scene
    TabletUIManager tablet = FindAnyObjectByType<TabletUIManager>();
    
    SaveData saveData = new SaveData
    {
        zombiesKilled = ZombieKillsManager.totalKills,
        leversPulled = LeverTracker.leversPulled,
        pulledLeverIds = LeverTracker.GetPulledLeverIds(),
        playerTransform = new SerializableTransform(player.transform),
        tabletTransform = tablet != null ? new SerializableTransform(tablet.transform) : null
    };

    Debug.Log($"Saving player position: {player.transform.position}");
    Debug.Log($"Saving tablet position: {(tablet != null ? tablet.transform.position.ToString() : "No tablet found")}");
    Debug.Log($"Save file location: {SavePath}");

    string json = JsonUtility.ToJson(saveData);
    string encryptedJson = EncryptString(json);
    File.WriteAllText(SavePath, encryptedJson);
    Debug.Log("Game saved successfully!");
}

    public static SaveData LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("No save file found!");
            return null;
        }

        try
        {
            string encryptedJson = File.ReadAllText(SavePath);
            string json = DecryptString(encryptedJson);
            return JsonUtility.FromJson<SaveData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading save file: {e.Message}");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted!");
        }
    }


    private static string EncryptString(string text)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
            aes.IV = new byte[16];

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(text);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    private static string DecryptString(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey.PadRight(32));
            aes.IV = new byte[16];

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}