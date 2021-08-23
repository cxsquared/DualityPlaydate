using DefaultEcs;
using DefaultEcs.System;
using DualityPlaydate.Component;
using Microsoft.Xna.Framework.Input;

namespace DualityPlaydate.System
{
    [With(typeof(PlayerControlled), typeof(Inputs))]
    class PlayerControlledSystem : AEntitySetSystem<float>
    {
        KeyboardState currentKeyState;

        public PlayerControlledSystem(World world)
            : base(world)
        {
        }

        protected override void PreUpdate(float state)
        {
            currentKeyState = Keyboard.GetState();
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var playerControlled = ref entity.Get<PlayerControlled>();
            ref var inputs = ref entity.Get<Inputs>();

            var currentPadState = GamePad.GetState(playerControlled.PadIndex);

            inputs.Left = IsLeftPressed(currentKeyState, currentPadState);
            inputs.Right = IsRightPressed(currentKeyState, currentPadState);
            inputs.Up = IsUpPressed(currentKeyState, currentPadState);
            inputs.Down = IsDownPressed(currentKeyState, currentPadState);
        }

        static bool IsLeftPressed(KeyboardState keyState, GamePadState gamePadState)
        {
            var keysPressed = keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left);
            var padPressed = false;
            if (gamePadState.IsConnected)
            {
                padPressed = gamePadState.IsButtonDown(Buttons.DPadLeft) || gamePadState.IsButtonDown(Buttons.RightThumbstickLeft);
            }

            return keysPressed || padPressed;
        }

        static bool IsRightPressed(KeyboardState keyState, GamePadState gamePadState)
        {
            var keysPressed = keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right);
            var padPressed = false;
            if (gamePadState.IsConnected)
            {
                padPressed = gamePadState.IsButtonDown(Buttons.DPadRight) || gamePadState.IsButtonDown(Buttons.RightThumbstickRight);
            }

            return keysPressed || padPressed;
        }

        static bool IsUpPressed(KeyboardState keyState, GamePadState gamePadState)
        {
            var keysPressed = keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up);
            var padPressed = false;
            if (gamePadState.IsConnected)
            {
                padPressed = gamePadState.IsButtonDown(Buttons.DPadUp) || gamePadState.IsButtonDown(Buttons.RightThumbstickUp);
            }

            return keysPressed || padPressed;
        }

        static bool IsDownPressed(KeyboardState keyState, GamePadState gamePadState)
        {
            var keysPressed = keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down);
            var padPressed = false;
            if (gamePadState.IsConnected)
            {
                padPressed = gamePadState.IsButtonDown(Buttons.DPadDown) || gamePadState.IsButtonDown(Buttons.RightThumbstickDown);
            }

            return keysPressed || padPressed;
        }
    }
}
