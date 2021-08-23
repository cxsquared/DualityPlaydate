using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using System;

namespace DualityPlaydate.System
{
    [With(typeof(TankTopDownMovement), typeof(Transform), typeof(Inputs))]
    class TankTopDownMovementSystem : AEntitySetSystem<float>
    {
        public TankTopDownMovementSystem(World _world)
            : base(_world)
        {
        }

        protected override void Update(float frameDelta, in Entity entity)
        {
            ref var input = ref entity.Get<Inputs>();
            ref var movement = ref entity.Get<TankTopDownMovement>();
            ref var trans = ref entity.Get<Transform>();

            if (HasMovementInput(in input))
            {
                if (input.Up)
                {
                    ApplyVelocity(ref movement, frameDelta);
                }

                ApplyRotation(ref movement, in input, frameDelta);

            }
            else
            {
                ApplyFriction(ref movement, frameDelta);
            }

            UpdateTransform(ref trans, in movement, frameDelta);
        }

        static bool HasMovementInput(in Inputs input)
        {
            return input.Right || input.Left || input.Up;
        }

        static void ApplyFriction(ref TankTopDownMovement movement, float frameDelta)
        {
            movement.ForwardVelocity *= movement.Friction * frameDelta;
        }

        static void ApplyVelocity(ref TankTopDownMovement movement, float frameDelta)
        {
            if (movement.ForwardVelocity < movement.MaxVelocity)
                movement.ForwardVelocity += 5f;
        }

        static void ApplyRotation(ref TankTopDownMovement movement, in Inputs input, float frameDelta)
        {
            if (input.Left)
                movement.Rotation -= (movement.RotationRate * frameDelta);

            if (input.Right)
                movement.Rotation += (movement.RotationRate * frameDelta);
        }

        void UpdateTransform(ref Transform transform, in TankTopDownMovement movement, float frameDelta)
        {
            transform.Position.X += (float)(movement.ForwardVelocity * Math.Cos(movement.Rotation) * frameDelta);
            transform.Position.Y += (float)(movement.ForwardVelocity * Math.Sin(movement.Rotation) * frameDelta);
            transform.Rotation = movement.Rotation;
        }

    }
}
