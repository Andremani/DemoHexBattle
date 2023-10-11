using UnityEngine;

namespace Andremani.DemoHexBattle.Enums
{
    public enum BattleSide
    {
        Left,
        Right
    }

    public static class BattleSideMethods
    {
        public static Color GetColor(this BattleSide battleSide)
        {
            switch (battleSide)
            {
                case BattleSide.Left:
                    return Color.red;
                case BattleSide.Right:
                    return Color.blue;
                default:
                    return Color.green;
            }
        }
    }
}