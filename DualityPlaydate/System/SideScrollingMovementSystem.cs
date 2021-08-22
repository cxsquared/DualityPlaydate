using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using System;

namespace DualityPlaydate.System
{
    [With(typeof(Transform), typeof(SideScrollingMovement), typeof(Inputs))]
    class SideScrollingMovementSystem : AEntitySetSystem<float>
    {
        public SideScrollingMovementSystem(World world)
            : base(world)
        {
        }

        protected override void Update(float frameDelta, in Entity entity)
        {
            ref var trans = ref entity.Get<Transform>();
            ref var movement = ref entity.Get<SideScrollingMovement>();
            ref readonly var input = ref entity.Get<Inputs>();

            if (!HasMovementInput(in input))
            {
                ApplyFriction(ref movement, frameDelta);
                return;
            }

            ApplyVelocity(ref movement, in input, frameDelta);
            UpdateTransform(ref trans, in movement);
        }

        static bool HasMovementInput(in Inputs input)
        {
            return input.Right || input.Left;
        }

        static void ApplyFriction(ref SideScrollingMovement movement, float frameDelta)
        {
            movement.Velocity.X *= movement.Friction * frameDelta;

            if (movement.Velocity.X < 0.1 && movement.Velocity.X > -0.1)
                movement.Velocity.X = 0;
        }

        static void ApplyVelocity(ref SideScrollingMovement movement, in Inputs input, float frameDelta)
        {
            if (input.Right && input.Left)
                return;

            if (input.Right)
            {
                movement.Velocity.X = Math.Min(
                    movement.MaxSpeed,
                    movement.Velocity.X + (movement.Speed * frameDelta));
            }

            if (input.Left)
            {
                movement.Velocity.X = Math.Max(
                    -movement.MaxSpeed,
                    movement.Velocity.X - (movement.Speed * frameDelta));
            }
        }

        void UpdateTransform(ref Transform transform, in SideScrollingMovement movement)
        {
            if (movement.Velocity.X > 0.1 || movement.Velocity.X < -0.1)
                transform.Position.X += movement.Velocity.X;

            if (movement.Velocity.Y > 0.1 || movement.Velocity.Y < -0.1)
                transform.Position.Y += movement.Velocity.Y;
        }
    }
}
