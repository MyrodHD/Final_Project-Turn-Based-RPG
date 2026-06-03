using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class MovementManager
    {
        public Vector2 position { get; private set; }
        private float _speed = 150f;
        public Vector2 currentDirection { get; private set; }

        private ParticleSystem _particleSystem;
        private float _dustTimer;
        private const float dustInterval = 0.15f;

        public MovementManager(Vector2 startPosition, ParticleSystem particleSystem)
        {
            position = startPosition;
            _particleSystem = particleSystem;
        }

        public void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.W))
                direction.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S))
                direction.Y += 1;
            if (keyboardState.IsKeyDown(Keys.A))
                direction.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D))
                direction.X += 1;

            // Prevents faster diagonal movement
            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                _dustTimer -= dt;
                if (_dustTimer <= 0)
                {
                    Vector2 feetOffset = new Vector2(16, 34);
                    _particleSystem.CreateDustBurst(position + feetOffset);

                    _dustTimer = dustInterval;
                }
            }
            else
            {
                 _dustTimer = 0; // Reset timer when not moving
            }
                currentDirection = direction;

            // _speed * dt ensures consistent movement regardless of frame rate
            position += direction * _speed * dt;
        }
    
    }
}
