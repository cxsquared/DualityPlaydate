using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.System;
using DualityPlaydate.Component;
using DualityPlaydate.System;
using DualityPlaydate.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;

namespace DualityPlaydate
{
    class DualityGame : Game
    {
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly World _world;
        private readonly TextureResourceManager _textureResourceManager;
        private readonly ISystem<SpriteBatch> _drawSystem;
        private readonly ISystem<float> _updateSystem;
        private ISystem<Entity> _fogSystem;

        private readonly Entity _map;
        private Entity[] _tiles;
        private Entity _tank;

        private Entity _snowman;
        private Entity _runner;

        private int _frameCount;
        private string _fps = string.Empty;
        private readonly Stopwatch _watch;
        private SpriteFont _font;

        private SpriteBatch _batch;
        private SpriteBatch Batch
        {
            get
            {
                return _batch;
            }
            set
            {
                if (_batch != null)
                    throw new ArgumentException("Can only set SpriteBatch once");

                _batch = value;
            }
        }

        public DualityGame()
        {
            _deviceManager = new GraphicsDeviceManager(this);
            IsFixedTimeStep = true;
            _deviceManager.GraphicsProfile = GraphicsProfile.HiDef;
            _deviceManager.IsFullScreen = false;
            _deviceManager.PreferredBackBufferWidth = 800;
            _deviceManager.PreferredBackBufferHeight = 600;
            _deviceManager.ApplyChanges();
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Content.RootDirectory = "Content";

            _world = new World();
            _textureResourceManager = new TextureResourceManager(GraphicsDevice, new TextureLoader(Content));
            _map = _world.CreateEntity();
            _map.Set(new Map
            {
                Width = 256,
                Height = 256,
            });

            _drawSystem = new SequentialSystem<SpriteBatch>(
                new FollowCameraSystem(_world),
                new DrawSystem(_world));

            //Create2dWorld();
            CreateTiles(_map.Get<Map>());

            _watch = Stopwatch.StartNew();

            _updateSystem = new SequentialSystem<float>(
                 new PlayerControlledSystem(_world),
                 new SideScrollingMovementSystem(_world),
                 new TankTopDownMovementSystem(_world),
                 new RunnerAnimationSystem(_world),
                 new AnimationSystem(_world));
        }

        protected override void Initialize()
        {
            //Initialize2dWorld();
            InitializeTiles(_map.Get<Map>());

            _fogSystem = new FogOfWarSystem(_world, _tiles, 256);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);

            _textureResourceManager.Manage(_world);

            _font = Content.Load<SpriteFont>("font");

            //Load2dWorldContent();
            LoadTilesContent();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _updateSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            _fogSystem.Update(_tank);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _drawSystem.Update(Batch);

            ++_frameCount;
            if (_watch.Elapsed.TotalSeconds > .5)
            {
                _fps = (_frameCount / _watch.Elapsed.TotalSeconds).ToString();
                _frameCount = 0;
                _watch.Restart();
            }

            _batch.Begin();
            _batch.DrawString(_font, _fps, new Vector2(10, 10), Color.Pink);
            _batch.End();

            base.Draw(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            _world?.Dispose();
            _drawSystem?.Dispose();
            Batch?.Dispose();

            base.Dispose(disposing);
        }

        void CreateTiles(Map map)
        {
            _tank = _world.CreateEntity();
            _tiles = new Entity[map.Height * map.Width];
            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = _world.CreateEntity();
            }
        }

        void InitializeTiles(Map map)
        {
            _tank.Set(new Transform(
                new Vector2(200, 200),
                0,
                1));
            _tank.Set(new DrawInfo(null, Color.White, new Rectangle(64, 32, 32, 32), new Vector2(16, 16), 0));
            _tank.Set(new PlayerControlled
            {
                PadIndex = PlayerIndex.One
            });
            _tank.Set(new Inputs());
            _tank.Set(new TankTopDownMovement
            {
                RotationRate = 1,
                MaxVelocity = 50
            });
            _tank.Set(new FollowCamera(_deviceManager.PreferredBackBufferWidth, _deviceManager.PreferredBackBufferHeight));

            var noise = NoiseUtils.GenerateMapNoice(map.Width, map.Height, 3, 0);
            for (var i = 0; i < _tiles.Length; i++)
            {
                ref var tile = ref _tiles[i];
                var type = TileTypeFromNoise(noise[i]);

                var mapX = i % map.Width;
                var mapY = i / map.Width;

                tile.Set(new Transform
                {
                    Position = new Vector2(mapX * 32, mapY * 32),
                    Scale = 1,
                    IsVisible = false
                });

                tile.Set(new Tile()
                {
                    MapX = i % map.Width,
                    MapY = i / map.Width,
                    Type = type
                });
            }
        }

        static TileType TileTypeFromNoise(float noise)
        {
            if (noise > 0.85)
                return TileType.Mountain;

            if (noise > 0.75)
                return TileType.Desert;

            if (noise > 0.6)
                return TileType.Forest;

            if (noise > 0.25)
                return TileType.Plain;

            return TileType.Ocean;
        }

        void LoadTilesContent()
        {
            _tank.Set(new ManagedResource<string, Texture2D>("tiledSprites2"));

            var planesEntity = _world.CreateEntity();
            planesEntity.Set(new DrawInfo(null, Color.White, new Rectangle(0, 0, 32, 32), new Vector2(0, 0), .1f));

            var forestEntity = _world.CreateEntity();
            forestEntity.Set(new DrawInfo(null, Color.White, new Rectangle(32, 0, 32, 32), new Vector2(0, 0), .1f));

            var desertEntity = _world.CreateEntity();
            desertEntity.Set(new DrawInfo(null, Color.White, new Rectangle(64, 0, 32, 32), new Vector2(0, 0), .1f));

            var mountainEntity = _world.CreateEntity();
            mountainEntity.Set(new DrawInfo(null, Color.White, new Rectangle(0, 32, 32, 32), new Vector2(0, 0), .1f));

            var oceanEntity = _world.CreateEntity();
            oceanEntity.Set(new DrawInfo(null, Color.White, new Rectangle(32, 32, 32, 32), new Vector2(0, 0), .1f));

            for (var i = 0; i < _tiles.Length; i++)
            {
                ref var tile = ref _tiles[i];
                ref var tileData = ref tile.Get<Tile>();

                switch (tileData.Type)
                {
                    case TileType.Plain:
                        tile.SetSameAs<DrawInfo>(planesEntity);
                        break;
                    case TileType.Forest:
                        tile.SetSameAs<DrawInfo>(forestEntity);
                        break;
                    case TileType.Desert:
                        tile.SetSameAs<DrawInfo>(desertEntity);
                        break;
                    case TileType.Mountain:
                        tile.SetSameAs<DrawInfo>(mountainEntity);
                        break;
                    case TileType.Ocean:
                        tile.SetSameAs<DrawInfo>(oceanEntity);
                        break;
                    default:
                        tile.SetSameAs<DrawInfo>(planesEntity);
                        break;
                }
                tile.Set(new ManagedResource<string, Texture2D>("tiledSprites2"));
            }
        }

        void Create2dWorld()
        {
            _snowman = _world.CreateEntity();
            _runner = _world.CreateEntity();
        }

        void Initialize2dWorld()
        {
            _snowman.Set(new Transform(
                new Vector2(500, 500),
                0,
                1));
            _snowman.Set(new DrawInfo(null, Color.Plum, new Rectangle(0, 128, 256, 256), new Vector2(128, 192), 0));

            _runner.Set(new Transform(
                new Vector2(200, 200),
                0,
                1));
            _runner.Set(new DrawInfo(null, Color.White, new Rectangle(0, 0, 128, 128), Vector2.Zero, 0));
            _runner.Set(new PlayerControlled
            {
                PadIndex = PlayerIndex.One
            });
            _runner.Set(new Inputs());
            _runner.Set(new SideScrollingMovement(10, 5, 0.9f));
            _runner.Set(new RunnerState());
            _runner.Set(new FollowCamera(_deviceManager.PreferredBackBufferWidth, _deviceManager.PreferredBackBufferHeight));
        }

        void Load2dWorldContent()
        {
            _snowman.Set(new ManagedResource<string, Texture2D>("snow_assets"));
            _runner.Set(new ManagedResource<string, Texture2D>("run_cycle"));
        }
    }
}
