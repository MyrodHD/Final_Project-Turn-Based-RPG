using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class EncounterManager
    {
        public List<Encounter> GetEncountersForLevel(Game1.GameState level)
        {
            var list = new List<Encounter>();

            switch (level)
            {
                case Game1.GameState.Level1:
                    list.Add(new Encounter(new Rectangle(220, 180, 48, 48), new Enemy("Goblin", 16, 3, 5)));
                    list.Add(new Encounter(new Rectangle(420, 260, 56, 56), new Enemy("Wolf", 18, 3, 6)));
                    list.Add(new Encounter(new Rectangle(80, 360, 48, 48), new Enemy("Bandit", 20, 4, 8)));
                    break;

                case Game1.GameState.Level2:
                    list.Add(new Encounter(new Rectangle(150, 120, 48, 48), new Enemy("Slime", 10, 2, 3)));
                    list.Add(new Encounter(new Rectangle(300, 220, 56, 56), new Enemy("Orc", 24, 5, 10)));
                    list.Add(new Encounter(new Rectangle(420, 340, 48, 48), new Enemy("Wraith", 20, 6, 12)));
                    break;

                case Game1.GameState.Level3:
                    list.Add(new Encounter(new Rectangle(200, 150, 48, 48), new Enemy("Skeleton", 18, 4, 8)));
                    list.Add(new Encounter(new Rectangle(350, 250, 56, 56), new Enemy("Troll", 30, 7, 15)));
                    list.Add(new Encounter(new Rectangle(500, 400, 48, 48), new Enemy("Vampire", 25, 6, 14)));
                    break;

                default:
                    break;
            }

            return list;
        }
    }
}
