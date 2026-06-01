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
                    list.Add(new Encounter(new Rectangle(300, 90, 48, 48), new Enemy("Goblin", 12, 3, 2)));
                    list.Add(new Encounter(new Rectangle(120, 360, 48, 48), new Enemy("Wolf", 15, 4, 4)));
                    list.Add(new Encounter(new Rectangle(510, 335, 48, 48), new Enemy("Bandit", 18, 5, 6)));

                    list.Add(new Encounter(new Rectangle(175, 245, 48, 48), new Enemy("Slime", 19, 3, 1)));
                    list.Add(new Encounter(new Rectangle(700, 80, 48, 48), new Enemy("Orc", 22, 6, 10)));
                    list.Add(new Encounter(new Rectangle(430, 340, 48, 48), new Enemy("Wraith", 20, 5, 3)));

                    list.Add(new Encounter(new Rectangle(550, 500, 48, 48), new Enemy("Skeleton", 18, 5, 8)));
                    list.Add(new Encounter(new Rectangle(375, 220, 48, 48), new Enemy("Troll", 30, 7, 15)));
                    list.Add(new Encounter(new Rectangle(400, 480, 48, 48), new Enemy("Vampire", 25, 6, 14)));

                    break;
            }

            return list;
        }
    }
}
