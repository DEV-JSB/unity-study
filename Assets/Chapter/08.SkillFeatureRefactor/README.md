# 08. SkillFeatureRefactor — 유니크 if 분기를 사건 구독으로 리팩토링

- 씬: 따로 없음. 아무 씬에 빈 GameObject를 만들고 `UniqueBehaviourChecker`를 붙인다
- 스크립트: `UniqueType.cs` — 유니크 세 개 enum과 규칙 (A 치명상 버티기 / B 다섯 번째 평타 치명타 / C 받는 피해 10% 감소)
- 스크립트: `Hero.cs` — 장착 목록, 기본 피해 처리
- 스크립트: `DamageCalculator.cs` — 평타 피해 계산
- 스크립트: `Stage.cs` — 스테이지 시작
- 스크립트: `Battle.cs` — 평타 한 번 흐름 (피해 계산 → 대상에게 피해)
- 스크립트: `UniqueBehaviourChecker.cs` — 시나리오 6개. Play 하거나 컴포넌트 메뉴 "유니크 동작 검사 실행". 리팩토링 뒤에도 이 파일은 고치지 않는다
