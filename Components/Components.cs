using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Underall;

// components don't have OnRun overriden. They're additional data
# region PhysicsComponents
public class CVelocity: AbstractComponent
{
    public Vector2 Vector;
}

# endregion


// components don't have OnRun overriden. They're additional data
# region SizeAndPositionComponents
public class CHitBox : AbstractComponent
{
    public Vector2 Size;
}

public class CPosition : AbstractComponent
{
    public Vector2 Position;
}

# endregion


public class 