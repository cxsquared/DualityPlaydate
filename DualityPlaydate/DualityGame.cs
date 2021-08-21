﻿using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.System;
using DualityPlaydate.Component;
using DualityPlaydate.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DualityPlaydate
{
    class DualityGame : Game
    {
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly World _world;
        private readonly TextureResourceManager _textureResourceManager;
        private readonly Entity _snowman;
        private readonly Entity _runner;
        private readonly ISystem<SpriteBatch> _drawSystem;
        private readonly ISystem<float> _updateSystem;

        private SpriteBatch _batch;
        private SpriteBatch Batch
        {
            get {
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
            _snowman = _world.CreateEntity();
            _runner = _world.CreateEntity();

            _drawSystem = new SequentialSystem<SpriteBatch>(
                new DrawSystem(_world));

            _updateSystem = new SequentialSystem<float>(
                new PlayerControlledSystem(_world),
                new MovementSystem(_world));
        }

        protected override void Initialize()
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
            _runner.Set(new PlayerControlled{
                PadIndex = PlayerIndex.One
                });
            _runner.Set(new Inputs());
            _runner.Set(new Movement(10, 5, 0.9f));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Batch = new SpriteBatch(GraphicsDevice);

            _textureResourceManager.Manage(_world);

            _snowman.Set(new ManagedResource<string, Texture2D>("snow_assets"));
            _runner.Set(new ManagedResource<string, Texture2D>("run_cycle"));

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
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
    }
}