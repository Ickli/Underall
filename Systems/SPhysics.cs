using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Underall.Components;
using Underall.MetaInfo;
using Underall.World;

namespace Underall.Systems;

using Underall.World;

public class SPhysics: EntityUpdateSystem
{
    private World World { get; set; }
    
    private ComponentMapper<CVelocity> VelocityMapper { get; set; }
    private ComponentMapper<CSizePosition> SizePositionMapper { get; set; }
    private ComponentMapper<CWalkableBy> WalkableByMapper { get; set; }
    private int PositiveVelocityLimit { get; set; }
    private int NegativeVelocityLimit { get; set; }
    private float EpsilonVelocity { get; set; }

    private float _acceleration = 2; // TODO: CHANGE THIS

    public SPhysics(World world, ConfigInfo config) : base(Aspect.All(typeof(CVelocity), typeof(CSizePosition), typeof(CWalkableBy)))
    {
        World = world;
        PositiveVelocityLimit = config.PositiveVelocityLimit;
        NegativeVelocityLimit = config.NegativeVelocityLimit;
        EpsilonVelocity = config.EpsilonVelocity;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        VelocityMapper = mapperService.GetMapper<CVelocity>();
        SizePositionMapper = mapperService.GetMapper<CSizePosition>();
        WalkableByMapper = mapperService.GetMapper<CWalkableBy>();
    }


    public override void Update(GameTime time)
    {
        for(var i = 0; i < ActiveEntities.Count; i++)
        {
            var ent = ActiveEntities[i];
            var cSizePosition = SizePositionMapper.Get(ent);
            var cVelocity =  VelocityMapper.Get(ent);
            var cWalkableBy = WalkableByMapper.Get(ent);
            
            World.Grid.RemoveEntity(cSizePosition, ent);
            // cVelocity.Vector.Y += _acceleration;
            
            // horizontal check
            UpdatePosition(time, ref cSizePosition.Location, cVelocity.Vector.X, 0);
            if (CollidesWithSomething(cSizePosition, cWalkableBy, ent, out var collidedEntityId,
                    out var collidedCSizePosition))
            {
                UpdatePosition(time, ref cSizePosition.Location, -cVelocity.Vector.X, 0);
                cVelocity.Vector.X = 0;
                // continue;
            }
            
            // vertical check
            UpdatePosition(time, ref cSizePosition.Location, -cVelocity.Vector.X, cVelocity.Vector.Y);
            if (CollidesWithSomething(cSizePosition, cWalkableBy, ent, out collidedEntityId,
                    out collidedCSizePosition))
            {
                UpdatePosition(time, ref cSizePosition.Location, cVelocity.Vector.X, -cVelocity.Vector.Y);
                cVelocity.Vector.Y = 0;
                cVelocity.IsFalling = false;
            }
            else UpdatePosition(time, ref cSizePosition.Location, cVelocity.Vector.X, 0);

            World.Grid.TryAddEntity(cSizePosition, ent);
            // UpdatePosition(time, ref cSizePosition.Location, cVelocity.Vector.X, _acceleration);
            UpdateVelocity(time, ref cVelocity.Vector);
        }
    }

    private void UpdatePosition(GameTime time, ref Vector2 position, ref Vector2 velocity)
    {
        UpdatePosition(time, ref position, velocity.X, velocity.Y);
    }

    private void UpdatePosition(GameTime time, ref Vector2 position, float x, float y)
    {
        position.X += x * (float)time.ElapsedGameTime.TotalMilliseconds / 10;
        position.Y += y * (float)time.ElapsedGameTime.TotalMilliseconds / 10;
    }

    private void UpdateVelocity(GameTime time, ref Vector2 velocity)
    {
        if (Math.Abs(velocity.X) < EpsilonVelocity) velocity.X = 0;
        if (Math.Abs(velocity.Y) < EpsilonVelocity) velocity.Y = 0;
        velocity.X += -Math.Sign(velocity.X);
        velocity.Y += -Math.Sign(velocity.Y);
    }

    /// <summary>
    /// Checks if passed cSizePosition collides with something in the grid 
    /// </summary>
    /// <returns>First entity with which collision happens</returns>
    private bool CollidesWithSomething(CSizePosition cSizePosition, CWalkableBy cWalkable, int entId,
        out int collidedEntityId, out CSizePosition collidedCSizePosition)
    {
        collidedEntityId = -1;
        collidedCSizePosition = null;

        var cells = World.Grid.GetCellsWhere(cSizePosition);

        for (var cellIndex = 0; cellIndex < cells.Count; cellIndex++)
        {
            var cell = cells[cellIndex];
            for (var i = 0; i < cell.Count; i++)
            {
                var neighbourEntId = cell.Values[i];
                if (entId == neighbourEntId) continue;
                if (IsWalkableByInCurrentDimension(WalkableByMapper.Get(neighbourEntId)))
                    continue;
                var otherCSizePosition = SizePositionMapper.Get(neighbourEntId);
                if (!cSizePosition.Intersects(otherCSizePosition))
                    continue;
                
                collidedEntityId = neighbourEntId;
                collidedCSizePosition = otherCSizePosition;
                return true;
            }
        }
        return false;
    }

    private bool IsWalkableByInCurrentDimension(CWalkableBy cWalkableBy) =>
        cWalkableBy.Dimensions[World.CurrentLevel.CurrentLevelId];
}