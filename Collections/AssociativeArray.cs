using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Underall.Collections;

/// <summary>
/// Collection of arrays used instead of Dictionary for memory saving
/// when lookups by key are not needed.
/// </summary>
public class AssociativeArray<T1, T2>
{
    [JsonInclude]
    private Tuple<T1, T2>[] Pairs
    {
        get
        {
            var array = new Tuple<T1, T2>[First.Length];
            for (var index = 0; index < First.Length; index++)
                array[index] = Tuple.Create(First[index], Second[index]);
            return array;
        }
        set
        {
            First = new T1[value.Length];
            Second = new T2[value.Length];
            for (var index = 0; index < value.Length; index++)
            {
                First[index] = value[index].Item1;
                Second[index] = value[index].Item2;
            }
        }
    }

    [JsonIgnore] public T1[] First { get; private set; }
    [JsonIgnore] public T2[] Second { get; private set; }
    
    public AssociativeArray() {}

    public AssociativeArray(T1[] first, T2[] second)
    {
        First = first;
        Second = second;
    }
    
    public AssociativeArray(int length): this(new T1[length], new T2[length]) {}
}

public class AssociativeArray<T1, T2, T3>
{
    [JsonInclude]
    private Tuple<T1, T2, T3>[] Pairs
    {
        get
        {
            var array = new Tuple<T1, T2, T3>[First.Length];
            for (var index = 0; index < First.Length; index++)
                array[index] = Tuple.Create(First[index], Second[index], Third[index]);
            return array;
        }
        set
        {
            First = new T1[value.Length];
            Second = new T2[value.Length];
            Third = new T3[value.Length];
            for (var index = 0; index < value.Length; index++)
            {
                First[index] = value[index].Item1;
                Second[index] = value[index].Item2;
                Third[index] = value[index].Item3;
            }
        }
    }

    [JsonIgnore] public T1[] First { get; private set; }
    [JsonIgnore] public T2[] Second { get; private set; }
    [JsonIgnore] public T3[] Third { get; private set; }
    
    public AssociativeArray() {}

    public AssociativeArray(T1[] first, T2[] second, T3[] third)
    {
        First = first;
        Second = second;
        Third = third;
    }
    
    public AssociativeArray(int length): this(new T1[length], new T2[length], new T3[length]) {}
}