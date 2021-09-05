using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using DualityPlaydate.Utils;
using Microsoft.Xna.Framework;

namespace DualityPlaydate.System
{
    [With(typeof(Tile), typeof(DrawInfo))]
    class FogOfWarSystem : AEntitySetSystem<Entity>
    {
        public int Distance { get; set; } = 5;

        bool playerTileSet = false;
        Entity CurrentPlayerTile;
        int MapSize;

        readonly Entity[] Tiles;
        Transform Player;
        FollowCamera Camera;

        public FogOfWarSystem(World _world, in Entity[] tiles, int mapSize)
            : base(_world)
        {
            Tiles = tiles;
            MapSize = mapSize;
        }

        protected override void PreUpdate(Entity state)
        {
            playerTileSet = false;
            Player = state.Get<Transform>();
            Camera = state.Get<FollowCamera>();
        }

        protected override void Update(Entity state, in Entity entity)
        {
            ref var transform = ref entity.Get<Transform>();
            ref var drawInfo = ref entity.Get<DrawInfo>();

            if (!CameraUtils.IsOnScreen(transform.Position, drawInfo.SourceLocation.Width, drawInfo.SourceLocation.Height, Camera))
                return;

            if (!playerTileSet
                && IsInside(new Vector2(Player.Position.X, Player.Position.Y), transform, drawInfo))
            {
                CurrentPlayerTile = entity;
                transform.IsVisible = true;
                playerTileSet = true;
            }
        }

        protected override void PostUpdate(Entity state)
        {
            var startingTile = CurrentPlayerTile.Get<Tile>();
            var centerX = startingTile.MapX;
            var centerY = startingTile.MapY;

            for(int y = -Distance; y < Distance; y++)
                for(int x = -Distance; x < Distance; x++)
                {
                    var index = (centerY + y) * MapSize + (centerX + x);
                    if (index >= Tiles.Length || index < 0)
                        continue;

                    var tile = Tiles[index];
                    ref var t = ref tile.Get<Transform>();
                    ref var di = ref tile.Get<DrawInfo>();
                    t.IsVisible = true;
                }

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
