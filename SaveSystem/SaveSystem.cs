using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER_PATH = Application.dataPath + "/Saves/";
    private static readonly string SAVE_FILE_NAME = "save_";
    private static readonly string SAVE_FILE_EXTENSION = ".save";
    private static readonly string META_EXTENSION = ".meta";

    public static void Initialization()
    {
        if (!Directory.Exists(SAVE_FOLDER_PATH))
        {
            Directory.CreateDirectory(SAVE_FOLDER_PATH);
        }
    }

    #region Save

    public static void Save(object pObject)
    {
        int lSaveNumber = 1;
        while (File.Exists(GetFilePath(SAVE_FILE_NAME + lSaveNumber)))
        {
            lSaveNumber++;
        }

        string lJson = JsonUtility.ToJson(pObject, true);
        File.WriteAllText(path: GetFilePath(SAVE_FILE_NAME + lSaveNumber), contents: lJson);
    }

    public static void Save(object pObject, string pFileName)
    {
        string lJson = JsonUtility.ToJson(pObject, true);
        File.WriteAllText(GetFilePath(pFileName), lJson);
    }

    public static void Save(object pObject, out string pFilePath)
    {
        int lSaveNumber = 1;
        while (File.Exists(GetFilePath(SAVE_FILE_NAME + lSaveNumber)))
        {
            lSaveNumber++;
        }

        string lJson = JsonUtility.ToJson(pObject, true);
        File.WriteAllText(path: SAVE_FOLDER_PATH + SAVE_FILE_NAME + lSaveNumber + SAVE_FILE_EXTENSION, contents: lJson);
        pFilePath = SAVE_FOLDER_PATH + SAVE_FILE_NAME + lSaveNumber + SAVE_FILE_EXTENSION;
    }

    #endregion

    #region Load

    public static T LoadMostRecentFile<T>()
    {
        DirectoryInfo lDirectoryInfo = new DirectoryInfo(SAVE_FOLDER_PATH);
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
            return LoadFile<T>(lMostRecentFile.FullName);
        }
        else
            return default;
    }

    public static T LoadFile<T>(string pFileName)
    {
        if (File.Exists(GetFilePath(pFileName)))
        {
            string lSaveString = File.ReadAllText(GetFilePath(pFileName));
            return JsonUtility.FromJson<T>(lSaveString);
        }
        return default;
    }

    public static T LoadFileAtPath<T>(string pPath)
    {
        if (File.Exists(pPath))
        {
            string lSaveString = File.ReadAllText(pPath);
            return JsonUtility.FromJson<T>(lSaveString);
        }
        else
        {
            Debug.Log("File path invalid.");
            return default;
        }
    }

    #endregion

    public static void DeleteSaveFileByName(string pFileName)
    {
        File.Delete(GetFilePath(pFileName));
        File.Delete(GetFilePath(pFileName) + META_EXTENSION);
    }

    public static void DeleteSaveFileByPath(string pFilePath)
    {
        File.Delete(pFilePath);
        pFilePath += META_EXTENSION;
        File.Delete(pFilePath);
    }

    private static string GetFilePath(string pFileName) => SAVE_FOLDER_PATH + pFileName + SAVE_FILE_EXTENSION;
}