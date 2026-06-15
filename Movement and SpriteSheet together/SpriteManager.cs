using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class SpriteManager
    {
        private Texture2D _texture;
        private int _rows, _columns;
        private int _currentFrame;
        private int _totalFrames;
        public int currentRow { get; set; } = 0;

        private double _timer;
        private double _frameTime = 0.2; // 200ms per frame

        public SpriteManager(Texture2D texture, int rows, int columns)
        {
            _texture = texture;
            _rows = rows;
            _columns = columns;
            _totalFrames = rows * columns;
        }

        public void Update(GameTime gameTime)
        {
            _timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer >= _frameTime)
            {
                _currentFrame = (_currentFrame + 1) % _totalFrames;
                _timer = 0;
            }

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            int width = _texture.Width / _columns;
            int height = _texture.Height / _rows;
            int column = _currentFrame % _columns;
            Rectangle sourceRectangle = new Rectangle(column * width, currentRow * height, width, height);
            spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);
        }

        public void Reset()
        {
            _currentFrame = 0;
        }
    }
}
