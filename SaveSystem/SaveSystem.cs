using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    private static readonly string SAVE_FILE_NAME = "save_";
    private static readonly string SAVE_FILE_EXTENSION = ".txt";

    private static void Initialization()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

    public static void Save(object pObject)
    {
        Initialization();

        int lSaveNumber = 1;
        while (File.Exists(SAVE_FOLDER + SAVE_FILE_NAME + lSaveNumber + SAVE_FILE_EXTENSION))
        {
            lSaveNumber++;
        }

        string lJson = JsonUtility.ToJson(pObject);
        File.WriteAllText(path: SAVE_FOLDER + SAVE_FILE_NAME + lSaveNumber + SAVE_FILE_EXTENSION, contents: lJson);
    }

    public static T Load<T>()
    {
        DirectoryInfo lDirectoryInfo = new DirectoryInfo(SAVE_FOLDER);
        // The star is used to get all the files of the specified extension
        FileInfo[] lFilesInfo = lDirectoryInfo.GetFiles("*" + SAVE_FILE_EXTENSION);
        FileInfo lMostRecentFile = null;

        foreach (FileInfo lCurrentFileInfo in lFilesInfo)
        {
            if (lMostRecentFile == null)
            {
                lMostRecentFile = lCurrentFileInfo;
                continue;
            }

            if (lCurrentFileInfo.LastWriteTime > lMostRecentFile.LastWriteTime)
                lMostRecentFile = lCurrentFileInfo;
        }

        if (lMostRecentFile != null)
        {
            return Load<T>(lMostRecentFile.FullName);
        }
        else
            return default;
    }

    public static T Load<T>(string lFilePath)
    {
        string lSaveString = File.ReadAllText(lFilePath);
        return JsonUtility.FromJson<T>(lSaveString);
    }
}