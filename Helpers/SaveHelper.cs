using System;
using System.Collections.Generic;
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
    public static int MaxSaveCount = 10;
    
    public static IEnumerable<string> GetSaveNames() => Directory
        .GetDirectories(PathHelper.SavesDirectory)
        .Select(dir => Path.GetFileName(dir));
    
    // public static string GetCurrentScreenPath(LevelInfo levelInfo) =>
        

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
        var removed = ReflectionHelper.GetPrivateField<EntityManager, Bag<int>>(entityManager, "_removedEntities");

        var fetched = new bool[entities.Count];
        Array.Fill(fetched, true);
        for (var e = 0; e < removed.Count; e++)
            fetched[removed[e]] = false;

        return fetched;
    }

    private static string GetJsonStringFromEntities(World world)
    {
        var entityManager = ReflectionHelper.GetPrivateProperty<World, EntityManager>(world, "EntityManager");
        var entitiesAlive = FetchEntitiesAlive(entityManager);
        return JsonHelper.GetAllEntitiesComponentsSerialized(entityManager, ComponentRegistry.Mappers, entitiesAlive,
            JsonHelper.DefaultSerializerOptions).ToJsonString();
    }

    public static void LoadSavedScreen(World world, string savedScreenPath) =>
        LoadSavedEntities(world, savedScreenPath, JsonHelper.DefaultDocumentOptions, JsonHelper.DefaultSerializerOptions);
    private static void LoadSavedEntities(World world, string saveInfoPath, JsonDocumentOptions documentOptions, 
        JsonSerializerOptions serializerOptions)
    {
        var cTypes = ComponentRegistry.GetTypes();
        var cMappers = ComponentRegistry.Mappers.ToArray();

        using var jsonDoc = JsonHelper.GetJsonDocument(saveInfoPath, documentOptions);
        var jsonEntities = jsonDoc.RootElement;
        var entitiesCount = jsonEntities.GetArrayLength();
        var nullEntities = new List<int>();
        
        // load every entity regardless of whether it's null or not
        for (var entId = 0; entId < entitiesCount; entId++)
            if(LoadEntityFromJsonElement(world, jsonEntities[entId], cTypes, cMappers, serializerOptions) == -1)
                nullEntities.Add(entId);

        // destroy every null entity
        for(var index = 0; index < nullEntities.Count; index++)
            world.DestroyEntity(nullEntities[index]);
    }

    /// <summary>
    /// Loads components of an entity from JsonElement.
    /// </summary>
    /// <returns>Id of loaded entity, -1 if null</returns>
    private static int LoadEntityFromJsonElement(World world, JsonElement entity, Type[] cTypes, ComponentMapper[] cMappers, 
        JsonSerializerOptions serializerOptions)
    {
        var ent = world.CreateEntity();
        if (entity.ValueKind == JsonValueKind.Null)
            return -1;

        for (var componentId = 0; componentId < cTypes.Length; componentId++)
            if (entity[componentId].ValueKind != JsonValueKind.Null)
                // if valueKind is null
                // the component was absent at the moment of saving
                // so it is skipped
                ((dynamic)cMappers[componentId]).Put(ent.Id, (dynamic)entity[componentId].Deserialize(cTypes[componentId], serializerOptions));
            

        return ent.Id;
    }

    public static LevelInfo GetLevelInfo(string saveFolderName) =>
        JsonHelper.Deserialize<LevelInfo>(PathHelper.GetFullPathToLevelInfo(saveFolderName));

    public static void MakeSave(World world, LevelInfo currentLevel, string saveFolderName)
    {
        PathHelper.CreateSaveFolderIfNotExists(saveFolderName);
        
        PathHelper.WriteToLevelInfo(saveFolderName, JsonHelper.Serialize(currentLevel));
        PathHelper.WriteToScreenInfo(saveFolderName, currentLevel ,GetJsonStringFromEntities(world));
    }

    public static void LoadSave(World world, LevelInfo currentLevel)
    {
        
    }
}