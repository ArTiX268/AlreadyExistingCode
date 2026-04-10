using Godot;
using System.Collections.Generic;

namespace ArTiX.DataManagment
{
    // Use only properties and not fields because JsonSerializer does not serialize fields by default.
    public class SavedData
    {

    }

    public static class SavingSystem
    {
        private const string SAVE_FILE_PATH = "user://Json/";
        private const string SAVE_FILE_NAME = "Saves.json";

        public static void SaveData(string username, SavedData dataToSave)
        {
            if (username == null || dataToSave == null) return;

            if (!DirAccess.DirExistsAbsolute(SAVE_FILE_PATH))
                DirAccess.MakeDirAbsolute(SAVE_FILE_PATH);

            if (TryLoadAllUsersData(out Dictionary<string, SavedData> datas))
            {
                if (datas.TryGetValue(username, out SavedData lData))
                    datas[username] = dataToSave;
                else
                    datas.Add(username, dataToSave);
            }
            else
            {
                datas = new Dictionary<string, SavedData>
                {
                    { username, dataToSave }
                };
            }

            JsonReader.WriteData(SAVE_FILE_PATH, SAVE_FILE_NAME, datas);
        }

        public static SavedData LoadData(string username)
        {
            if (username == null) return null;

            if (TryLoadAllUsersData(out Dictionary<string, SavedData> datas))
            {
                if (datas.TryGetValue(username, out SavedData playerData))
                    return playerData;
                else
                    GD.Print("Given Username wasn't found.");
            }

            GD.Print("There is no data in the file.");
            return null;
        }

        public static bool TryLoadAllUsersData(out Dictionary<string, SavedData> datas)
        {
            datas = JsonReader.LoadData<Dictionary<string, SavedData>>(SAVE_FILE_PATH + SAVE_FILE_NAME);
            return datas != null;
        }
    }
}
