using System;
using System.Numerics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Sprites;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Systems;

using Underall.World;
using Microsoft.Xna.Framework;


public class SDraw: EntityDrawSystem
{
    private World World; 
    private ComponentMapper<CSprite> SpriteMapper { get; set; }
    private ComponentMapper<CSizePosition> SizePositionMapper { get; set; }
    public SpriteBatch SpriteBatch { get; private set; }
    public GraphicsDevice GraphicsDevice { get; private set; }

    private Texture2D[] BackgroundTextures { get; set; }
    private Vector2[] BackgroundScales { get; set; }
    private Vector2 _backgroundOrigin = new Vector2(0, 0);
    
    public SDraw(World world, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, ContentManager contentManager, LevelInfo currentLevel): 
        base(Aspect.All(typeof(CSprite)).Exclude(typeof(CAnimatedSprite)))
    {
        World = world;
        GraphicsDevice = graphicsDevice;
        SpriteBatch = spriteBatch;


        ChangeBackgrounds(contentManager, currentLevel.BackgroundFileNames[currentLevel.CurrentLevelId],
            currentLevel.BackgroundScales);
    }
    
    public override void Initialize(IComponentMapperService mapperService)
    {
        SpriteMapper = mapperService.GetMapper<CSprite>();
        SizePositionMapper = mapperService.GetMapper<CSizePosition>();
    }

    public override void Draw(GameTime time)
    {
        GraphicsDevice.Clear(Color.Black);
        SpriteBatch.Begin();
        DrawBackground();
        var maxDepth = World.Grid.LayeredGrid.Count;
        var dimension = (int)World.CurrentLevel.CurrentDimension;
        for (var depth = 0; depth < maxDepth; depth++)
            foreach (var entId in World.Grid.LayeredGrid[depth])
                SpriteBatch.Draw(SpriteMapper.Get(entId).Sprites[dimension], SizePositionMapper.Get(entId).Location - World.Grid.TopLeftCorner);
        // SpriteBatch.Draw(Sphere, World.ControllablePos.Location - new Vector2(600, 600), Color.White);
        
        
        SpriteBatch.End();
    }

    private void ChangeBackgrounds(ContentManager contentManager, string[] textureNames, Vector2[] scales)
    {
        var newTextures = new Texture2D[textureNames.Length];
        for (var t = 0; t < newTextures.Length; t++)
            newTextures[t] = contentManager.Load<Texture2D>(textureNames[t]);
        ChangeBackgrounds(newTextures, scales);
    }
    
    private void ChangeBackgrounds(Texture2D[] newTextures, Vector2[] scales)
    {
        if (newTextures.Length != scales.Length)
            throw new ArgumentException("newTextures and scales arrays are not the same length");
        if (newTextures.Length < ComponentRegistry.DimensionsAmount)
            throw new ArgumentException($"Method hasn't been provided with enough textures: needed: " +
                                        $"{ComponentRegistry.DimensionsAmount}," +
                                        $"actual amount {newTextures.Length}");

        BackgroundTextures = newTextures;
        BackgroundScales = scales;
    }
    private void ChangeBackground(Texture2D newTexture, Vector2 scale, Dimensions dimension)
    {
        BackgroundTextures[(int)dimension] = newTexture;
        BackgroundScales[(int)dimension] = scale;
    }

    private void DrawBackground()
    {
        var dimension = (int)World.CurrentLevel.CurrentDimension;
        SpriteBatch.Draw(
            BackgroundTextures[dimension],
             -World.Grid.TopLeftCorner,
            null,
            Color.White,
            0,
            _backgroundOrigin,
            BackgroundScales[dimension],
            SpriteEffects.None,
            0);
    }
}