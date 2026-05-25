using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class LevelManager
    {
        // Configure transition zones and spawn points here
        private readonly Rectangle _level1To2Zone = new Rectangle(750, 0, 10, 600);
        private readonly Rectangle _level2To3Zone = new Rectangle(750, 0, 10, 600);

        private readonly Vector2 _level1Spawn = new Vector2(100, 100);
        private readonly Vector2 _level2Spawn = new Vector2(50, 50);
        private readonly Vector2 _level3Spawn = new Vector2(100, 50);

        public bool TryGetTransition(Game1.GameState currentLevel, Rectangle playerRect, out Game1.GameState newLevel, out Vector2 spawnPoint)
        {
            newLevel = currentLevel;
            spawnPoint = Vector2.Zero;

            switch (currentLevel)
            {
                case Game1.GameState.Level1:
                    if (playerRect.Intersects(_level1To2Zone))
                    {
                        newLevel = Game1.GameState.Level2;
                        spawnPoint = _level2Spawn;
                        return true;
                    }
                    break;

                case Game1.GameState.Level2:
                    if (playerRect.Intersects(_level2To3Zone))
                    {
                        newLevel = Game1.GameState.Level1;
                        spawnPoint = _level3Spawn;
                        return true;
                    }
                    break;
            }

            return false;
        }

        public Vector2 GetSpawnPointForLevel(Game1.GameState level)
        {
            return level switch
            {
                Game1.GameState.Level1 => _level1Spawn,
                Game1.GameState.Level2 => _level2Spawn,
                _ => Vector2.Zero,
            };
        }
    }
}
