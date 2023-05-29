using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Underall.MetaInfo;
using MenuButton = Underall.UI.Buttons.RelativeButton;

namespace Underall.Screens;


public class ScSaveList: ScMenu
{
    /// <summary>
    /// Screen which lists all saves in SAVE_FOLDER as buttons.
    /// Buttons can be either saving or loading a game.
    /// </summary>
    /// <param name="buttons">Buttons matched with saves</param>
    public ScSaveList(Game1 game, SpriteBatch spriteBatch, List<MenuButton> buttons, 
        string backgroundName, Vector2 backgroundScale, ConfigInfo config) 
        : base(game, spriteBatch, buttons, backgroundName, backgroundScale)
    {
        Buttons.Add(config.LoadMenu_ReturnButton.Initialized(game.UserInterface, spriteBatch, game.Content, game.UserInterface.ScreenDimensions));
    }
}