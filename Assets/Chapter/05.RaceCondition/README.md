# 05. 경쟁 상태와 lock — 같은 변수를 두 스레드가 더하면 값이 사라진다

> 정리 노트: [study-notes / race-condition](https://dev-jsb.github.io/study-notes/unity/race-condition.html) (글 버전 [race-condition.md](https://github.com/DEV-JSB/study-notes/blob/main/unity/race-condition.md))

- 씬: `05.RaceCondition.unity`
- 스크립트: `RaceCondition.cs` — Inspector의 `Fix Mode`(None / Lock / InterLocked / Semaphore), `Loop Count`, `Trial Count`를 바꿔 가며 실행
