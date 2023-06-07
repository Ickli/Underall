using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Underall.Components;
using Underall.Gameplay;
using Underall.Gameplay.Effects;
using Underall.Helpers;

namespace Underall.MetaInfo;

public class LevelInfo
{
    // public const int IntermediateScreenId = 0;
    
    [JsonInclude] public int CurrentLevelId { get; private set; }
    [JsonInclude] public int CurrentScreenId { get; private set; }
    [JsonInclude] public Dimensions CurrentDimension { get; private set; }
    [JsonInclude] public string[][] BackgroundFileNames { get; private set; } // at index X lies info about backgrounds for level with X id
    [JsonInclude] public Vector2[] BackgroundScales { get; private set; }
    [JsonInclude] public int[] GridWidths { get; private set; }
    [JsonInclude] public int[] GridHeights { get; private set; }
    [JsonInclude] public int[] GridCellWidths { get; private set; }
    [JsonInclude] public int[] GridCellHeights { get; private set; }
    [JsonInclude] public int[] WidthsInPixels { get; private set; }
    [JsonInclude] public int[] HeightsInPixels { get; private set; }

    [JsonInclude] public Dictionary<int, int[]> EntitiesLocation { get; private set; }
    [JsonInclude] public Dictionary<string, object> Flags { get; private set; }
    [JsonInclude] public SortedList<TimeSpan, Effect> Effects { get; private set; }
    public void SwitchToNextDimension() => 
        CurrentDimension = (Dimensions)((int)(CurrentDimension + 1) % ComponentRegistry.DimensionsAmount);
}