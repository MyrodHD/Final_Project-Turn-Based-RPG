using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class Hero
    {
        private string _name;
        private int _HP;
        private int _maxHP;
        private int _attackPower;

        private XPSystem _xpSystem;

        private Texture2D _heroTexture;
        private Rectangle _heroRect;

        public string Name => _name;
        public int HP => _HP;
        public int MaxHP => _maxHP;
        public int AttackPower => _attackPower;
        public int Level => _xpSystem.Level;
        public int CurrentXP => _xpSystem.CurrentXP;
        public int XPToNextLevel => _xpSystem.XPToNextLevel;

        public Hero(string name, int HP, int attackPower, Texture2D heroTexture, Rectangle heroRect, XPSystem? xpSystem = null)
        {
            _name = name;
            _HP = HP;
            _maxHP = HP;
            _maxHP = HP;
            _attackPower = attackPower;
            _heroRect = heroRect;
            _heroTexture = heroTexture;

            _xpSystem = xpSystem ?? new XPSystem();
            _xpSystem.OnLevelUp += OnLevelUp;
        }

        private void OnLevelUp(int newLevel)
        {
            // Example level up scaling: increase max HP and attack power.
            // This can be tuned as needed.
            _maxHP += 5;
            _attackPower += 2;

            // Heal the hero to full on level up to reward the player
            _HP = _maxHP;
        }

        public void AddXP(int amount)
        {
            _xpSystem.AddXP(amount);
        }

        public void TakeDamage(int dmg)
        {
            _HP -= dmg;
            if (_HP < 0)
                _HP = 0;
        }

        public void Heal(int amount)
        {
            _HP += amount;
            if (_HP > _maxHP)
                _HP = _maxHP;
        }

        public bool IsDead()
        {
            return _HP <= 0;
        }
    }
}
