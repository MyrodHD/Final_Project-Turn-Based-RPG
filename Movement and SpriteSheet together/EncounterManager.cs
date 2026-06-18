using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class EncounterManager
    {
        public List<Encounter> GetEncountersForLevel(Game1.GameState level, int playerLevel = 1)
        {
            var list = new List<Encounter>();

            switch (level)
            {
                case Game1.GameState.Level1:
                    list.Add(new Encounter(new Rectangle(185, 65, 36, 36), new Enemy("Snowman", 13, 3, 2)));
                    list.Add(new Encounter(new Rectangle(95, 630, 36, 36), new Enemy("Mushroom", 15, 4, 4)));
                    list.Add(new Encounter(new Rectangle(385, 65, 36, 36), new Enemy("Piggy", 16, 4, 4)));

                    list.Add(new Encounter(new Rectangle(590, 95, 36, 36), new Enemy("Bee", 19, 3, 1)));
                    list.Add(new Encounter(new Rectangle(1055, 345, 35, 36), new Enemy("Boar", 21, 5, 6)));

                    list.Add(new Encounter(new Rectangle(1030, 105, 36, 36), new Enemy("Skeleton", 19, 5, 7)));
                    list.Add(new Encounter(new Rectangle(105, 382, 36, 36), new Enemy("Witch Doctor", 29, 6, 7)));

                    break;
            }

            if (playerLevel > 1 && list.Count > 0)
            {
                float multiplier = 1f + (playerLevel - 1) * 0.20f; // 20% stronger per level above 1

                for (int i = 0; i < list.Count; i++)
                {
                    var enc = list[i];
                    var baseEnemy = enc.Enemy;

                    int scaledHP = System.Math.Max(1, (int)System.MathF.Ceiling(baseEnemy.HP * multiplier));
                    int scaledAttack = System.Math.Max(1, (int)System.MathF.Ceiling(baseEnemy.AttackPower * multiplier));
                    int scaledXP = System.Math.Max(1, (int)System.MathF.Ceiling(baseEnemy.XPValue * multiplier));

                    // Create a new Enemy instance with scaled stats (Encounter.Active defaults remain true).
                    list[i] = new Encounter(enc.Hitbox, new Enemy(baseEnemy.Name, scaledHP, scaledAttack, scaledXP));
                }
            }


            return list;
        }
    }
}
