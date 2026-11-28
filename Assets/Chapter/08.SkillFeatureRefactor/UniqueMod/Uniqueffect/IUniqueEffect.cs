using UnityEngine;

namespace SkillFeatureStudy
{
    public interface IUniqueEffect
    {
        void Execute(Hero ownerHero, TriggerContext context);
    }
}
