using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MonoGame.Extended.Collections;
using Underall.Components;
using Underall.Helpers;

namespace Underall.Helpers;

using MonoGame.Extended.Entities;

/// <summary>
/// SaveHelper fetches game data: entities and components.
/// </summary>
public class SaveHelper
{

    /// <summary>
    /// Returns bool array where true at index X means that entity with X Id is alive.
    /// </summary>
    /// <remarks>
    /// Attention! This works fine until you want to use EntityAdded, EntityRemoved, EntityChanged actions
    /// because it does not preserve fields _removedEntities and _changedEntities.
    /// </remarks>
    /// <returns></returns>
    private static bool[] FetchActiveEntities(World world, EntityManager entityManager)
    {
        var entities = ReflectionHelper.GetPrivateField<EntityManager, Bag<Entity>>(entityManager, "_entityBag");
        var removed = ReflectionHelper.GetPrivateField<EntityManager, Bag<int>>(entityManager, "_removedEntities").ToHashSet();
        
        var fetched = new bool[entities.Count];
        for (var entityId = 0; entityId < removed.Count; entityId++)
            fetched[entityId] = !removed.Contains(entityId);

        return fetched;
    }

    public static JsonArray GetJsonArrayFromEntities(World world)
    {
        var entityManager = ReflectionHelper.GetPrivateProperty<World, EntityManager>(world, "EntityManager");
        var entitiesAlive = FetchActiveEntities(world, entityManager);
        return JsonHelper.GetAllEntitiesComponentsSerialized(entityManager, ComponentRegistry.Mappers, entitiesAlive,
            new JsonSerializerOptions { IncludeFields = true });
    }
}