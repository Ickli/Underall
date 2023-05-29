using System;

namespace Underall.UI;

public class UIEventArgs: EventArgs
{
    public object Sender { get; private set; }
    public Game1 Game { get; private set; }
    public string SaveName { get; private set; }

    public UIEventArgs(object sender, Game1 game)
    {
        Sender = sender;
        Game = game;
    }
}