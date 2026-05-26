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
                    list.Add(new Encounter(new Rectangle(345, 90, 48, 48), new Enemy("Goblin", 12, 3, 2)));
                    list.Add(new Encounter(new Rectangle(120, 360, 56, 56), new Enemy("Wolf", 15, 4, 4)));
                    list.Add(new Encounter(new Rectangle(510, 335, 48, 48), new Enemy("Bandit", 18, 5, 6)));
                    break;

                case Game1.GameState.Level2:
                    list.Add(new Encounter(new Rectangle(150, 120, 48, 48), new Enemy("Slime", 15, 3, 1)));
                    list.Add(new Encounter(new Rectangle(300, 220, 56, 56), new Enemy("Orc", 22, 6, 10)));
                    list.Add(new Encounter(new Rectangle(420, 340, 48, 48), new Enemy("Wraith", 18, 5, 3)));
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
