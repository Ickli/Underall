using System.IO;
using Microsoft.Xna.Framework;
using Underall.Components;
using Underall.Helpers;

namespace Underall.MetaInfo;

public class LevelInfo
{
    public int CurrentLevelId;
    public Dimensions CurrentDimension;
    public string[][] BackgroundFileNames; // at index X lies info about backgrounds for level with X id
    public Vector2[] BackgroundScales;
    public int[] GridWidths;
    public int[] GridHeights;
    public int[] GridCellWidths;
    public int[] GridCellHeights;
    public int[] WidthsInPixels;
    public int[] HeightsInPixels;

    public void SwitchToNextDimension()
    {
        CurrentDimension = (Dimensions)((int)(CurrentDimension + 1) % ComponentRegistry.DimensionsAmount);
    }
}