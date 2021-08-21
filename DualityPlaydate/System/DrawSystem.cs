using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DualityPlaydate.System
{
    [With(typeof(Transform), typeof(DrawInfo))]
    public sealed class DrawSystem : AEntitySetSystem<SpriteBatch>
    {
        public DrawSystem(World world)
            : base(world.GetEntities().With<Transform>().With<DrawInfo>().AsSet())
        {
        }

        protected override void PreUpdate(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        protected override void Update(SpriteBatch batch, ReadOnlySpan<Entity> entities)
        {
            foreach(var entity in entities)
            {
                ref var trans = ref entity.Get<Transform>();
                ref var drawInfo = ref entity.Get<DrawInfo>();

                batch.Draw(drawInfo.Texture,
                    trans.Position,
                    drawInfo.SourceLocation,
                    drawInfo.Color,
                    trans.Rotation,
                    drawInfo.SourceOrigin,
                    trans.Scale,
                    SpriteEffects.None,
                    drawInfo.ZIndex);
            }
        }

        protected override void PostUpdate(SpriteBatch batch)
        {
            batch.End();
        }
    }
}
