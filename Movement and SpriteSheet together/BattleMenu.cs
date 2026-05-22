using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class BattleMenu
    {
        private SpriteFont _font;
        private List<string> _menuItems;
        private Vector2 _position;
        private int _spacing;

        private int _selectedIndex = 0;
        private KeyboardState _previousKeyboard;
        private MouseState _previousMouse;

        private Rectangle[] _itemRectangles;

        public bool IsActivated { get; private set; } = false;
        public int SelectedIndex => _selectedIndex;
        public string SelectedItem => _menuItems[_selectedIndex];

        public BattleMenu(SpriteFont font, List<string> menuItems, Vector2 position, int spacing = 40)
        {
            _font = font;
            _menuItems = menuItems;
            _position = position;
            _spacing = spacing;
            _itemRectangles = new Rectangle[_menuItems.Count];
        }

        public void Update(GameTime gameTime)
        {
            var keyboard = Keyboard.GetState();
            var mouse = Mouse.GetState();

            // keyboard navigation (detect key presses)
            if (keyboard.IsKeyDown(Keys.Down) && _previousKeyboard.IsKeyUp(Keys.Down))
            {
                _selectedIndex++;
                if (_selectedIndex >= _menuItems.Count)
                    _selectedIndex = 0;
            }

            if (keyboard.IsKeyDown(Keys.Up) && _previousKeyboard.IsKeyUp(Keys.Up))
            {
                _selectedIndex--;
                if (_selectedIndex < 0)
                    _selectedIndex = _menuItems.Count - 1;
            }

            // compute rectangles for each item (width based on measured string)
            Vector2 pos = _position;
            for (int i = 0; i < _menuItems.Count; i++)
            {
                Vector2 size = _font.MeasureString(_menuItems[i]);
                _itemRectangles[i] = new Rectangle((int)pos.X, (int)pos.Y - (int)(size.Y / 2), (int)size.X + 10, (int)size.Y + 10);
                pos.Y += _spacing;
            }

            // mouse hover
            for (int i = 0; i < _itemRectangles.Length; i++)
            {
                if (_itemRectangles[i].Contains(mouse.Position))
                {
                    _selectedIndex = i;
                    break;
                }
            }

            // activation by Enter or mouse click
            if (keyboard.IsKeyDown(Keys.Enter) && _previousKeyboard.IsKeyUp(Keys.Enter))
            {
                IsActivated = true;
            }

            if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            {
                // If clicked inside the selected rect (or any rect), activate and ensure selected index matches clicked item
                for (int i = 0; i < _itemRectangles.Length; i++)
                {
                    if (_itemRectangles[i].Contains(mouse.Position))
                    {
                        _selectedIndex = i;
                        IsActivated = true;
                        break;
                    }
                }
            }

            _previousKeyboard = keyboard;
            _previousMouse = mouse;
        }

        public void Draw(SpriteBatch spriteBatch, Color normalColor, Color selectedColor)
        {
            Vector2 drawPos = _position;
            for (int i = 0; i < _menuItems.Count; i++)
            {
                Color color = (i == _selectedIndex) ? selectedColor : normalColor;
                spriteBatch.DrawString(_font, _menuItems[i], drawPos, color);
                drawPos.Y += _spacing;
            }
        }

        // Returns the selected index if a selection was made since the last consume call.
        // Caller should perform the action (e.g. call BattleSystem.HeroAttack()) and then ignore until next activation.
        public bool ConsumeSelection(out int index)
        {
            if (IsActivated)
            {
                index = _selectedIndex;
                IsActivated = false;
                return true;
            }

            index = -1;
            return false;
        }
    }
}
