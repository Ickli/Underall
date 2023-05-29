using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Systems;

using Underall.World;

public class SControls: EntityProcessingSystem
{
    private ComponentMapper<CVelocity> ControllableVelocityMapper;
    private ComponentMapper<CSizePosition> SizePositionMapper { get; set; }
    private World _world;

    private int _negativeVelocityLimit;
    private int _positiveVelocityLimit;
    private int _timeDif;

    public SControls(Game1 game, World world, ConfigInfo config)
    : base(Aspect.All(typeof(CControllable)))
    {
        _timeDif = game.timeDif;
        _world = world;
        _negativeVelocityLimit = config.NegativeVelocityLimit;
        _positiveVelocityLimit = config.PositiveVelocityLimit;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        ControllableVelocityMapper = mapperService.GetMapper<CVelocity>();
        SizePositionMapper = mapperService.GetMapper<CSizePosition>();
    }

    public override void Process(GameTime time, int entId)
    {
        _world.ChangeGridIfControllableOutOfScreen(SizePositionMapper.Get(entId));
        var controllableVelocity = ControllableVelocityMapper.Get(entId);
        if (controllableVelocity.IsFalling) return;
        var keyb = Keyboard.GetState();

        if(keyb.IsKeyDown(Keys.W))
        {
            controllableVelocity.Vector.Y =
                Math.Max(controllableVelocity.Vector.Y - 2 * _timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10,
                    _negativeVelocityLimit * 2);
            // controllableVelocity.IsFalling = true;
        }
        if(keyb.IsKeyDown(Keys.S))
            controllableVelocity.Vector.Y = Math.Min(controllableVelocity.Vector.Y + _timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _positiveVelocityLimit);
        if(keyb.IsKeyDown(Keys.A))
            controllableVelocity.Vector.X = Math.Max(controllableVelocity.Vector.X - _timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _negativeVelocityLimit);
        if(keyb.IsKeyDown(Keys.D))
            controllableVelocity.Vector.X = Math.Min(controllableVelocity.Vector.X + _timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _positiveVelocityLimit);
        if (keyb.IsKeyDown(Keys.Space))
            _world.CurrentLevel.SwitchToNextDimension();
        if (keyb.IsKeyDown(Keys.X))
        {
            var statInfo = _world.ComponentManager.GetMapper<CBasicStats>().Get(_world.ControllableEntityId);
            statInfo.Sanity.ChangeCurrent(statInfo.Sanity.Current - 5);
        }
            
    }
}