using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class ParticleSystem
    {
        private List<Particle> _particles = new List<Particle>();
        private Texture2D _texture;
        private Random _rng = new Random();

        public ParticleSystem(Texture2D texture)
        {
            _texture = texture;
        }

        public void CreateDustBurst(Vector2 position)
        {
            for (int i = 0; i< 5; i++)
            {
                _particles.Add(new Particle
                {
                    _position = position,
                    _velocity = new Vector2((float)_rng.NextDouble() * 2 -1, (float)_rng.NextDouble() * -1.5f),
                    _lifespan = 0.5f,
                    _currentLife = 0,
                    _scale = (float)_rng.NextDouble() * 0.7f + 0.1f,
                    _color = Color.LightGray * 0.15f
                });
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                _particles[i].Update(gameTime);
                if (_particles[i]._isExpired)
                    _particles.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in _particles)
            {
                float alpha = 1.0f - (particle._currentLife / particle._lifespan);
                spriteBatch.Draw(_texture, particle._position, null, particle._color * alpha, 0f, Vector2.Zero, particle._scale, SpriteEffects.None, 0f);
            }
        }
    }
}
