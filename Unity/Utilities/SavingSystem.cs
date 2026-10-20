using System.IO;
using UnityEngine;

namespace ArTiX.Utils.FileManagement
{
    public abstract class SavedData
    {

    }

    public static class SavingSystem<T> where T : SavedData
    {
        private const string SAVING_FILE = "PlayerData.json";
        private static readonly string SAVING_FOLDER = Application.persistentDataPath + "/Json/";
        private static readonly string COMPLETE_PATH = SAVING_FOLDER + SAVING_FILE;

        static SavingSystem()
        {
            if (!Directory.Exists(SAVING_FOLDER))
                Directory.CreateDirectory(SAVING_FOLDER);
        }

        public static void Save(in T dataToSave)
        {
            if (dataToSave == null) return;

            if (!File.Exists(COMPLETE_PATH))
            {
                FileStream file = File.Create(COMPLETE_PATH);
                file.Close();
            }

            StreamWriter writer = new StreamWriter(COMPLETE_PATH, false);

            string jsonTxt = JsonUtility.ToJson(dataToSave, true);
            writer.Write(jsonTxt);
            writer.Close();
        }

        public static T LoadData()
        {
            if (!File.Exists(COMPLETE_PATH)) return null;

            StreamReader reader = File.OpenText(COMPLETE_PATH);
            T loadedDatas = JsonUtility.FromJson<T>(reader.ReadToEnd());
            reader.Close();
            return loadedDatas;
        }
    }
}