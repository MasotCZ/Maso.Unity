using System;

namespace Game.Serialization
{
    // move somewhere in Editor ?
    [Serializable]
    [Flags]
    public enum EMetaTags
    {
        Structure = 1,
        Item = 2,
        Unit = 4,
        Placeable = 8,
    }
}