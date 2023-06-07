namespace Underall.Components;

public class CMeta
{
    // Id acquired by entity that hasn't been initialized yet,
    // e.g. right after being added by LevelEditor.
    // It is not Nullable<int> due to memory saving.
    public const int NotInitializedId = 0;
    // Id unique for every entity kind
    public int CommonId;
    // Id unique for every entity
    public int Id;
}