using System.Collections.Generic;
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
        List<Entity> VisibleTiles = new();
        bool playerTileSet = false;
        Entity CurrentPlayerTile;
        bool firstRun = true; // this is bad

        readonly Transform Player;
        readonly FollowCamera Camera;
        readonly Map Map;

        public FogOfWarSystem(World _world, in Transform player, in FollowCamera camera, in Map map)
            : base(_world)
        {
            Player = player;
            Camera = camera;
            Map = map;
        }

        protected override void PreUpdate(float state)
        {
            VisibleTiles.Clear();
            playerTileSet = false;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var transform = ref entity.Get<Transform>();
            ref var drawInfo = ref entity.Get<DrawInfo>();

            if (!CameraUtils.IsOnScreen(transform.Position, drawInfo.SourceLocation.Width, drawInfo.SourceLocation.Height, in Camera))
                return;

            VisibleTiles.Add(entity);

            if (!playerTileSet
                && IsInside(new Vector2(Player.Position.X + 16, Player.Position.Y + 16), transform, drawInfo))
            {
                CurrentPlayerTile = entity;
                drawInfo.Visible = true;
                playerTileSet = true;
            }
        }

        protected override void PostUpdate(float state)
        {
            firstRun = false;

            var center = Player.Position; // TODO: Make this use player width/height offset
            /*
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
            */
        }

        bool IsInside(in Vector2 point,
            in Transform bTransform, in DrawInfo bDrawInfo)
        {
            // This ignores scale
            var bBounds = new Rectangle((int)bTransform.Position.X, (int)bTransform.Position.Y,
                bDrawInfo.SourceLocation.Width, bDrawInfo.SourceLocation.Height);

            return bBounds.Contains((int)point.X, (int)point.Y);
        }
    }
}
