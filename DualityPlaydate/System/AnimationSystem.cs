using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;

namespace DualityPlaydate.System
{
    [With(typeof(Animation), typeof(DrawInfo))]
    class AnimationSystem : AEntitySetSystem<float>
    {
        public AnimationSystem(World world)
            : base(world)
        { }

        protected override void Update(float frameDelta, in Entity entity)
        {
            ref var animation = ref entity.Get<Animation>();
            ref var drawInfo = ref entity.Get<DrawInfo>();

            animation.AnimationTime += frameDelta * 1000; // frameDelta is in Seconds

            if (animation.AnimationTime >= animation.MsPerFrame)
            {
                animation.AnimationTime -= animation.MsPerFrame;
                animation.CurrentFrame++;

                if (!animation.Looped && animation.CurrentFrame >= animation.Frames.Length)
                {
                    entity.Remove<Animation>();
                }

                animation.CurrentFrame %= animation.Frames.Length;

                drawInfo.SourceLocation.X = drawInfo.SourceLocation.Width * animation.Frames[animation.CurrentFrame];
            }

            drawInfo.Flip = animation.Flip;
        }
    }
}
