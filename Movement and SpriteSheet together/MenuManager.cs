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
    public class MenuManager
    {
        private int _selectedIndex = 0;
        private SpriteFont _font;
        private List<string> _menuItems;
        private KeyboardState _previousState;

        public MenuManager(SpriteFont font, List<string> menuItems)
        {
            _font = font;
            _menuItems = menuItems;
        }

        public void Update(GameTime gameTime, ref Game1.GameState currentState)
        {
            KeyboardState _currentState = Keyboard.GetState();

            if (_currentState.IsKeyDown(Keys.Down) && _previousState.IsKeyUp(Keys.Down))
            {
                _selectedIndex++;
                if (_selectedIndex >= _menuItems.Count)
                    _selectedIndex = 0;
            }

            if (_currentState.IsKeyDown(Keys.Up) && _previousState.IsKeyUp(Keys.Up))
            {
                _selectedIndex--;
                if (_selectedIndex < 0)
                    _selectedIndex = _menuItems.Count - 1;
            }

            if (_currentState.IsKeyDown(Keys.Enter) && _previousState.IsKeyUp(Keys.Enter))
            {
                if (_selectedIndex == 0)
                    currentState = Game1.GameState.Level1;
                else if (_selectedIndex == 1)
                    currentState = Game1.GameState.Controls;
            }
            _previousState = _currentState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _menuItems.Count; i++)
            {
                Color color = (i == _selectedIndex) ? Color.Yellow : Color.White;
                spriteBatch.DrawString(_font, _menuItems[i], new Vector2(55, 215 + i * 50), color);
            }
        }
    }
}
