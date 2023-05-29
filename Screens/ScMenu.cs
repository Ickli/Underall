using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using Underall.MetaInfo;
using Underall.UI.Interfaces;

using MenuButton = Underall.UI.Buttons.RelativeButton;

namespace Underall.Screens;

/// <summary>
/// Screen menu is basic screen with buttons and background.
/// </summary>
/// <remarks>
/// ScMenu should be detached by functionality from UserInterface
/// </remarks>
public class ScMenu: GameScreen
{
    protected static Vector2 Origin = new Vector2(0, 0);
    protected static int Depth = 0;
    protected SpriteBatch SpriteBatch { get; set; }
    protected Texture2D Background { get; set; }
    protected Vector2 BackgroundScale { get; set; }
    protected Vector2 BackgroundPosition = new Vector2(0, 0);
    protected List<MenuButton> Buttons { get; set; }
    protected Point MousePosition { get; set; }
    protected Point PrevMousePosition { get; set; }
    protected bool WasMouseLeftPressed { get; set; }
    protected MenuButton HoveredButton { get; set; }
    protected new Game1 Game => (Game1)base.Game;

    public ScMenu(Game1 game, SpriteBatch spriteBatch, List<MenuButton> buttons, string backgroundName, Vector2 backgroundScale) : base(game)
    {
        base.Initialize();
        SpriteBatch = spriteBatch;
        Buttons = buttons;
        PrevMousePosition = new Point(0, 0);
        WasMouseLeftPressed = false;
        Background = game.Content.Load<Texture2D>(backgroundName);
        BackgroundScale = backgroundScale;
    }

    public override void Update(GameTime time)
    {
        var mouse = Mouse.GetState();
        var isLeftButtonPressed = mouse.LeftButton == ButtonState.Pressed;
        PrevMousePosition = MousePosition;
        MousePosition = mouse.Position;
        
        if (HoveredButton is not null && isLeftButtonPressed && !WasMouseLeftPressed)
            ((IClickable)HoveredButton).Click(Game);
        WasMouseLeftPressed = isLeftButtonPressed;
        
        if (MousePosition != PrevMousePosition // if position is changed
            && TryUpdateHoveredButton(MousePosition) // next hovered button is different
            && HoveredButton is not null // cursor is on a button
            && HoveredButton.OnHoverSoundEffect is not null) // the button has sound effect
            HoveredButton.OnHoverSoundEffect.Play();
    }

    public sealed override void Draw(GameTime time)
    {
        SpriteBatch.GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch.Begin(SpriteSortMode.Deferred);
        SpriteBatch.Draw(Background, 
            BackgroundPosition, 
            null, 
            Color.White, 
            0,
            Origin, 
            BackgroundScale, 
            SpriteEffects.None, 
            Depth
            );
        foreach (var button in Buttons)
            button.Draw(button.AbsoluteOffset);
        if(HoveredButton != null) 
            HoveredButton.DrawHovered();
        SpriteBatch.End();
    }

    private MenuButton? GetHoveredButton(Point mousePosition)
    {
        // In case if other other menus (settings, saves, etc) will overlap main one
        return Enumerable.Reverse(Buttons).FirstOrDefault(button => ((IHoverable)button).IsHovered(
            button.AbsoluteOffset, (int)button.Dimensions.X,
            (int)button.Dimensions.Y, mousePosition));
    }

    /// <summary>
    /// Returns true if hovered button is changed
    /// </summary>
    private bool TryUpdateHoveredButton(Point mousePosition)
    {
        var newHoveredButton = GetHoveredButton(mousePosition);
        if (HoveredButton == newHoveredButton) return false;
        HoveredButton = newHoveredButton;
        return true;
    }
}