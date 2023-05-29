using Microsoft.Xna.Framework;
using Underall.UI.Bars;
using Underall.UI.Buttons;
using Underall.UI.Frames;

using MenuButton = Underall.UI.Buttons.RelativeButton;
namespace Underall.MetaInfo;

public record ConfigInfo
{
    public string DefaultLevelName;
    
    public int PositiveVelocityLimit;
    public int NegativeVelocityLimit;
    public int DefaultLayeredGridDepth;
    public float EpsilonVelocity;

    public RelativeFrameTextItem StandardRelativeFrameTextItem;
    
    public StatBarBuilder HealthBarBuilder;
    public StatBarBuilder SanityBarBuilder;
    public StatBarBuilder StaminaBarBuilder;
    public RelativeFrame DialogueFrame;

    public string MainMenuBackgroundName;
    public Vector2 MainMenuBackgroundScale;
    public MenuButton MainMenu_StartGameButton;
    public MenuButton MainMenu_LoadSaveButton;
    public MenuButton MainMenu_ExitButton;

    public string LoadMenuBackgroundName;
    public Vector2 LoadMenuBackgroundScale;
    public Vector2 LoadMenuTextScale;
    public MenuButton LoadMenu_LongButton;
    public MenuButton LoadMenu_ReturnButton;
    
    public string PauseMenuBackgroundName;
    public Vector2 PauseMenuBackgroundScale;
    public MenuButton PauseMenu_ReturnButton;
    public MenuButton PauseMenu_MakeSaveButton;
    public MenuButton PauseMenu_LoadSaveButton;
    public MenuButton PauseMenu_ExitButton;
}