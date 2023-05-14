using System;
using System.ComponentModel.Design;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Systems;

public class SPhysics: EntityUpdateSystem
{
    private ComponentMapper<CVelocity> VelocityMapper;
    private ComponentMapper<CSizePosition> PositionMapper;
    private int _positiveVelocityLimit;
    private int _negativeVelocityLimit;

    public SPhysics(ConfigInfo config) : base(Aspect.All(typeof(CVelocity), typeof(CSizePosition)))
    {
        _positiveVelocityLimit = config.PositiveVelocityLimit;
        _negativeVelocityLimit = config.NegativeVelocityLimit;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        VelocityMapper = mapperService.GetMapper<CVelocity>();
        PositionMapper = mapperService.GetMapper<CSizePosition>();
    }


    public override void Update(GameTime time)
    {
        foreach (var ent in ActiveEntities)
        {
            ref var pos = ref PositionMapper.Get(ent).Location;
            ref var velocity = ref VelocityMapper.Get(ent).Vector;

            pos.X += velocity.X * (float)time.ElapsedGameTime.TotalMilliseconds / 10;
            pos.Y += velocity.Y * (float)time.ElapsedGameTime.TotalMilliseconds / 10;
            velocity.X += -Math.Sign(velocity.X);
            velocity.Y += -Math.Sign(velocity.Y);
        }
    }
}