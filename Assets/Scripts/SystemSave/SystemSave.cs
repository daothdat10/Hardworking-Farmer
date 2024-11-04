using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class SystemSave
{ 
    
    public static void Save(GameData data)
    {
        PlayerProfile playerData = new PlayerProfile
        {
            coins = data.Coins,
            
        };
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(GetPath(), FileMode.Create);

        // Serialize PlayerProfile instead of GameData
        formatter.Serialize(fs, playerData);
        fs.Close();
    }

    public static void Load(GameData gameData)
    {
        if (!File.Exists(GetPath()))
        {
            Debug.LogWarning("Save file not found. Creating new data.");
            return;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fs = new FileStream(GetPath(), FileMode.Open);

        // Deserialize to PlayerProfile
        PlayerProfile playerData = formatter.Deserialize(fs) as PlayerProfile;
        fs.Close();

        // Set GameData properties based on PlayerProfile data
        gameData.Coins = playerData.coins;
       
    }

    private static string GetPath()
    {
        string folderPath = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        return folderPath + "/data.qnd";
    }

}