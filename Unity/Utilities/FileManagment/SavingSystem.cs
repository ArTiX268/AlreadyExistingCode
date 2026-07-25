using System.IO;
using UnityEngine;

public class SavedData
{
    // The index represent the level.
    public int[] nbStarPerLevel;
    public int[] scorePerLevel;
    public float[] killedPercentagePerLevel;
}

public static class SavingSystem
{
    private const string SAVING_PATH = "/Json/PlayerData.json";
    private static readonly string completePath;

    static SavingSystem()
    {
        completePath = Application.persistentDataPath + SAVING_PATH;
    }

    public static void Save(in SavedData dataToSave)
    {
        if (dataToSave == null) return;

        if (!Directory.Exists(Application.persistentDataPath + "/Json"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Json");

        if (!File.Exists(completePath))
        {
            FileStream file = File.Create(completePath);
            file.Close();
        }

        StreamWriter writer = new StreamWriter(completePath, false);

        string jsonTxt = JsonUtility.ToJson(dataToSave, true);
        writer.Write(jsonTxt);
        writer.Close();
    }

    public static SavedData LoadData()
    {
        if (!File.Exists(completePath)) return null;

        StreamReader reader = File.OpenText(completePath);
        SavedData loadedDatas = JsonUtility.FromJson<SavedData>(reader.ReadToEnd());
        reader.Close();
        return loadedDatas;
    }
}
