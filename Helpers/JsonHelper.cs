using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Serialization;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Helpers;

public static class JsonHelper
{
    public static JsonContentLoader JsonContentLoader = new JsonContentLoader();
    
    public static JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        AllowTrailingCommas = true,
        IncludeFields = true,
    };

    public static JsonDocumentOptions DefaultDocumentOptions = new JsonDocumentOptions
    {
        AllowTrailingCommas = true,
    };

    public static T Deserialize<T>(string filePath)
    {
        using var fileStream = File.Open(filePath, FileMode.Open);
        return JsonSerializer.Deserialize<T>(fileStream, DefaultSerializerOptions);
    }

    public static string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, DefaultSerializerOptions);
    }
    
    /// <summary>
    /// Serializes all components of an entity into tightly packed array
    /// where order of components is dictated by rawMappers order
    /// </summary>
    /// <param name="rawMappers">List of non-generic ComponentMappers that can be used as generic</param>
    /// <param name="componentBits">Bool vector where true at index X means entity has component with X id</param>
    private static JsonArray GetComponentsSerialized(List<ComponentMapper> rawMappers, BitVector32 componentBits, 
        int entityId, JsonSerializerOptions options)
    {
        // componentBits: true at index X means that entity has component with X id
        ComponentRegistry.AssertComponentsCountUnderOrEqual32();
        var array = new JsonArray();
        
        for (var componentId = 0; componentId < ComponentRegistry.TypesAmount; componentId++)
        {
            var dynamicMapper = (dynamic)rawMappers[componentId];
            array.Add(
                dynamicMapper.Has(entityId) ? JsonSerializer.SerializeToNode((object)dynamicMapper.Get(entityId), options)
                    : null
                );
        }
    
        return array;
    }

    /// <summary>
    /// Serializes all entities alive and their components into tightly packed JsonArray
    /// </summary>
    /// <param name="rawMappers">List of non-generic ComponentMappers that can be used as generic</param>
    /// <param name="entitiesAlive">Bool array where true at index X means entity with X id is alive</param>
    public static JsonArray GetAllEntitiesComponentsSerialized(EntityManager entityManager, List<ComponentMapper> rawMappers, 
        bool[] entitiesAlive, JsonSerializerOptions options)
    {
        var array = new JsonArray();
        for (var entId = 0; entId < entitiesAlive.Length; entId++)
            array.Add(
                entitiesAlive[entId] ?
                    GetComponentsSerialized(rawMappers, entityManager.GetComponentBits(entId), entId, options)
                    : null
                );
        return array;
    }


    /// <returns>Either array or a single element stored in file</returns>
    public static JsonDocument GetJsonDocument(string filePath, JsonDocumentOptions documentOptions)
    {
        using var fileStream = File.Open(filePath, FileMode.Open);
        return JsonDocument.Parse(fileStream, documentOptions);
    }

    public static ConfigInfo GetGameConfig() => Deserialize<ConfigInfo>(PathHelper.ConfigFullPath);
}