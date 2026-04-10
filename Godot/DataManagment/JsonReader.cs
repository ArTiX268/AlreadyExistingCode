using Godot;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ArTiX.DataManagment
{
    public static class JsonReader
    {
        public static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static void WriteData<T>(string dirPath, string fileName, T data) where T : class
        {
            if (!DirAccess.DirExistsAbsolute(dirPath))
                DirAccess.MakeDirAbsolute(dirPath);

            FileAccess file = FileAccess.Open(dirPath + fileName, FileAccess.ModeFlags.Write);
            string lJsonText = JsonSerializer.Serialize(data, jsonOptions);
            file.StoreString(lJsonText);
            file.Close();
        }

        public static T LoadData<T>(string path) where T : class
        {
            if (!FileAccess.FileExists(path)) return null;

            FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            T data = JsonSerializer.Deserialize<T>(file.GetAsText(), jsonOptions);
            file.Close();

            return data;
        }
    }
}
