using System;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Underall.Components;

public delegate void StatChangedNotification();

public class StatInfo
{
    [JsonInclude]
    public Vector2 MinMaxLimits { get; private set; }
    [JsonInclude]
    public int Current { get; private set; }
    [JsonInclude]
    public float CurrentProportion { get; private set; }

    public event StatChangedNotification StatChanged;

    public void Add(int amount) => ChangeCurrent(Current + amount);

    public void ChangeCurrent(int current)
    {
        RawChangeCurrent(current);
        CalculateCurrentProportionByInverseLerp();
        StatChanged?.Invoke();
    }

    public void ChangeCurrentProportion(float newProportion)
    {
        CurrentProportion = newProportion;
        CalculateCurrentByLerp();
    }

    public void ChangeLimits(int min, int max, bool saveCurrentProportion) =>
        ChangeLimits(new Vector2(min, max), saveCurrentProportion);
    

    public void ChangeLimits(Vector2 newMinMaxLimits, bool saveCurrentProportion)
    {
        MinMaxLimits = newMinMaxLimits;
        if(saveCurrentProportion) CalculateCurrentByLerp();
        else CalculateCurrentProportionByInverseLerp();
        StatChanged?.Invoke();
    }

    private void CalculateCurrentProportionByInverseLerp() =>
        CurrentProportion = (Current - MinMaxLimits.X) / (MinMaxLimits.Y - MinMaxLimits.X);

    private void CalculateCurrentByLerp() => 
        Current = (int)(MinMaxLimits.X + CurrentProportion * (MinMaxLimits.Y - MinMaxLimits.X));
    

    private void RawChangeCurrent(int current) => 
        Current = (int)Math.Min(MinMaxLimits.Y, Math.Max(MinMaxLimits.X, current));
    
}