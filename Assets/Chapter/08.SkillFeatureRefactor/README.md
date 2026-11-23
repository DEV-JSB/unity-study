# 08. SkillFeatureRefactor — 유니크 if 분기를 Observer 구독으로 리팩토링

> 정리 노트
> - [study-notes / unique-mod-observer](https://dev-jsb.github.io/study-notes/unity/unique-mod-observer.html) — 유니크가 늘 때마다 if가 쌓인다, Observer 구독으로 옮기기 (글 버전 [unique-mod-observer.md](https://github.com/DEV-JSB/study-notes/blob/main/unity/unique-mod-observer.md))

- 씬: `08.SkillFeatureRefactor.unity` — `UniqueBehaviourChecker`가 붙어 있다. Play 하거나 컴포넌트 메뉴 "유니크 동작 검사 실행"
- 스크립트: `Hero.cs`, `Gear.cs`, `Stage.cs`, `Battle.cs`, `DamageCalculator.cs`, `Observer.cs`
- 스크립트: `UniqueMod/` — 유니크 모드 부모와 세 유니크, 사건 상자, `UniqueModHandler`
