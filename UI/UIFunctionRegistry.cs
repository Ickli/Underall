using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Underall.UI.Buttons;
using Underall.UI.Interfaces;

namespace Underall.UI;

/// <summary>
/// Contains functions which any button can have. Usually, a function is got via its name
/// </summary>
public static class UIFunctionRegistry
{
    public static readonly Dictionary<string, Action<UIEventArgs>> Functions = new Dictionary<string, Action<UIEventArgs>>
    {
        {"Test", Test},
        {"LoadMainMenuScreen", LoadMainMenuScreen},
        {"LoadPauseMenuScreen",LoadPauseMenuScreen},
        {"LoadSaveMenuToSave", LoadSaveMenuToSave},
        {"LoadSaveMenuToLoad", LoadSaveMenuToLoad},
        {"ReturnToPreviousScreen", ReturnToPreviousScreen},
        {"Exit", Exit},
        {"StartNewGame", StartNewGame},
        {"LoadSave", LoadSave},
        {"MakeSave", MakeSave},
        {"DummyFunction", DummyFunction}
    };

    private static void Test(UIEventArgs args) =>
        Console.WriteLine("button was clicked!");
    
    private static void LoadMainMenuScreen(UIEventArgs args) => args.Game.LoadMainMenuScreen();

    private static void LoadPauseMenuScreen(UIEventArgs args) => args.Game.LoadPauseMenuScreen();

    private static void LoadSaveMenuToSave(UIEventArgs args) => args.Game.LoadSaveMenuToSave();

    private static void LoadSaveMenuToLoad(UIEventArgs args) => args.Game.LoadSaveMenuToLoad();

    private static void ReturnToPreviousScreen(UIEventArgs args) => args.Game.ReturnToPreviousScreen();
    
    private static void Exit(UIEventArgs args) => args.Game.Exit();

    private static void StartNewGame(UIEventArgs args) => args.Game.LoadGameplayScreen();

    private static void LoadSave(UIEventArgs args) => args.Game.LoadGameplayScreen(((ILabeled)args.Sender).Label);

    private static void MakeSave(UIEventArgs args) => args.Game.World.MakeSave(((ILabeled)args.Sender).Label);

    private static void DummyFunction(UIEventArgs args) => Console.WriteLine("This UI element has a dummy function that is " +
                                                                             "to be replaced with another one");
}