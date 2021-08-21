using Microsoft.Xna.Framework;

namespace DualityPlaydate.Component
{
    public struct Movement
    {
        public Vector2 Velocity;
        public float Speed;
        public float MaxSpeed;
        public float Friction;

        public Movement(float speed, float max, float friction)
        {
            Speed = speed;
            MaxSpeed = max;
            Friction = friction;
            Velocity = Vector2.Zero;
        }
    }
}
