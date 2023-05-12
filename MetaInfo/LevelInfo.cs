using System.IO;
using Underall.Helpers;

namespace Underall.MetaInfo;

public class LevelInfo
{
    public static string DefaultLevel = "default_level";
    
    private string LevelFilePath { get; }
    private string LevelName { get; }
    
    public int GridWidth { get; }
    public int GridHeight { get; }
    public int GridCellWidth { get; }
    public int GridCellHeight { get; }
    public int WidthInPixels { get; }
    public int HeightInPixels { get; }

    public static LevelInfo GetInfoFrom(string fileName) => 
        JsonHelper.ParseLevelInfo(PathHelper.GetFullPathTo(fileName));
    
}