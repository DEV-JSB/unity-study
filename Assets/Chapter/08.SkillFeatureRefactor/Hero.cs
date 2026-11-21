using System.Collections.Generic;

namespace SkillFeatureStudy
{
    // 리팩토링 "전" 영웅. 장착 목록, 피해 처리, 평타 횟수.
    // 유니크 A·C 의 동작이 TakeDamage 안에 if 분기로 있다. A 사용 기록은 StartSetting 이 스테이지마다 지운다
    // (규칙은 EUniqueType.cs, 정답은 검사기).
    public class Hero
    {
        public readonly string heroName;
        public readonly float attackPower;
        private int _attackCount = 0;
        public int AttackCount => _attackCount;
        
        
        // 장비 고유 번호 → 낀 장비. 장비 하나에 유니크 모드가 하나 달려 있다
        private readonly Dictionary<ulong, Gear> _equippedGearsByUniqueId = new Dictionary<ulong, Gear>();

        private readonly Dictionary<ulong, UniqueModBase> _equippedUniqueMod = new ();
        private Dictionary<EUniqueType, int> _uniqueActivateCount = new Dictionary<EUniqueType, int>();

        public float CurrentHp { get; private set; }


        public void IncreaseAttackCount()
        {
            _attackCount++;
        }
        
        public bool IsDead
        {
            get { return CurrentHp <= 0f; }
        }

        public Hero(string heroName, float startHp, float attackPower)
        {
            this.heroName = heroName;
            this.attackPower = attackPower;
            CurrentHp = startHp;
        }

        public void StartSetting()
        {
            _uniqueActivateCount.Clear();
        }
        
        // 장비 하나를 낀다. 같은 유니크가 달린 장비를 두 개 끼면 (고유 번호가 다르니) 두 개 다 들어간다.
        // 이미 낀 장비(같은 고유 번호)를 또 끼면 아무 일도 없다
        public void Equip(Gear gear)
        {
            if (!_equippedGearsByUniqueId.TryAdd(gear.UniqueId, gear))
            {
                return;
            }

            UniqueModBase uniqueMod = null;
            switch (gear.UniqueType)
            {
                case EUniqueType.SurviveLethalOncePerStage:
                    uniqueMod = new SurviveLethalMod();
                    break;
                case EUniqueType.DamageTakenReduction10:
                    uniqueMod = new DamageTakenReductionMod();
                    break;
                case EUniqueType.EveryFifthBaseAttackCritical:
                    uniqueMod = new EveryNthBaseAttackCriticalMod();
                    break;
            }

            if (uniqueMod == null)
            {
                return;
            }

            uniqueMod.Attach(this);
            _equippedUniqueMod.Add(gear.UniqueId, uniqueMod);
        }

        // 장비 고유 번호로 장비 하나를 뺀다. 끼지 않은 번호면 아무 일도 없다
        public void Unequip(ulong gearUniqueId)
        {
            _equippedGearsByUniqueId.Remove(gearUniqueId);

            if (_equippedUniqueMod.TryGetValue(gearUniqueId, out UniqueModBase uniqueMod))
            {
                uniqueMod.Detach();
                _equippedUniqueMod.Remove(gearUniqueId);
            }
        }

        public bool HasUnique(EUniqueType eUniqueType)
        {
            return CountUnique(eUniqueType) > 0;
        }

        // 낀 장비 중 이 유니크가 달린 장비 개수
        public int CountUnique(EUniqueType eUniqueType)
        {
            int uniqueCount = 0;
            foreach (Gear equippedGear in _equippedGearsByUniqueId.Values)
            {
                if (equippedGear.UniqueType == eUniqueType)
                {
                    ++uniqueCount;
                }
            }
            return uniqueCount;
        }

        // 피해를 받는다. 체력이 0 이하가 되면 0으로 두고 쓰러진다
        public void TakeDamage(float incomingDamage)
        {
            if (IsDead)
            {
                return;
            }

            if (HasUnique(EUniqueType.DamageTakenReduction10))
            {
                int reductionValue = CountUnique(EUniqueType.DamageTakenReduction10);
                incomingDamage *= (1f - (0.1f * reductionValue));
            }
            
            if (CurrentHp - incomingDamage > 0f)
            {
                CurrentHp -= incomingDamage;
                return;
            }


            if (!_uniqueActivateCount.TryGetValue(EUniqueType.SurviveLethalOncePerStage, out int activateCount) &&
                HasUnique(EUniqueType.SurviveLethalOncePerStage))
            {
                IncreaseUniqueActivateCount(EUniqueType.SurviveLethalOncePerStage, 1);
                CurrentHp = 1f;
                return;
            }

            CurrentHp = 0f;
        }

        private void IncreaseUniqueActivateCount(EUniqueType eUniqueType, int count)
        {
            if (!_uniqueActivateCount.TryAdd(eUniqueType, 1))
            {
                _uniqueActivateCount[eUniqueType] += count;
            }
        }


    }
}
