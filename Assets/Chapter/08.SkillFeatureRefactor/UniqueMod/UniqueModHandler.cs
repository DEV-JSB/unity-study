using System.Collections.Generic;


namespace SkillFeatureStudy
{
    public static class UniqueModHandler
    {
        private static Dictionary<int, Dictionary<ulong, UniqueModBase>> _heroEquippedUniqueMod = new();



        public static void UnEquipUniqueMod(int heroKey, ulong itemUniqueId)
        {
            if (_heroEquippedUniqueMod.TryGetValue(heroKey, out Dictionary<ulong, UniqueModBase> uniqueMods))
            {
                if (uniqueMods.TryGetValue(itemUniqueId, out UniqueModBase uniqueMod))
                {
                    uniqueMod.Detach();
                    uniqueMods.Remove(itemUniqueId);
                }
            }
        }

        public static void EquipUniqueMod(Hero hero, ulong itemUniqueId, EUniqueType uniqueType)
        {
            UniqueModBase uniqueMod = null;

            switch (uniqueType)
            {
                case EUniqueType.SurviveLethalOncePerStage:
                    uniqueMod = new SurviveLethalMod();
                    break;
                case EUniqueType.DamageTakenReduction10:
                    uniqueMod = new DamageTakenReductionMod(0.1f);
                    break;
                case EUniqueType.EveryFifthBaseAttackCritical:
                    uniqueMod = new EveryNthBaseAttackCriticalMod(5);
                    break;
            }

            if (uniqueMod == null)
            {
                return;
            }

            uniqueMod.Attach(hero);
            _heroEquippedUniqueMod.TryAdd(hero.HeroKey, new());
            _heroEquippedUniqueMod[hero.HeroKey].TryAdd(itemUniqueId, uniqueMod);
        }
    }
}

