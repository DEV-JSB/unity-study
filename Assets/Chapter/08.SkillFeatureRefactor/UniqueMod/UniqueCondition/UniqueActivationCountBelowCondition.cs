using UnityEngine;

namespace SkillFeatureStudy
{
    public class UniqueActivationCountBelowCondition : IUniqueCondition
    {
        private readonly EUniqueType _uniqueType;
        private readonly int _count;

        public UniqueActivationCountBelowCondition(int count, EUniqueType uniqueType)
        {
            _count = count;
            _uniqueType = uniqueType;
        }
        
        public bool IsSatisfied(Hero hero, TriggerContext context)
        {
            int activateHistory =
                UniqueModHandler.GetActivateHistory(hero.HeroKey, _uniqueType);
            if (activateHistory < _count)
            {
                return true;
            }
            return false;
        }
    }

}
