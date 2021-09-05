using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;

namespace DualityPlaydate.System
{
    [With(typeof(DrawInfo), typeof(Tile), typeof(Transform))]
    class LineOfSightSystem : AEntitySetSystem<float>
    {
        List<Tile> TilesOnScreen = new();
        Transform Player;

        public LineOfSightSystem(World world, in Transform player)
            :base (world)
        {
            Player = player;
        }

        protected override void PreUpdate(float state)
        {
            TilesOnScreen.Clear();
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var transform = ref entity.Get<Transform>();
        }

        protected override void PostUpdate(float state)
        {
        }
    }
}
