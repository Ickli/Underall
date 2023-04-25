using System;

namespace Underall;

public class AbstractComponent
{
    public Guid Guid { get; set; }
    public virtual void OnRun() {}
}