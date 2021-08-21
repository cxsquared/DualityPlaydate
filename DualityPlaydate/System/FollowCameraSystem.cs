using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;

namespace DualityPlaydate.System
{
    [With(typeof(FollowCamera), typeof(Transform))]
    class FollowCameraSystem : AEntitySetSystem<float>
    {
        public FollowCameraSystem(World world)
            : base(world)
        { }

        protected override void Update(float state, in Entity entity)
        {
            ref var camera = ref entity.Get<FollowCamera>();
            ref var transform = ref entity.Get<Transform>();

            if (camera.Position == transform.Position)
                return;

            camera.Position.Y = transform.Position.Y;

            if (camera.Position.X < transform.Position.X)
            {
                camera.Position.X -= ((camera.Position.X - transform.Position.X) * camera.FollowDampening);
                return;
            }

            camera.Position.X += ((camera.Position.X - transform.Position.X) * -camera.FollowDampening);
        }
    }
}
