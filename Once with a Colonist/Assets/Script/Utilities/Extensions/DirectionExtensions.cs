using UnityEngine;

namespace TendedTarsier.Script.Utilities.Extensions
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public static class DirectionExtensions
    {
        public static Vector3 ToVector3(this Direction container)
        {
            return container switch
            {
                Direction.Left => Vector3.left,
                Direction.Right => Vector3.right,
                Direction.Up => Vector3.up,
                Direction.Down => Vector3.down,
                _ => Vector3.zero
            };
        }
    }
}