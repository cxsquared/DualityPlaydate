using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using DualityPlaydate.Utils;
using System;

namespace DualityPlaydate.System
{
    [With(typeof(RunnerState), typeof(Movement))]
    class RunnerAnimationSystem : AEntitySetSystem<float>
    {
        Animation Idle = new()
        {
            Name = "idle",
            MsPerFrame = 0,
            Frames = new int[] { 0 }
        };

        Animation Running = new()
        {
            Name = "running",
            MsPerFrame = AnimationUtils.FpsToMs(12),
            Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
            Looped = true
        };

        public RunnerAnimationSystem(World world)
            : base(world)
        {
        }

        protected override void Update(float frameDelta, in Entity entity)
        {
            ref var state = ref entity.Get<RunnerState>();
            ref var movement = ref entity.Get<Movement>();

            if (movement.Velocity.X == 0 && state.IsMoving)
            {
                state.IsMoving = false;
                entity.Set(Idle);
                return;
            }

            if (Math.Abs(movement.Velocity.X) > 0)
            {
                if (!state.IsMoving)
                {
                    entity.Set(Running);
                    state.IsMoving = true;
                }

                ref var currentAnim = ref entity.Get<Animation>();
                currentAnim.Flip = movement.Velocity.X < 0;
                Idle.Flip = currentAnim.Flip;
            }
        }
    }
}
