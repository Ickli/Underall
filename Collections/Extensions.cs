using System;
using System.Collections.Generic;

namespace Underall.Collections;

public static class Extensions
{
    public static void AddSorted<T>(this List<T> sortedList, T element)
        where T : IComparable
    {
        var index = 0;
        for (; index < sortedList.Count; index++)
            if (sortedList[index].CompareTo(element) >= 0)
                break;
        sortedList.Insert(index, element);
    }
}