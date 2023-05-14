using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Nodes;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Entities;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Helpers;

public static class JsonHelper
{
    public static LevelInfo ParseLevelInfo(string path)
    {
        // TODO
        throw new NotImplementedException();
    }

    public static SaveInfo ParseSaveInfo(string path)
    {
        // TODO: SaveInfo class is not implemented too
        throw new NotImplementedException();
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
        // for(var componentId = 0; componentId < ComponentRegistry.TypesAmount; componentId++)
        // {
        //     var dynamicMapper = (dynamic)rawMappers[componentId];
        //     array.Add(
        //         componentBits[componentId] ? // if componentBits have a component
        //             JsonSerializer.SerializeToNode((object)dynamicMapper.Get(entityId), options)  // add component to array
        //             : null // else, add null
        //     );
        // }
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
            array.Add(GetComponentsSerialized(rawMappers, entityManager.GetComponentBits(entId), entId, options));
        return array;
    }
}


/*
for (var e = 0; e < entites.Count; e++)
    for(var m = 0; m < mappers.Count; m++)
        if (m.hasEntity(e))
        eComponents.Add(m[e].ParseToNode)

*/