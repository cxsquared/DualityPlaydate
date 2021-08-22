using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DualityPlaydate.System
{
    [With(typeof(FollowCamera), typeof(Transform))]
    class FollowCameraSystem : AEntitySetSystem<SpriteBatch>
    {
        public FollowCameraSystem(World world)
            : base(world)
        { }

        protected override void Update(SpriteBatch state, in Entity entity)
        {
            ref var camera = ref entity.Get<FollowCamera>();
            ref var transform = ref entity.Get<Transform>();

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                camera.Zoom += 0.01f;
            }
            else if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                camera.Zoom -= 0.01f;
            }

            if (camera.Position == transform.Position)
                return;

            if (camera.Position.Y < transform.Position.Y)
            {
                camera.Position.Y -= ((camera.Position.Y - transform.Position.Y) * camera.FollowDampening);
            }
            else
            {
                camera.Position.Y += ((camera.Position.Y - transform.Position.Y) * -camera.FollowDampening);
            }

            if (camera.Position.X < transform.Position.X)
            {
                camera.Position.X -= ((camera.Position.X - transform.Position.X) * camera.FollowDampening);
            }
            else
            {
                camera.Position.X += ((camera.Position.X - transform.Position.X) * -camera.FollowDampening);
            }

        }
    }
}
