using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class Enemy
    {
        private string _name;
        private int _HP;
        private int _attackPower;
        private int _xpValue;

        public string Name => _name;
        public int HP => _HP;
        public int AttackPower => _attackPower;

        public int XPValue => _xpValue;

        public Enemy(string name, int HP, int attackPower, int xpValue = 5)
        {
            _name = name;
            _HP = HP;
            _attackPower = attackPower;
            _xpValue = xpValue;
        }

        public void TakeDamage(int dmg)
        {
            _HP -= dmg;
            if (_HP < 0)
                _HP = 0;
        }

        public bool IsDead()
        {
            return _HP <= 0;
        }
    }
}
