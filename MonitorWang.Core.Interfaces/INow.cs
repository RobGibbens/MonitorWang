using System;

namespace MonitorWang.Core.Interfaces
{
    public interface INow
    {
        DateTime Now();
        DateTime UtcNow();
    }
}