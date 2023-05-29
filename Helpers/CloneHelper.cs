using System.Text.Json;
namespace Underall.Helpers;

public static class CloneHelper
{
    public static T Clone<T>(T source)
    {
        var serialized = JsonHelper.Serialize(source);
        return JsonSerializer.Deserialize<T>(serialized, JsonHelper.DefaultSerializerOptions);
    }
}