# 06. 비트 플래그 — 8×8 격자와 ulong 키

> 정리 노트
> - [study-notes / bit-flag](https://dev-jsb.github.io/study-notes/unity/bit-flag.html) — 칸 8개를 int 하나에 담는다 (글 버전 [bit-flag.md](https://github.com/DEV-JSB/study-notes/blob/main/unity/bit-flag.md))
> - [study-notes / ulong-key](https://dev-jsb.github.io/study-notes/unity/ulong-key.html) — enum 두 개를 ulong 키 하나로 (글 버전 [ulong-key.md](https://github.com/DEV-JSB/study-notes/blob/main/unity/ulong-key.md))

- 씬: `06.BitFlag.unity`
- 스크립트: `BitGrid.cs` + `BitGridView.cs` — 8×8 체크박스 격자를 int 8개에 비트로 담기
- 스크립트: `ItemKey.cs` + `ItemKeyChecker.cs` — enum 두 개를 ulong 키 하나로 묶고 꺼내기 (숫자 그대로 넣기 / 비트 하나씩 켜기), 체커는 빈 GameObject에 붙이고 Play
