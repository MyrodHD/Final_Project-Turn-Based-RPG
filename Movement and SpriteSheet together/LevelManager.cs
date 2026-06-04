using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class LevelManager
    {
        
        private readonly Vector2 _level1Spawn = new Vector2(175, 1135);

        public Rectangle GetLevelBounds(Game1.GameState level)
        {
            return level switch
            {
                Game1.GameState.Level1 => new Rectangle(0, 0, 1200, 1080),
                _ => Rectangle.Empty,
            };
        }

        public Vector2 GetSpawnPointForLevel(Game1.GameState level)
        {
            return level switch
            {
                Game1.GameState.Level1 => _level1Spawn,
                _ => Vector2.Zero,
            };
        }
    }
}
