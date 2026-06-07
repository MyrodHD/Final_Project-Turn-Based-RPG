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

        private float _respawnTimer = 0f;

        public Encounter(Rectangle hitbox, Enemy enemy)
        {
            Hitbox = hitbox;
            Enemy = enemy;
        }
        public void StartRespawn(float seconds)
        {
            _respawnTimer = Math.Max(0f, seconds);
            Active = false;
        }

        // Call from Game1.Update to advance the timer.
        // Returns true when the timer just reached zero (i.e. ready to reactivate).
        public bool UpdateRespawn(float dt)
        {
            if (_respawnTimer <= 0f)
                return false;

            _respawnTimer -= dt;
            if (_respawnTimer <= 0f)
            {
                _respawnTimer = 0f;
                // Do not automatically set Active true here — Game1 will check player overlap first.
                return true;
            }

            return false;
        }

        // Expose whether an encounter is currently waiting to respawn.
        public bool IsAwaitingRespawn => _respawnTimer > 0f;
    }
}
