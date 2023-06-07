using System.Collections.Generic;
using System.IO;
using Underall.MetaInfo;

namespace Underall.Helpers;

public static class PathHelper
{
    private static readonly string _saveFolderName = "saves";
    // private static readonly string _levelInfoListName = "saves";
    private static readonly string _levelInfoFileName = "_level.json";
    private static readonly string _saveInfoFileName = "_save.json";
    private static readonly string _configFileName = "game_config.json";
    public static char PathSeparator => Path.DirectorySeparatorChar;
    
    private static readonly string ParentDirectory = new DirectoryInfo("./../../..").FullName;
    public static readonly string SavesDirectory = Path.Combine(ParentDirectory, _saveFolderName);
    public static readonly string ConfigFullPath = Path.Combine(ParentDirectory, _configFileName);
    
    public static string GetFullPathTo(string fileName) => Path.Combine(ParentDirectory, fileName);
    
    private static string GetFullPathToSaveFolder(string folderName) => Path.Combine(SavesDirectory, folderName);

    public static string GetFullPathToScreenInfo(string saveFolderName, LevelInfo levelInfo) =>
        Path.Combine(GetFullPathToSaveFolder(saveFolderName), levelInfo.CurrentLevelId.ToString(), levelInfo.CurrentScreenId.ToString());
    public static string GetFullPathToLevelInfo(string saveFolderName) =>
        Path.Combine(GetFullPathToSaveFolder(saveFolderName), _levelInfoFileName);

    private static void CreateFolderIfNotExists(string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static void CreateSaveFolderIfNotExists(string saveFolderName) => 
        CreateFolderIfNotExists(GetFullPathToSaveFolder(saveFolderName));

    public static void WriteToScreenInfo(string saveFolderName, LevelInfo levelInfo, string content) =>
        File.WriteAllText(GetFullPathToScreenInfo(saveFolderName, levelInfo), content);

    public static void WriteToLevelInfo(string saveFolderName, string content) =>
        File.WriteAllText(GetFullPathToLevelInfo(saveFolderName), content);
}