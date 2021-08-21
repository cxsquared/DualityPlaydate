using Microsoft.Xna.Framework;

namespace DualityPlaydate.Component
{
    public struct Transform
    {
        public Vector2 Position;
        public float Rotation;
        public float Scale;

        public Transform(Vector2 position, float rotation, float scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }
    }
}
