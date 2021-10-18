using Microsoft.Xna.Framework;

namespace TrexGame.Entities
{
    public interface IDayNightCycle
    {
        int NightCount { get; }

        bool IsNight { get; }

        Color ClearColor { get; }
    }
}
