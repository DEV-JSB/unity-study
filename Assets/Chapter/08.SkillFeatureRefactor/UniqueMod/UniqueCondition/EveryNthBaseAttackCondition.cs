using UnityEngine;

namespace SkillFeatureStudy
{
    public class EveryNthBaseAttackCondition : IUniqueCondition
    {
        private readonly int _n;

        public EveryNthBaseAttackCondition(int n)
        {
            _n = n;
        }
        
        
        public bool IsSatisfied(Hero hero, TriggerContext context)
        {
            if (hero != null && hero.AttackCount % _n == 0)
            {
                return true;
            }
            return false;
        }   
    }
}
