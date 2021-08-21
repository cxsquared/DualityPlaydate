using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace DualityPlaydate.System
{
    [With(typeof(Transform), typeof(DrawInfo))]
    public sealed class DrawSystem : AEntitySetSystem<SpriteBatch>
    {
        Vector2 DrawLocation = Vector2.Zero;

        public DrawSystem(World world)
            : base(world)
        {
        }

        protected override void PreUpdate(SpriteBatch batch)
        {
            // Probably do this at run time and trigger a changed only when the component changes
            var cameraEntity = World.GetEntities().With<FollowCamera>().AsEnumerable().FirstOrDefault();
            if (cameraEntity != null)
            {
                var camera = cameraEntity.Get<FollowCamera>();
                DrawLocation.X = MathHelper.Clamp(camera.Position.X - camera.Offset.X, camera.MinWorldX, camera.MaxWorldX);
                DrawLocation.Y = camera.Position.Y - camera.Offset.Y;
            }
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        protected override void Update(SpriteBatch batch, in Entity entity)
        {
            ref var trans = ref entity.Get<Transform>();
            ref var drawInfo = ref entity.Get<DrawInfo>();

            batch.Draw(drawInfo.Texture,
                trans.Position - DrawLocation,
                drawInfo.SourceLocation,
                drawInfo.Color,
                trans.Rotation,
                drawInfo.SourceOrigin,
                trans.Scale,
                drawInfo.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawInfo.ZIndex);
        }

        protected override void PostUpdate(SpriteBatch batch)
        {
            batch.End();
        }
    }
}
