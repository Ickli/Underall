using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using Underall.Helpers;

namespace Underall.Components;

using MonoGame.Extended.Entities;
using MonoGame.Extended.Content;

/// <summary>
/// ComponentRegistry contains types of all existing components
/// and order in which they should be initialized in ComponentManager
/// Changing the order can lead to UB; appending new types is allowed.
/// </summary>
public static class ComponentRegistry
{
    // this limitation is due to technical issues in MonoGame.Extended
    private static int _maxComponentsCount = 32;
    public static int DimensionsAmount = Enum.GetNames(typeof(Dimensions)).Length;
    public static bool IsInitialized = false;


    public static void InitializeComponentMappers(ComponentManager manager)
    {
        IsInitialized = true;

        InitializeMapper(manager, new CMeta());
        InitializeMapper(manager, new CSizePosition());
        InitializeMapper(manager, new CVelocity());
        InitializeMapper(manager, new CAnimatedSprite());
        InitializeMapper(manager, new CSprite());
        InitializeMapper(manager, new CControllable());
        InitializeMapper(manager, new CWalkableBy());
        InitializeMapper(manager, new CBasicStats());
    }
    
    // These mappers are shared between ComponentRegistry and ComponentManager of current world
    public static List<ComponentMapper> Mappers = new List<ComponentMapper>();
    
    public static Type[] GetTypes()
    {
        var array = new Type[TypesAmount];
        for (var i = 0; i < TypesAmount; i++)
            array[i] = Mappers[i].ComponentType;
        return array;
    }

    public static void AssertComponentsCountUnderOrEqual32()
    {
        if (TypesAmount > _maxComponentsCount)
            throw new ConstraintException(
                "Maximal number of components is 32 due to technical issues in Monogame.Extended");
    }

    private static ComponentMapper<CType> InitializeMapper<CType>(ComponentManager manager, CType component)
    where CType: class
    {
        var mapper = manager.GetMapper<CType>();
        Mappers.Add(mapper);
        return mapper;
    }

    public static void BindComponentSources(ComponentManager componentManager, ContentManager contentManager)
    {
        BindTexturesToSpriteComponents(componentManager, contentManager);
    }
    
    /// <summary>
    /// Makes up deserialized fileNames and dimension parameters into Sprite/AnimatedSprite object
    /// and bounds it to component
    /// </summary>
    private static void BindTexturesToSpriteComponents(ComponentManager componentManager, ContentManager contentManager)
    {
        var cSprites = componentManager.GetMapper<CSprite>().Components;
        var cAnimatedSprites = componentManager.GetMapper<CAnimatedSprite>().Components;
        
        // Bounding CSprite 
        for(var componentIndex = 0; componentIndex < cSprites.Count; componentIndex++)
        {
            var component = cSprites[componentIndex];
            if (component is null) continue;
            component.Sprites = new Sprite[DimensionsAmount];
            for (var dim = 0; dim < DimensionsAmount; dim++)
                component.Sprites[dim] = new Sprite(
                    new TextureRegion2D(
                        contentManager.Load<Texture2D>(component.TextureNames[dim]), 
                        component.Xs[dim], 
                        component.Ys[dim], 
                        component.Widths[dim], 
                        component.Heights[dim]
                    )
                );
        }
        
        // Bounding CAnimatedSprite
        var jsonContentLoader = JsonHelper.JsonContentLoader;
        for (var componentIndex = 0; componentIndex < cAnimatedSprites.Count; componentIndex++)
        {
            var component = cAnimatedSprites[componentIndex];
            for(var dim = 0; dim < DimensionsAmount; dim++)
                component.Sprites[dim] = new MonoGame.Extended.Sprites.AnimatedSprite(
                    contentManager.Load<SpriteSheet>(component.SpriteSheetNames[dim], jsonContentLoader)
                );
        }
    }

    public static int TypesAmount => Mappers.Count;
}