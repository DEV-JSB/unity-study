using UnityEngine;

namespace SkillFeatureStudy
{
    public class UniqueMod
    {
        protected readonly Hero _ownerHero;
        protected readonly ETriggerType _triggerType;
        protected readonly IUniqueCondition[] _condition;
        protected readonly IUniqueEffect[] _effect;
        protected readonly EUniqueType _uniqueType;

        public UniqueMod(Hero hero, ETriggerType triggerType, IUniqueCondition[] condition, IUniqueEffect[] effect, EUniqueType uniqueType)
        {
            _ownerHero = hero;
            _triggerType = triggerType;
            _condition = condition;
            _effect = effect;
            _uniqueType = uniqueType;
        }
        
        public virtual void Attach()
        {
            switch (_triggerType)
            {
                case ETriggerType.DamageApplying:
                    _ownerHero.DamageApplying.Subscribe(OnTriggered);
                    break;
                case ETriggerType.DamageModifying:
                    _ownerHero.DamageModifying.Subscribe(OnTriggered);
                    break;
                case ETriggerType.BaseAttacking:
                    _ownerHero.BaseAttacking.Subscribe(OnTriggered);
                    break;
            }
        }

        public virtual void Detach()
        {
            switch (_triggerType)
            {
                case ETriggerType.DamageApplying:
                    _ownerHero.DamageApplying.Unsubscribe(OnTriggered);
                    break;
                case ETriggerType.DamageModifying:
                    _ownerHero.DamageModifying.Unsubscribe(OnTriggered);
                    break;
                case ETriggerType.BaseAttacking:
                    _ownerHero.BaseAttacking.Unsubscribe(OnTriggered);
                    break;
            }
        }

        private void OnTriggered(TriggerContext context)
        {
            if (_condition != null)
            {
                foreach (var condition in _condition)
                {
                    if (condition == null)
                    {
                        continue;
                    }
                
                    if (!condition.IsSatisfied(_ownerHero, context))
                    {
                        return;
                    }
                }
            }
            

            foreach (var effect in _effect)
            {
                effect.Execute(_ownerHero, context);
            }
            UniqueModHandler.RegisterActivateHistory(_ownerHero.HeroKey, _uniqueType, 1);
        }
    }

}
