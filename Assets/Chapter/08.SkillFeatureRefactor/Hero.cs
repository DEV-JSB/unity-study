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
        
        
        private readonly List<EUniqueType> _equippedUniques = new List<EUniqueType>();

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
        
        // 유니크 하나를 낀다. 같은 유니크를 두 번 끼면 목록에 두 개가 들어간다
        public void Equip(EUniqueType eUniqueType)
        {
            _equippedUniques.Add(eUniqueType);
        }

        // 유니크 하나를 뺀다. 같은 유니크가 두 개면 하나만 빠진다
        public void Unequip(EUniqueType eUniqueType)
        {
            _equippedUniques.Remove(eUniqueType);
        }

        public bool HasUnique(EUniqueType eUniqueType)
        {
            return _equippedUniques.Contains(eUniqueType);
        }

        public int CountUnique(EUniqueType eUniqueType)
        {
            int uniqueCount = 0;
            foreach (EUniqueType equippedUnique in _equippedUniques)
            {
                if (equippedUnique == eUniqueType)
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
                _uniqueActivateCount[eUniqueType]++;
            }
        }


    }
}
