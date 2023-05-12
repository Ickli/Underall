using System.IO;

namespace Underall.Helpers;

public static class PathHelper
{
    private static readonly string _saveFolderName = "saves";
    public static char PathSeparator => Path.DirectorySeparatorChar;
    
    public static readonly string ParentDirectory = new DirectoryInfo(".").FullName;
    public static readonly string SavesDirectory = Path.Combine(ParentDirectory, _saveFolderName);
    public static string GetFullPathTo(string fileName)
    {
        return Path.Combine(ParentDirectory, fileName);
    }
}