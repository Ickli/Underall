using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using MonoGame.Extended.Input.InputListeners;
using Underall.Components;
using Underall.MetaInfo;

namespace Underall.Systems;

public class SControls: EntityProcessingSystem
{
    private ComponentMapper<CVelocity> ControllableVelocityMapper;
    private Entity controllableEntity { get; set; }
    // private CPosition controllablePos;
    // private CVelocity controlableVelocity;

    private int _negativeVelocityLimit;
    private int _positiveVelocityLimit;
    private int timeDif;

    public SControls(Game1 game, ConfigInfo config)
    : base(Aspect.All(typeof(CControllable)))
    {
        timeDif = game.timeDif;
        // controllablePos = controllableEntity.Get<CPosition>();
        // controlableVelocity = controllableEntity.Get<CVelocity>();
        _negativeVelocityLimit = config.ControllableNegativeVelocityLimit;
        _positiveVelocityLimit = config.ControllablePositiveVelocityLimit;
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        ControllableVelocityMapper = mapperService.GetMapper<CVelocity>();
    }

    public override void Process(GameTime time, int entId)
    {
        var keyb = Keyboard.GetState();
        var controllableVelocity = ControllableVelocityMapper.Get(entId);
        
        if(keyb.IsKeyDown(Keys.W))
            controllableVelocity.Vector.Y = Math.Max(controllableVelocity.Vector.Y - timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _negativeVelocityLimit);
        if(keyb.IsKeyDown(Keys.S))
            controllableVelocity.Vector.Y = Math.Min(controllableVelocity.Vector.Y + timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _positiveVelocityLimit);
        if(keyb.IsKeyDown(Keys.A))
            controllableVelocity.Vector.X = Math.Max(controllableVelocity.Vector.X - timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _negativeVelocityLimit);
        if(keyb.IsKeyDown(Keys.D))
            controllableVelocity.Vector.X = Math.Min(controllableVelocity.Vector.X + timeDif * (float)time.ElapsedGameTime.TotalMilliseconds / 10, _positiveVelocityLimit);
    }
}

// using System;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Input;
// using MonoGame.Extended.Entities;
// using MonoGame.Extended.Input.InputListeners;
// using Underall.Components;
// using Underall.MetaInfo;
//
// namespace Underall.Systems;
//
// public class SControls
// {
//     private Entity ControllableEntity { get; set; }
//     private CPosition ControllablePos;
//     private CVelocity ControlableVelocity;
//
//     private int _negativeVelocityLimit;
//     private int _positiveVelocityLimit;
//     private int timeDif;
//
//     public SControls(Game1 game, Entity controllableEntity, ConfigInfo config)
//     {
//         var keyboardListener = new KeyboardListener(new KeyboardListenerSettings
//         {
//             RepeatPress = true,
//             InitialDelayMilliseconds = 50, // start sending signals after time elapsed
//             RepeatDelayMilliseconds = 50 // sends signals at the interval
//         });
//         timeDif = game.timeDif;
//         ControllablePos = controllableEntity.Get<CPosition>();
//         ControlableVelocity = controllableEntity.Get<CVelocity>();
//         _negativeVelocityLimit = config.ControllableNegativeVelocityLimit;
//         _positiveVelocityLimit = config.ControllablePositiveVelocityLimit;
//         
//         
//         keyboardListener.KeyPressed += (sender, args) =>
//         {
//             var keyb = Keyboard.GetState();
//
//             if(keyb.IsKeyDown(Keys.W))
//                 ControlableVelocity.Vector.Y = Math.Max(ControlableVelocity.Vector.Y - timeDif, _negativeVelocityLimit);
//             if(keyb.IsKeyDown(Keys.S))
//                 ControlableVelocity.Vector.Y = Math.Min(ControlableVelocity.Vector.Y + timeDif, _positiveVelocityLimit);
//             if(keyb.IsKeyDown(Keys.A))
//                 ControlableVelocity.Vector.X = Math.Max(ControlableVelocity.Vector.X - timeDif, _negativeVelocityLimit);
//             if(keyb.IsKeyDown(Keys.D))
//                 ControlableVelocity.Vector.X = Math.Min(ControlableVelocity.Vector.X + timeDif, _positiveVelocityLimit);
//                         
//         };
//         
//         ControllableEntity = controllableEntity;
//         game.Components.Add(new InputListenerComponent(game, keyboardListener));
//     }
// }