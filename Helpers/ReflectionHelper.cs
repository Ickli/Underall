using System;
using System.Reflection;

namespace Underall.Helpers;

public static class ReflectionHelper
{
    public static TPrivate GetPrivateField<TSource, TPrivate>(TSource source, string fieldName)
    {
        return (TPrivate)typeof(TSource)
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(source);
    }
    
    public static TPrivate GetPrivateProperty<TSource, TPrivate>(TSource source, string fieldName)
    {
        return (TPrivate)typeof(TSource)
            .GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(source);
    }
}