using Microsoft.Xna.Framework;
using System;

namespace Movement_and_SpriteSheet_together
{
    public enum BattleState
    {
        PlayerTurn,
        EnemyTurn,
        Win,
        Lose
    }

    public class BattleSystem
    {
        public BattleState State;

        private Hero _hero;
        private Enemy _enemy;

        private Random _rng = new Random();

        private float _turnTimer = 0f;
        private const float turnDelay = 1.0f;

        public string LastAction { get; private set; } = string.Empty;

        public Hero Hero => _hero;
        public Enemy Enemy => _enemy;

        public void BattleStart(Hero hero, Enemy enemy)
        {
            _hero = hero;
            _enemy = enemy;
            State = BattleState.PlayerTurn;
            _turnTimer = 0f;
            LastAction = $"A wild {_enemy.Name} appears!";
        }

        public void Update(GameTime gameTime)
        {
            if (State == BattleState.EnemyTurn)
            {
                _turnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_turnTimer >= turnDelay)
                {
                    _turnTimer = 0f;
                    EnemyAttack();
                }
            }
        }

        public void HeroAttack()
        {
            if (State != BattleState.PlayerTurn)
                return;

            int dmg = GetRandomDamage(_hero.AttackPower);
            _enemy.TakeDamage(dmg);
            LastAction = $"{_hero.Name} hits {_enemy.Name} for {dmg} dmg.";

            if (_enemy.IsDead())
            {
                State = BattleState.Win;
                LastAction = $"{_hero.Name} defeated {_enemy.Name}!";
                return;
            }

            State = BattleState.EnemyTurn;
            _turnTimer = 0f;
        }

        public void HeroHeal()
        {
            if (State != BattleState.PlayerTurn) return;

            int heals = GetRandomHeal(_hero.HP);
            _hero.Heal(heals);
            LastAction = $"{_hero.Name} heals for {heals} amount of health.";

            State = BattleState.EnemyTurn;
            _turnTimer = 0f;
        }

        private void EnemyAttack()
        {
            if (State != BattleState.EnemyTurn)
                return;

            int dmg = GetRandomDamage(_enemy.AttackPower);
            _hero.TakeDamage(dmg);
            LastAction = $"{_enemy.Name} hits {_hero.Name} for {dmg} dmg.";

            if (_hero.IsDead())
            {
                State = BattleState.Lose;
                LastAction = $"{_hero.Name} was defeated by {_enemy.Name}...";
                return;
            }

            State = BattleState.PlayerTurn;
        }

        private int GetRandomDamage(int baseAttack)
        {
            int damage = _rng.Next(0, 3);
            return baseAttack + damage;
        }

        public int GetRandomHeal(int baseHeal)
        {
            int heal = _rng.Next(5, _hero.MaxHP / 2);
            return heal;
        }
    }
}
