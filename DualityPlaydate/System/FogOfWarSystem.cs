using System;
using System.Collections.Generic;
using System.Linq;
using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using DualityPlaydate.Utils;
using Microsoft.Xna.Framework;

namespace DualityPlaydate.System
{
    [With(typeof(Tile), typeof(DrawInfo))]
    class FogOfWarSystem : AEntitySetSystem<float>
    {
        Vector2 DrawLocation = Vector2.Zero;
        FollowCamera Camera;
        List<Entity> TilesToCheck = new();
        HashSet<Tile> TilesChecked = new();
        readonly int ScreenWidth;
        readonly int ScreenHeight;
        float PreviousZoom;

        public FogOfWarSystem(World _world, int screenWidth, int screenHeight)
            : base(_world)
        {
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
        }

        protected override void PreUpdate(float state)
        {
            Camera = World.GetEntities().With<FollowCamera>().AsEnumerable().FirstOrDefault().Get<FollowCamera>();

            TilesToCheck.Clear();
            TilesChecked.Clear();

            CameraUtils.SetDrawLocation(Camera, ref DrawLocation);

            if (Camera.Zoom != PreviousZoom)
            {
                PreviousZoom = Camera.Zoom;
            }
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var tile = ref entity.Get<Tile>();
            ref var transform = ref entity.Get<Transform>();
            ref var drawInfo = ref entity.Get<DrawInfo>();
            if (!CameraUtils.IsOnScreen(transform.Position - DrawLocation, drawInfo.SourceLocation.Width, drawInfo.SourceLocation.Height, in Camera))
                return;

            drawInfo.Visible = false;
            TilesToCheck.Add(entity);
        }

        protected override void PostUpdate(float state)
        {
            var center = new Point((int)Camera.Position.X, (int)Camera.Position.Y);
            var distance = 64 / Camera.Zoom;

            foreach (var entity in TilesToCheck)
            {
                ref var tile = ref entity.Get<Tile>();
                if (TilesChecked.Contains(tile))
                    continue;

                ref var trans = ref entity.Get<Transform>();

                var line = RayCastingUtils.BresenhamLine(new Point((int)trans.Position.X, (int)trans.Position.Y), center);

                foreach (var point in line)
                {
                    var entity = GetTileAtWorldCoord(point.X, point.Y);

                    if (!entity.Has<Tile>())
                        continue;

                    if (IsTileSolid(in entity.Get<Tile>()))
                        break;

                    ref var drawInfo = ref entity.Get<DrawInfo>();
                    drawInfo.Visible = true;
                }
            }
        }

        Entity GetTileAtWorldCoord(int x, int y)
        {
            return TilesToCheck.FirstOrDefault(e =>
            {
                var transform = e.Get<Transform>();
                return transform.Position.X < x &&
                transform.Position.X + 32 / Camera.Zoom > x &&
                transform.Position.Y < y &&
                transform.Position.Y + 32 / Camera.Zoom > y;
            });
        }

        static bool IsTileSolid(in Tile tile)
        {
            return tile.Type == TileType.Mountain;
        }
    }
}
