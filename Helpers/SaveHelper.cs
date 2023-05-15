using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MonoGame.Extended.Collections;
using Underall.Components;
using Underall.Helpers;
using Underall.MetaInfo;

namespace Underall.Helpers;

using MonoGame.Extended.Entities;

/// <summary>
/// SaveHelper fetches game data: entities and components.
/// </summary>
public static class SaveHelper
{

    /// <summary>
    /// Returns bool array where true at index X means that entity with X Id is alive.
    /// </summary>
    /// <remarks>
    /// Attention! This works fine until you want to use EntityAdded, EntityRemoved, EntityChanged actions
    /// because it does not preserve fields _removedEntities and _changedEntities.
    /// </remarks>
    /// <returns></returns>
    private static bool[] FetchEntitiesAlive(EntityManager entityManager)
    {
        var entities = ReflectionHelper.GetPrivateField<EntityManager, Bag<Entity>>(entityManager, "_entityBag");
        var removed = ReflectionHelper.GetPrivateField<EntityManager, Bag<int>>(entityManager, "_removedEntities").ToHashSet();

        var fetched = entities.Select(e => !removed.Contains(e.Id)).ToArray();

        return fetched;
    }

    private static JsonArray GetJsonArrayFromEntities(World world)
    {
        var entityManager = ReflectionHelper.GetPrivateProperty<World, EntityManager>(world, "EntityManager");
        var entitiesAlive = FetchEntitiesAlive(entityManager);
        return JsonHelper.GetAllEntitiesComponentsSerialized(entityManager, ComponentRegistry.Mappers, entitiesAlive,
            new JsonSerializerOptions { IncludeFields = true });
    }

    public static void LoadSaveInfoWithDefaultOptions(World world, string saveInfoPath)
    {
        LoadSavedEntities(world, saveInfoPath, JsonHelper.DefaultDocumentOptions, JsonHelper.DefaultSerializerOptions);
    }
    private static void LoadSavedEntities(World world, string saveInfoPath, JsonDocumentOptions documentOptions, 
        JsonSerializerOptions serializerOptions)
    {
        var cTypes = ComponentRegistry.GetTypes();
        var cMappers = ComponentRegistry.Mappers;

        using var jsonEntities = JsonHelper.GetJsonDocument(saveInfoPath, documentOptions).RootElement.EnumerateArray();
        foreach (var jsonEntity in jsonEntities)
            LoadEntityFromJsonElement(world, jsonEntity, cTypes, cMappers.ToArray(), serializerOptions);
    }

    private static void LoadEntityFromJsonElement(World world, JsonElement entity, Type[] cTypes, ComponentMapper[] cMappers, 
        JsonSerializerOptions serializerOptions)
    {
        var ent = world.CreateEntity();

        if (entity.ValueKind == JsonValueKind.Null)
        {
            // if valueKind is null 
            // the entity was removed at the moment of saving
            // so it is removed again by being destroyed
            ent.Destroy();
            return;
        }
        
        for (var componentId = 0; componentId < cTypes.Length; componentId++)
            if (entity[componentId].ValueKind != JsonValueKind.Null)
            {
                // if valueKind is null
                // the component was absent at the moment of saving
                // so it is skipped
                var dynMapper = cMappers[componentId] as dynamic;
                dynMapper.Put(ent.Id, (dynamic)entity[componentId].Deserialize(cTypes[componentId], serializerOptions));
            }
    }

    public static LevelInfo GetLevelInfo(string saveFolderName) =>
        JsonHelper.Deserialize<LevelInfo>(PathHelper.GetFullPathToSavedLevelInfo(saveFolderName));

    private static void MakeSave(World world, LevelInfo currentLevel, string saveFolderName)
    {
        PathHelper.CreateSaveFolderIfNotExists(saveFolderName);
        
        PathHelper.WriteToLevelInfo(saveFolderName, JsonHelper.Serialize(currentLevel));
        PathHelper.WriteToSaveInfo(saveFolderName, GetJsonArrayFromEntities(world).ToJsonString());
    }
}