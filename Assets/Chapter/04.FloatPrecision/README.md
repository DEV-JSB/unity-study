# 04. float 정밀도 — 좌표가 커지면 캐릭터가 멈춘다

> 그림·코드로 정리한 노트: [study-notes / float-precision](https://dev-jsb.github.io/study-notes/unity/float-precision.html)

## 발단

실무 프로젝트에서 캐릭터가 좌표 한 축을 따라 계속 이동하는데, 같은 스테이지를 반복할 때 좌표를 0으로 되돌리는 코드가 없었다. 게임을 오래 켜 둔 유저에게서 문제가 생겼다.

Unity 좌표가 `float`이라는 것과, 좌표가 커지면 작은 이동값이 반올림으로 사라진다는 것을 몰랐다. 그래서 빈 씬에서 같은 조건을 만들어 직접 쟀다.

## 재는 것

매 프레임 **더하려던 이동량**과 **실제로 늘어난 좌표**를 비교한다.

```csharp
float requested = _speed * deltaTime;               // 더하려던 이동량

float before = transform.position.x;
transform.position += new Vector3(requested, 0f, 0f);
float after = transform.position.x;

float actual = after - before;                      // 실제로 늘어난 좌표
```

60프레임마다 Console에 요약 한 줄이 찍힌다.

```
[60프레임 요약] x … | 눈금 간격 … | 요청 합 … | 실제 합 … (…%) | 안 움직인 프레임 …
```

## 씬 구성

씬은 [04.FloatPrecision.unity](04.FloatPrecision.unity).

| 오브젝트 | 스크립트 | 하는 일 |
|---|---|---|
| `Mover` (+ 자식 `Sphere`) | [FloatPrecision.cs](FloatPrecision.cs) | 이동하고 측정한다. 부모 없는 루트 오브젝트여야 한다 |
| `Main Camera` | [FloatPrecisionView.cs](FloatPrecisionView.cs) | `Mover`를 따라가고, 월드의 정수 좌표마다 눈금 막대를 세운다 (눈으로 보기용) |
| `Canvas`의 버튼 두 개 | — | `X = 0`: 좌표를 0으로. `X = Start X`: Inspector의 Start X 좌표로 |

`Mover`의 Inspector 값:

| 칸 | 뜻 |
|---|---|
| Start X | 시작 좌표 |
| Speed | 1초에 가는 거리 |
| Delta Mode | `Fixed60`: 매 프레임 1/60초로 고정 (돌릴 때마다 같은 숫자). `UnityDeltaTime`: `Time.deltaTime` 그대로 |
| Log Every Frame | 프레임마다 한 줄씩 찍기 |
| Summary Frames | 이 프레임 수마다 요약 한 줄 |

## 결과 요약

Speed 3 (60프레임이면 한 프레임에 0.05):

| 좌표 | 눈금 간격 | 1초에 실제로 간 거리 |
|---|---|---|
| 0 근처 | 아주 촘촘 | 3.0 (정상) |
| 1,000,000 | 0.0625 | 3.75 (25% 빠름) |
| 1,048,576 이상 | 0.125 | 0 (멈춤) |
| 9,999,999 | 1 | 0 (멈춤) |

좌표가 1,000,000일 때 Speed 1.85까지는 멈추고, 1.89부터는 Speed를 얼마로 넣든 초당 3.75로 간다. 왜 그런지와 `float`의 구조는 위의 정리 노트에 있다.
