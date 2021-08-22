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
        FollowCamera Camera;

        public DrawSystem(World world)
            : base(world)
        {
        }

        protected override void PreUpdate(SpriteBatch batch)
        {
            // Probably do this at run time and trigger a changed only when the component changes
            var cameraEntity = World.GetEntities().With<FollowCamera>().AsEnumerable().FirstOrDefault();
            var zoomLevel = 1.0f;
            if (cameraEntity.Has<FollowCamera>())
            {
                Camera = cameraEntity.Get<FollowCamera>();
                DrawLocation.X = MathHelper.Clamp(Camera.Position.X - (Camera.Offset.X / Camera.Zoom), Camera.WorldBounds.X, Camera.WorldBounds.Width);
                DrawLocation.Y = MathHelper.Clamp(Camera.Position.Y - (Camera.Offset.Y / Camera.Zoom), Camera.WorldBounds.Y, Camera.WorldBounds.Height);
                zoomLevel = Camera.Zoom;
            }
            batch.Begin(SpriteSortMode.BackToFront,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone,
                null,
                Matrix.CreateScale(zoomLevel));
        }

        protected override void Update(SpriteBatch batch, in Entity entity)
        {
            ref var trans = ref entity.Get<Transform>();
            ref var drawInfo = ref entity.Get<DrawInfo>();

            var targetDrawLocation = trans.Position - DrawLocation;

            if (targetDrawLocation.X < -Camera.Offset.X / Camera.Zoom / 2 || targetDrawLocation.X > Camera.Offset.X / Camera.Zoom * 2.5)
                return;

            if (targetDrawLocation.Y < -Camera.Offset.Y / Camera.Zoom / 2 || targetDrawLocation.Y > Camera.Offset.Y / Camera.Zoom * 2.5)
                return;

            batch.Draw(drawInfo.Texture,
                targetDrawLocation,
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
