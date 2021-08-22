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

namespace DualityPlaydate
{
    class DualityGame : Game
    {
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly World _world;
        private readonly TextureResourceManager _textureResourceManager;
        private readonly ISystem<SpriteBatch> _drawSystem;
        private readonly ISystem<float> _updateSystem;

        private readonly Entity _map;
        private Entity[] _tiles;

        private Entity _snowman;
        private Entity _runner;

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
                Width = 64,
                Height = 64,
            });

            _drawSystem = new SequentialSystem<SpriteBatch>(
                new FollowCameraSystem(_world),
                new DrawSystem(_world));

            _updateSystem = new SequentialSystem<float>(
                new PlayerControlledSystem(_world),
                new SideScrollingMovementSystem(_world),
                new RunnerAnimationSystem(_world),
                new AnimationSystem(_world));

            //Create2dWorld();
            CreateTiles(_map.Get<Map>());
        }

        protected override void Initialize()
        {
            //Initialize2dWorld();
            InitializeTiles(_map.Get<Map>());

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);

            _textureResourceManager.Manage(_world);

            //Load2dWorldContent();
            LoadTilesContent();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _updateSystem.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _drawSystem.Update(Batch);

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
            _tiles = new Entity[map.Height * map.Width];
            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = _world.CreateEntity();
            }
        }

        void InitializeTiles(Map map)
        {
            var noise = NoiseUtils.GenerateMapNoice(map.Width, map.Height, 0);
            for (var i = 0; i < _tiles.Length; i++)
            {
                ref var tile = ref _tiles[i];
                var type = TileTypeFromNoise(noise[i]);

                var mapX = i % map.Width;
                var mapY = i / map.Width;

                tile.Set(new Transform
                {
                    Position = new Vector2(mapX * 32, mapY * 32),
                    Scale = 1
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
            if (noise > 0.9)
                return TileType.Ocean;

            if (noise > 0.8)
                return TileType.Mountain;

            if (noise > 0.7)
                return TileType.Desert;

            if (noise > 0.4)
                return TileType.Forest;

            return TileType.Plain;
        }

        void LoadTilesContent()
        {
            for (var i = 0; i < _tiles.Length; i++)
            {
                ref var tile = ref _tiles[i];
                ref var tileData = ref tile.Get<Tile>();

                tile.Set(new DrawInfo(null, Color.White, TileTextureLocationFromType(tileData.Type), new Vector2(0, 0)));
                tile.Set(new ManagedResource<string, Texture2D>("tiledSprites2"));
            }
        }

        readonly Rectangle PlainsRect = new Rectangle(0, 0, 32, 32);
        readonly Rectangle ForestRect = new Rectangle(32, 0, 32, 32);
        readonly Rectangle DesertRect = new Rectangle(64, 0, 32, 32);
        readonly Rectangle MountainRect = new Rectangle(0, 32, 32, 32);
        readonly Rectangle OceanRect = new Rectangle(32, 32, 32, 32);
        Rectangle TileTextureLocationFromType(TileType type)
        {
            return type switch
            {
                (TileType.Plain) => PlainsRect,
                (TileType.Forest) => ForestRect,
                (TileType.Desert) => DesertRect,
                (TileType.Mountain) => MountainRect,
                (TileType.Ocean) => OceanRect,
                _ => PlainsRect,
            };
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
            _snowman.Set(new DrawInfo(null, Color.Plum, new Rectangle(0, 128, 256, 256), new Vector2(128, 192)));

            _runner.Set(new Transform(
                new Vector2(200, 200),
                0,
                1));
            _runner.Set(new DrawInfo(null, Color.White, new Rectangle(0, 0, 128, 128), Vector2.Zero));
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
