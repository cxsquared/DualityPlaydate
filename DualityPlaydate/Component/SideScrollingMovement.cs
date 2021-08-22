using Microsoft.Xna.Framework;

namespace DualityPlaydate.Component
{
    public struct SideScrollingMovement
    {
        public Vector2 Velocity;
        public float Speed;
        public float MaxSpeed;
        public float Friction;

        public SideScrollingMovement(float speed, float max, float friction)
        {
            Speed = speed;
            MaxSpeed = max;
            Friction = friction;
            Velocity = Vector2.Zero;
        }
    }
}
