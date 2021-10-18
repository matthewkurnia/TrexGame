using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TrexGame.Entities;

namespace TrexGame.System
{
    public class InputController
    {
        private Trex _trex;

        private bool _isBlocked = false;

        private KeyboardState _previousKeyboardState;

        public InputController(Trex trex)
        {
            _trex = trex;
        }

        public void ProcessControls(GameTime gameTime)
        {
            if (_isBlocked)
            {
                _previousKeyboardState = Keyboard.GetState();
                _isBlocked = false;
                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();
            if (!_previousKeyboardState.IsKeyDown(Keys.Up) && keyboardState.IsKeyDown(Keys.Up))
            {
                if (_trex.State != TrexState.Jumping) _trex.BeginJump();
            }
            else if (_previousKeyboardState.IsKeyDown(Keys.Up) && !keyboardState.IsKeyDown(Keys.Up))
            {
                if (_trex.State == TrexState.Jumping) _trex.CancelJump();
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                if (!_trex.Duck()) _trex.Drop(gameTime);
            }
            else
            {
                _trex.Stand();
            }

            _previousKeyboardState = keyboardState;
        }

        public void BlockInputFrame()
        {
            _isBlocked = true;
        }
    }
}
