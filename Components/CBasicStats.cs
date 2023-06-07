using System.Collections.Generic;

namespace Underall.Components;

public class CBasicStats : Dictionary<string, StatInfo>
{
    public static readonly string[] StandardStatsNames =
    {
        "Health",
        "Sanity",
        "Stamina"
    };
}

// public class CBasicStats
// {
//     public StatInfo Health;
//     public StatInfo Sanity;
//     public StatInfo Stamina;
// }

