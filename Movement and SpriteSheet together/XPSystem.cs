using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movement_and_SpriteSheet_together
{
    public class XPSystem
    {
        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; } = 0;

        // Simple progression formula: base 10 XP + 5 per level above 1
        public int XPToNextLevel => 10 + (Level - 1) * 5;

        public event Action<int>? OnLevelUp;

        public XPSystem() { }

        public void AddXP(int amount)
        {
            if (amount <= 0)
                return;

            CurrentXP += amount;

            // Handle multiple level-ups if a large XP chunk is added
            while (CurrentXP >= XPToNextLevel)
            {
                CurrentXP -= XPToNextLevel;
                Level++;
                OnLevelUp?.Invoke(Level);
            }
        }

        public void Reset()
        {
            Level = 1;
            CurrentXP = 0;
        }
    }
}
