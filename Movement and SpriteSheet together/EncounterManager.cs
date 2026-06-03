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
                    list.Add(new Encounter(new Rectangle(185, 65, 36, 36), new Enemy("Goblin", 12, 3, 2)));
                    list.Add(new Encounter(new Rectangle(95, 630, 36, 36), new Enemy("Wolf", 15, 4, 4)));
                    list.Add(new Encounter(new Rectangle(385, 65, 36, 36), new Enemy("Bandit", 18, 4, 6)));

                    list.Add(new Encounter(new Rectangle(590, 95, 36, 36), new Enemy("Slime", 19, 3, 1)));
                    list.Add(new Encounter(new Rectangle(1055, 345, 35, 36), new Enemy("Orc", 22, 5, 10)));
                    list.Add(new Encounter(new Rectangle(965, 1045, 36, 36), new Enemy("Wraith", 20, 5, 3)));

                    list.Add(new Encounter(new Rectangle(1030, 105, 36, 36), new Enemy("Skeleton", 18, 5, 8)));
                    list.Add(new Encounter(new Rectangle(105, 375, 36, 36), new Enemy("Troll", 30, 6, 15)));
                    list.Add(new Encounter(new Rectangle(600, 950, 36, 36), new Enemy("Vampire", 25, 6, 14)));

                    break;
            }

            return list;
        }
    }
}
