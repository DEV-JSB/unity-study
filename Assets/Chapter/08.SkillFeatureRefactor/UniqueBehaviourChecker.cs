using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SkillFeatureStudy
{
    // 유니크 세 개(A, B, C)의 동작을 시나리오로 돌려서 정답 값과 비교한다.
    // 사용법: 빈 GameObject에 붙이고 Play 하거나, Play 없이 컴포넌트 메뉴의 "유니크 동작 검사 실행"을 누른다.
    //
    // 이 검사기의 역할: 리팩토링 전후 동작이 같은지 확인하는 안전망 (특성 테스트, characterization test).
    //   리팩토링은 동작은 그대로 두고 구조만 바꾸는 것이다. 그래서 리팩토링 뒤에도 이 파일은 고치지 않고,
    //   전부 성공으로 나와야 한다.
    //
    // 검사기가 부르는 바깥 버튼 — 리팩토링 뒤에도 이 이름과 모양은 남겨야 한다
    //   new Hero(heroName, startHp, attackPower), hero.Equip, hero.Unequip, hero.TakeDamage,
    //   hero.CurrentHp, hero.IsDead, new Stage(heroes), stage.StartStage,
    //   new Battle(), battle.BaseAttack, AttackResult.isCritical
    //
    // Console에는 시나리오 하나 = 로그 하나. 성공은 일반 로그, 실패는 빨간 에러. 클릭하면 단계별 값이 나온다.
    public class UniqueBehaviourChecker : MonoBehaviour
    {
        private const string SuccessWord = "<color=#5DC264>성공</color>";
        private const string FailWord = "<color=#FF6B6B>실패</color>";
        private const float HpTolerance = 0.001f;

        // 시나리오 하나의 단계별 기록. 단계마다 정답과 실제 값을 한 줄씩 남긴다
        private class ScenarioRecord
        {
            public readonly string scenarioTitle;
            public readonly StringBuilder stepLines = new StringBuilder();
            public int stepCount;
            public int failedStepCount;

            public ScenarioRecord(string scenarioTitle)
            {
                this.scenarioTitle = scenarioTitle;
            }

            public void ExpectHp(string stepDescription, Hero hero, float expectedHp)
            {
                bool isCorrect = Math.Abs(hero.CurrentHp - expectedHp) <= HpTolerance;
                AddLine(stepDescription, isCorrect, $"정답 체력 {expectedHp}", $"실제 체력 {hero.CurrentHp}");
            }

            public void ExpectDead(string stepDescription, Hero hero, bool expectedIsDead)
            {
                bool isCorrect = hero.IsDead == expectedIsDead;
                AddLine(stepDescription, isCorrect, $"정답 쓰러짐 {expectedIsDead}", $"실제 쓰러짐 {hero.IsDead}");
            }

            public void ExpectText(string stepDescription, string expectedText, string actualText)
            {
                bool isCorrect = expectedText == actualText;
                AddLine(stepDescription, isCorrect, $"정답 {expectedText}", $"실제 {actualText}");
            }

            private void AddLine(string stepDescription, bool isCorrect, string expectedText, string actualText)
            {
                ++stepCount;
                if (!isCorrect)
                {
                    ++failedStepCount;
                }
                stepLines.AppendLine($"{(isCorrect ? "맞음" : "틀림")}  {stepDescription}");
                stepLines.AppendLine($"    {expectedText}  /  {actualText}");
            }
        }

        private void Start()
        {
            RunAllScenarios();
        }

        [ContextMenu("유니크 동작 검사 실행")]
        private void RunAllScenarios()
        {
            var scenarioRunners = new List<Func<ScenarioRecord>>
            {
                RunNoUniqueScenario,
                RunSurviveLethalScenario,
                RunSurviveLethalReEquipScenario,
                RunEveryFifthCriticalScenario,
                RunDamageTakenReductionScenario,
                RunSurviveLethalWithReductionScenario,
            };

            foreach (Func<ScenarioRecord> runScenario in scenarioRunners)
            {
                ScenarioRecord scenarioRecord;
                try
                {
                    scenarioRecord = runScenario();
                }
                catch (Exception scenarioException)
                {
                    // 리팩토링 도중 null 참조 같은 예외가 나도 나머지 시나리오는 계속 돈다
                    Debug.LogError($"{FailWord}  시나리오 도중 예외: {scenarioException.GetType().Name}: {scenarioException.Message}\n"
                                   + scenarioException.StackTrace);
                    continue;
                }
                LogScenario(scenarioRecord);
            }
        }

        // 1. 유니크 없음: 기본 피해 처리만 확인한다
        private ScenarioRecord RunNoUniqueScenario()
        {
            var scenarioRecord = new ScenarioRecord("1. 유니크 없음");
            var hero = new Hero("Knight", 100f, 10f);
            var stage = new Stage(hero);
            stage.StartStage();

            hero.TakeDamage(30f);
            scenarioRecord.ExpectHp("체력 100에서 피해 30", hero, 70f);

            hero.TakeDamage(200f);
            scenarioRecord.ExpectHp("체력 70에서 피해 200 (버티기 없음)", hero, 0f);
            scenarioRecord.ExpectDead("쓰러졌나", hero, true);
            return scenarioRecord;
        }

        // 2. A: 스테이지당 한 번 버티고, 새 스테이지가 시작되면 다시 쓸 수 있다
        private ScenarioRecord RunSurviveLethalScenario()
        {
            var scenarioRecord = new ScenarioRecord("2. A 스테이지당 한 번 치명상 버티기");
            var hero = new Hero("Knight", 100f, 10f);
            hero.Equip(EUniqueType.SurviveLethalOncePerStage);
            var stage = new Stage(hero);
            stage.StartStage();

            hero.TakeDamage(150f);
            scenarioRecord.ExpectHp("첫 스테이지, 체력 100에서 피해 150 → 버팀", hero, 1f);

            stage.StartStage();
            hero.TakeDamage(150f);
            scenarioRecord.ExpectHp("새 스테이지, 체력 1에서 피해 150 → 다시 버팀", hero, 1f);

            hero.TakeDamage(150f);
            scenarioRecord.ExpectHp("같은 스테이지, 체력 1에서 피해 150 → 이미 써서 쓰러짐", hero, 0f);
            scenarioRecord.ExpectDead("쓰러졌나", hero, true);
            return scenarioRecord;
        }

        // 3. A: 같은 스테이지에서 뺐다 다시 껴도 "썼음" 표시는 남아 있다 (끼었다 뺐다로 횟수 초기화 방지)
        private ScenarioRecord RunSurviveLethalReEquipScenario()
        {
            var scenarioRecord = new ScenarioRecord("3. A 같은 스테이지에서 뺐다 끼기");
            var hero = new Hero("Knight", 100f, 10f);
            hero.Equip(EUniqueType.SurviveLethalOncePerStage);
            var stage = new Stage(hero);
            stage.StartStage();

            hero.TakeDamage(150f);
            scenarioRecord.ExpectHp("체력 100에서 피해 150 → 버팀", hero, 1f);

            hero.Unequip(EUniqueType.SurviveLethalOncePerStage);
            hero.Equip(EUniqueType.SurviveLethalOncePerStage);
            hero.TakeDamage(150f);
            scenarioRecord.ExpectHp("A를 뺐다 다시 끼고 피해 150 → 이미 써서 쓰러짐", hero, 0f);
            return scenarioRecord;
        }

        // 4. B: 평타 다섯 번째마다 치명타. B를 안 낀 영웅은 치명타가 없다
        private ScenarioRecord RunEveryFifthCriticalScenario()
        {
            var scenarioRecord = new ScenarioRecord("4. B 다섯 번째 평타마다 치명타");

            var attackerWithUnique = new Hero("Archer", 100f, 10f);
            attackerWithUnique.Equip(EUniqueType.EveryFifthBaseAttackCritical);
            var targetOfUnique = new Hero("Dummy", 1000f, 0f);
            var stage = new Stage(attackerWithUnique, targetOfUnique);
            stage.StartStage();
            var battle = new Battle();

            string criticalAttackNumbersText = AttackTenTimes(battle, attackerWithUnique, targetOfUnique);
            scenarioRecord.ExpectText("B를 낀 영웅이 평타 10번, 치명타가 난 번호", "5, 10", criticalAttackNumbersText);
            scenarioRecord.ExpectHp("공격력 10, 평타 8번 + 치명타 2번(20씩) = 피해 120", targetOfUnique, 880f);

            var attackerWithoutUnique = new Hero("Archer", 100f, 10f);
            var targetOfNoUnique = new Hero("Dummy", 1000f, 0f);
            var stageWithoutUnique = new Stage(attackerWithoutUnique, targetOfNoUnique);
            stageWithoutUnique.StartStage();
            var battleWithoutUnique = new Battle();

            string noUniqueCriticalNumbersText = AttackTenTimes(battleWithoutUnique, attackerWithoutUnique, targetOfNoUnique);
            scenarioRecord.ExpectText("B를 안 낀 영웅이 평타 10번, 치명타가 난 번호", "(없음)", noUniqueCriticalNumbersText);
            scenarioRecord.ExpectHp("공격력 10, 평타 10번 = 피해 100", targetOfNoUnique, 900f);
            return scenarioRecord;
        }

        // 5. C: 하나 끼면 10%, 둘 끼면 20% 감소. 빼면 그만큼 돌아온다
        private ScenarioRecord RunDamageTakenReductionScenario()
        {
            var scenarioRecord = new ScenarioRecord("5. C 받는 피해 10% 감소 (겹치면 더함)");
            var hero = new Hero("Knight", 1000f, 10f);
            var stage = new Stage(hero);
            stage.StartStage();

            hero.Equip(EUniqueType.DamageTakenReduction10);
            hero.TakeDamage(100f);
            scenarioRecord.ExpectHp("C 한 개, 피해 100 → 90 받음", hero, 910f);

            hero.Equip(EUniqueType.DamageTakenReduction10);
            hero.TakeDamage(100f);
            scenarioRecord.ExpectHp("C 두 개, 피해 100 → 80 받음", hero, 830f);

            hero.Unequip(EUniqueType.DamageTakenReduction10);
            hero.TakeDamage(100f);
            scenarioRecord.ExpectHp("C 하나 뺌(한 개 남음), 피해 100 → 90 받음", hero, 740f);

            hero.Unequip(EUniqueType.DamageTakenReduction10);
            hero.TakeDamage(100f);
            scenarioRecord.ExpectHp("C 다 뺌, 피해 100 → 100 받음", hero, 640f);
            return scenarioRecord;
        }

        // 6. A + C: 감소가 먼저, 쓰러지나 판정은 그 다음. 순서가 바뀌면 값이 달라진다
        private ScenarioRecord RunSurviveLethalWithReductionScenario()
        {
            var scenarioRecord = new ScenarioRecord("6. A + C 순서 (감소 먼저, 치명상 판정 나중)");
            var hero = new Hero("Knight", 100f, 10f);
            hero.Equip(EUniqueType.SurviveLethalOncePerStage);
            hero.Equip(EUniqueType.DamageTakenReduction10);
            var stage = new Stage(hero);
            stage.StartStage();

            hero.TakeDamage(105f);
            scenarioRecord.ExpectHp("체력 100에서 피해 105 → 10% 줄어 94.5 → 안 쓰러짐(A 안 씀)", hero, 5.5f);

            hero.TakeDamage(200f);
            scenarioRecord.ExpectHp("피해 200 → 180 → 쓰러질 피해라 A로 버팀", hero, 1f);

            hero.TakeDamage(200f);
            scenarioRecord.ExpectHp("피해 200 → A 이미 써서 쓰러짐", hero, 0f);
            return scenarioRecord;
        }

        // 평타 10번을 치고, 치명타가 난 번호(1부터)를 "5, 10" 모양 글자로 돌려준다
        private static string AttackTenTimes(Battle battle, Hero attacker, Hero target)
        {
            var criticalAttackNumbers = new List<int>();
            for (int attackNumber = 1; attackNumber <= 10; ++attackNumber)
            {
                AttackResult attackResult = battle.BaseAttack(attacker, target);
                if (attackResult.isCritical)
                {
                    criticalAttackNumbers.Add(attackNumber);
                }
            }
            return criticalAttackNumbers.Count == 0 ? "(없음)" : string.Join(", ", criticalAttackNumbers);
        }

        private static void LogScenario(ScenarioRecord scenarioRecord)
        {
            string logPrefix = $"[{scenarioRecord.scenarioTitle}]";
            if (scenarioRecord.failedStepCount == 0)
            {
                Debug.Log($"{logPrefix} {SuccessWord}  {scenarioRecord.stepCount}단계 모두 맞음 (클릭하면 단계별 값)\n\n"
                          + scenarioRecord.stepLines);
                return;
            }

            Debug.LogError($"{logPrefix} {FailWord}  {scenarioRecord.stepCount}단계 중 {scenarioRecord.failedStepCount}단계 틀림\n\n"
                           + scenarioRecord.stepLines);
        }
    }
}
