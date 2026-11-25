using System.Collections.Generic;

namespace SkillFeatureStudy
{
    // 영웅. 낀 장비, 평타 횟수, 유니크가 구독하는 사건, 체력(HealthController).
    // 유니크 동작은 Hero 에 없다. 장비를 끼면 장비에 달린 유니크 모드 객체(UniqueMod 폴더)가 만들어져
    // Hero 의 사건(DamageModifying, DamageApplying, BaseAttacking)을 구독한다. 사건을 울리는 건 DamageCalculator 와 Battle 이다.
    // Hero 에는 피해를 받는 입구가 없다. 피해는 DamageCalculator 를 거쳐 HealthController 로 들어가고,
    // Hero 는 HealthController 의 알림(Damaged, Died)을 구독해서 연출만 한다.
    // 유니크 사용 기록은 UniqueModHandler 에 있다 (규칙은 EUniqueType.cs, 정답은 검사기).
    public class Hero
    {
        // 다음에 만들어질 영웅에게 줄 고유 키. 영웅을 만들 때마다 1씩 늘어나서 겹치지 않는다.
        // static 이라 모든 영웅이 이 칸 하나를 같이 쓴다 (그래야 번호가 이어진다)
        private static int _nextHeroKey = 1;

        // 이 영웅의 고유 키. 영웅마다 다르고 바뀌지 않는다. UniqueModHandler 의 사용 기록(영웅 → 유니크 종류 → 횟수)에서 키로 쓴다
        public int HeroKey { get; }

        public readonly string heroName;
        public readonly float attackPower;
        private int _attackCount = 0;
        public int AttackCount => _attackCount;
        
        
        // 장비 고유 번호 → 낀 장비. 장비 하나에 유니크 모드가 하나 달려 있다
        private readonly Dictionary<ulong, Gear> _equippedGearsByUniqueId = new ();


        
        
        public readonly Observer<DamageContext> DamageModifying = new ();
        public readonly Observer<DamageContext> DamageApplying = new ();

        public readonly Observer<BaseAttackContext> BaseAttacking = new();
        
        // 체력은 HealthController 가 맡는다. 검사기가 hero.CurrentHp / hero.IsDead 를 쓰니까 이름은 남긴다
        public HealthController Health { get; }
        public float CurrentHp => Health.CurrentHp;
        public bool IsDead => Health.IsDead;


        public void IncreaseAttackCount()
        {
            _attackCount++;
        }

        public Hero(string heroName, float startHp, float attackPower)
        {
            HeroKey = _nextHeroKey;
            ++_nextHeroKey;

            this.heroName = heroName;
            this.attackPower = attackPower;
            Health = new HealthController(startHp);
            Health.Damaged.Subscribe(OnDamaged);
            Health.Died.Subscribe(OnDied);
            Health.HpChanged.Subscribe(OnHpChanged);
        }

        // 쓰러졌을 때 영웅이 할 일 (지금은 없음)
        private void OnDied(HealthController health)
        {
        }

        
        // 장비 하나를 낀다. 같은 유니크가 달린 장비를 두 개 끼면 (고유 번호가 다르니) 두 개 다 들어간다.
        // 같은 장비(같은 고유 번호)는 두 번 낄 수 없다는 게 게임 규칙이라, 들어오면 버그 방어로 무시한다
        public void Equip(Gear gear)
        {
            if (!_equippedGearsByUniqueId.TryAdd(gear.UniqueId, gear))
            {
                return;
            }

            UniqueModHandler.EquipUniqueMod(this, gear.UniqueId, gear.UniqueType);
        }

        // 장비 고유 번호로 장비 하나를 뺀다. 끼지 않은 번호면 아무 일도 없다
        public void Unequip(ulong gearUniqueId)
        {
            _equippedGearsByUniqueId.Remove(gearUniqueId);

            UniqueModHandler.UnEquipUniqueMod(HeroKey, gearUniqueId);
        }

        // 피해를 받았을 때의 연출 (피격 이펙트처럼 피해 정보가 필요한 것. 지금은 없음)
        private void OnDamaged(DamageContext damageContext)
        {
        }

        // 체력이 감소했을때의 연출
        public void OnHpChanged(float currentHp)
        {
            
        }


    }
}
