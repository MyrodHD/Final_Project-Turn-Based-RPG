using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class Encounter
    {
        public Rectangle Hitbox;
        public Enemy Enemy;
        public bool Active = true;

        public Encounter(Rectangle hitbox, Enemy enemy)
        {
            Hitbox = hitbox;
            Enemy = enemy;
        }

    }
}
