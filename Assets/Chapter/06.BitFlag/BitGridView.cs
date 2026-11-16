using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// 8×8 체크박스 격자 + 버튼 UI. 빈 GameObject에 붙이면 Start에서 화면을 전부 코드로 만든다.
// 비트 계산은 전부 BitGrid가 하고, 이 스크립트는 두 방향만 맡는다.
//   클릭 → 값 : 칸을 누르면 OnCellClicked → _bitGrid.Toggle(x, y)
//   값 → 화면 : RefreshView가 _bitGrid.IsOn(x, y)로 64칸의 체크 표시를 다시 맞춘다
// 체크 표시는 Toggle 컴포넌트 자신의 isOn이 아니라 항상 BitGrid 값으로 그린다.
// 그래서 BitGrid 연산이 틀리면 화면도 틀리게 보인다 (버그가 숨지 않는다).
public class BitGridView : MonoBehaviour
{
    [SerializeField] private float _cellSize = 56f;
    [SerializeField] private float _cellSpacing = 6f;
    [SerializeField] private Vector2 _gridCenter = new Vector2(-330f, -20f);
    [SerializeField] private Color _cellColor = new Color(0.22f, 0.24f, 0.28f);
    [SerializeField] private Color _checkMarkColor = new Color(0.35f, 0.8f, 0.45f);
    [SerializeField] private Color _buttonColor = new Color(0.3f, 0.4f, 0.6f);

    private readonly BitGrid _bitGrid = new BitGrid();
    private readonly Toggle[,] _cellToggles = new Toggle[BitGrid.Size, BitGrid.Size];
    private readonly Image[,] _cellCheckMarks = new Image[BitGrid.Size, BitGrid.Size];
    private Text _rawBitsText;
    private Font _koreanFont;    // 한글용
    private Font _monospaceFont; // 2진수 표시용 (글자 폭이 같아야 자릿수가 세로로 맞는다)

    private void Start()
    {
        _koreanFont = CreateFont("Malgun Gothic");
        _monospaceFont = CreateFont("Consolas");

        EnsureEventSystem();
        RectTransform canvasRect = CreateCanvas();
        CreateTitle(canvasRect);
        CreateGrid(canvasRect);
        CreateAxisLabels(canvasRect);
        CreateButtons(canvasRect);
        CreateRawBitsText(canvasRect);

        // 시작할 때는 BitGrid가 전부 0이고 체크 표시도 전부 꺼져 있어서 이미 맞다.
        // IsOn이 구현 전이어도 화면이 뜨도록, 여기서는 2진수 표시만 갱신한다.
        RefreshRawBitsText();
    }

    // ───────── 클릭 → 값 ─────────

    private void OnCellClicked(int cellX, int cellY)
    {
        _bitGrid.Toggle(cellX, cellY);
        RefreshView();
    }

    private void SetDiagonal(bool turnOn)
    {
        for (int diagonalIndex = 0; diagonalIndex < BitGrid.Size; ++diagonalIndex)
        {
            if (turnOn)
            {
                _bitGrid.Set(diagonalIndex, diagonalIndex);
            }
            else
            {
                _bitGrid.Clear(diagonalIndex, diagonalIndex);
            }
        }
    }

    // ───────── 값 → 화면 ─────────

    private void RefreshView()
    {
        // 2진수 표시를 먼저 갱신한다. IsOn에서 예외가 나도 배열 값은 볼 수 있게.
        RefreshRawBitsText();

        for (int cellY = 0; cellY < BitGrid.Size; ++cellY)
        {
            for (int cellX = 0; cellX < BitGrid.Size; ++cellX)
            {
                bool isCellOn = _bitGrid.IsOn(cellX, cellY);

                // isOn = isCellOn 으로 넣으면 onValueChanged가 다시 불려서 OnCellClicked → Toggle이 또 실행된다.
                // 값을 화면에 맞출 때는 알림 없이 넣는다.
                _cellToggles[cellX, cellY].SetIsOnWithoutNotify(isCellOn);
                _cellCheckMarks[cellX, cellY].enabled = isCellOn;
            }
        }
    }

    private void RefreshRawBitsText()
    {
        var rawBitsBuilder = new System.Text.StringBuilder();
        rawBitsBuilder.AppendLine("_bits");
        for (int rowIndex = 0; rowIndex < BitGrid.Size; ++rowIndex)
        {
            int rowValue = _bitGrid.GetRaw(rowIndex);
            string rowBinary = Convert.ToString(rowValue, 2).PadLeft(BitGrid.Size, '0');
            rawBitsBuilder.AppendLine($"[{rowIndex}] {rowBinary}  ({rowValue})");
        }
        _rawBitsText.text = rawBitsBuilder.ToString();
    }

    // ───────── 화면 만들기 ─────────

    private void CreateGrid(RectTransform canvasRect)
    {
        RectTransform gridPanelRect = CreateRect("Grid", canvasRect);
        gridPanelRect.sizeDelta = new Vector2(GetGridSideLength(), GetGridSideLength());
        gridPanelRect.anchoredPosition = _gridCenter;

        // GridLayoutGroup 기본값은 왼쪽 위에서 시작해 가로로 채운다.
        // 그래서 y 바깥 루프, x 안쪽 루프로 만들면 왼쪽 위가 (0, 0), 오른쪽 아래가 (7, 7)이 된다.
        var gridLayout = gridPanelRect.gameObject.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(_cellSize, _cellSize);
        gridLayout.spacing = new Vector2(_cellSpacing, _cellSpacing);
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = BitGrid.Size;

        for (int loopY = 0; loopY < BitGrid.Size; ++loopY)
        {
            for (int loopX = 0; loopX < BitGrid.Size; ++loopX)
            {
                // for 루프 변수 loopX, loopY는 반복마다 새로 생기지 않는다.
                // 람다가 루프 변수를 직접 잡으면 클릭할 때 전부 루프가 끝난 값(8, 8)을 보게 되므로 복사해서 잡는다.
                int cellX = loopX;
                int cellY = loopY;

                RectTransform cellRect = CreateRect($"Cell ({cellX},{cellY})", gridPanelRect);
                var cellBackground = cellRect.gameObject.AddComponent<Image>();
                cellBackground.color = _cellColor;

                RectTransform checkMarkRect = CreateRect("CheckMark", cellRect);
                checkMarkRect.anchorMin = Vector2.zero;
                checkMarkRect.anchorMax = Vector2.one;
                checkMarkRect.offsetMin = new Vector2(8f, 8f);
                checkMarkRect.offsetMax = new Vector2(-8f, -8f);
                var checkMarkImage = checkMarkRect.gameObject.AddComponent<Image>();
                checkMarkImage.color = _checkMarkColor;
                checkMarkImage.raycastTarget = false;
                checkMarkImage.enabled = false;

                var cellToggle = cellRect.gameObject.AddComponent<Toggle>();
                cellToggle.targetGraphic = cellBackground; // 마우스를 올리거나 누를 때 색이 변하는 대상
                cellToggle.graphic = null;                 // 체크 표시는 Toggle이 아니라 RefreshView가 그린다
                cellToggle.SetIsOnWithoutNotify(false);
                // Toggle이 넘겨주는 bool(_)은 쓰지 않는다. 켜짐/꺼짐은 BitGrid 값이 정한다.
                cellToggle.onValueChanged.AddListener(_ => OnCellClicked(cellX, cellY));

                _cellToggles[cellX, cellY] = cellToggle;
                _cellCheckMarks[cellX, cellY] = checkMarkImage;
            }
        }
    }

    // 격자 위쪽에 x 번호, 왼쪽에 y 번호
    private void CreateAxisLabels(RectTransform canvasRect)
    {
        float halfGridSide = GetGridSideLength() * 0.5f;
        for (int labelIndex = 0; labelIndex < BitGrid.Size; ++labelIndex)
        {
            // 격자 가운데에서 labelIndex번 칸 가운데까지의 거리
            float cellCenterOffset = -halfGridSide + _cellSize * 0.5f + labelIndex * (_cellSize + _cellSpacing);

            Text xAxisLabel = CreateText($"x{labelIndex}", canvasRect, _monospaceFont, 22, TextAnchor.MiddleCenter);
            xAxisLabel.rectTransform.sizeDelta = new Vector2(_cellSize, 30f);
            xAxisLabel.rectTransform.anchoredPosition = _gridCenter + new Vector2(cellCenterOffset, halfGridSide + 22f);

            Text yAxisLabel = CreateText($"y{labelIndex}", canvasRect, _monospaceFont, 22, TextAnchor.MiddleCenter);
            yAxisLabel.rectTransform.sizeDelta = new Vector2(50f, _cellSize);
            yAxisLabel.rectTransform.anchoredPosition = _gridCenter + new Vector2(-halfGridSide - 32f, -cellCenterOffset);
        }
    }

    private void CreateButtons(RectTransform canvasRect)
    {
        CreateButton(canvasRect, 0, "전체 지우기  ClearAll", () => _bitGrid.ClearAll());
        CreateButton(canvasRect, 1, "전체 켜기  FillAll", () => _bitGrid.FillAll());
        CreateButton(canvasRect, 2, "전체 뒤집기  InvertAll", () => _bitGrid.InvertAll());
        CreateButton(canvasRect, 3, "대각선 켜기  Set(i, i)", () => SetDiagonal(true));
        CreateButton(canvasRect, 4, "대각선 끄기  Clear(i, i)", () => SetDiagonal(false));
    }

    // buttonOrder: 위에서부터 몇 번째 버튼인지 (0부터)
    private void CreateButton(RectTransform canvasRect, int buttonOrder, string buttonLabel, Action onClickAction)
    {
        RectTransform buttonRect = CreateRect(buttonLabel, canvasRect);
        buttonRect.sizeDelta = new Vector2(320f, 64f);
        buttonRect.anchoredPosition = new Vector2(180f, 180f - buttonOrder * 84f);

        var buttonImage = buttonRect.gameObject.AddComponent<Image>();
        buttonImage.color = _buttonColor;

        var button = buttonRect.gameObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(() =>
        {
            onClickAction();
            RefreshView();
        });

        Text buttonLabelText = CreateText(buttonLabel, buttonRect, _koreanFont, 24, TextAnchor.MiddleCenter);
        buttonLabelText.rectTransform.anchorMin = Vector2.zero;
        buttonLabelText.rectTransform.anchorMax = Vector2.one;
        buttonLabelText.rectTransform.sizeDelta = Vector2.zero;
    }

    private void CreateRawBitsText(RectTransform canvasRect)
    {
        _rawBitsText = CreateText(string.Empty, canvasRect, _monospaceFont, 28, TextAnchor.UpperLeft);
        _rawBitsText.rectTransform.sizeDelta = new Vector2(560f, 420f);
        _rawBitsText.rectTransform.anchoredPosition = new Vector2(660f, 30f);
        _rawBitsText.horizontalOverflow = HorizontalWrapMode.Overflow;
    }

    private void CreateTitle(RectTransform canvasRect)
    {
        Text titleText = CreateText("06. 비트 플래그 격자 — 칸을 누르면 Toggle(x, y)", canvasRect, _koreanFont, 34, TextAnchor.MiddleCenter);
        titleText.rectTransform.sizeDelta = new Vector2(1600f, 60f);
        titleText.rectTransform.anchoredPosition = new Vector2(0f, 440f);
    }

    // ───────── 공용 도우미 ─────────

    // 격자 한 변의 길이 = 칸 8개 + 칸 사이 간격 7개
    private float GetGridSideLength()
    {
        return BitGrid.Size * _cellSize + (BitGrid.Size - 1) * _cellSpacing;
    }

    // primaryOsFontName이 없으면 맑은 고딕 → Arial 순서로 대신 쓴다
    private Font CreateFont(string primaryOsFontName)
    {
        Font createdFont = Font.CreateDynamicFontFromOSFont(new[] { primaryOsFontName, "Malgun Gothic", "Arial" }, 32);
        if (createdFont == null)
        {
            createdFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        return createdFont;
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        // 이 프로젝트는 새 Input System만 켜져 있어서, StandaloneInputModule 대신 InputSystemUIInputModule을 쓴다.
        var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        eventSystemObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    private RectTransform CreateCanvas()
    {
        var canvasObject = new GameObject("BitGridCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        var canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        return (RectTransform)canvasObject.transform;
    }

    // objectName: Hierarchy 창에 보일 GameObject 이름
    private RectTransform CreateRect(string objectName, Transform parentTransform)
    {
        var rectObject = new GameObject(objectName, typeof(RectTransform));
        rectObject.transform.SetParent(parentTransform, false);
        return (RectTransform)rectObject.transform;
    }

    // textContent: 화면에 보일 글자
    private Text CreateText(string textContent, Transform parentTransform, Font textFont, int fontSize, TextAnchor textAlignment)
    {
        RectTransform textRect = CreateRect("Text", parentTransform);
        var uiText = textRect.gameObject.AddComponent<Text>();
        uiText.font = textFont;
        uiText.fontSize = fontSize;
        uiText.alignment = textAlignment;
        uiText.color = Color.white;
        uiText.raycastTarget = false;
        uiText.text = textContent;
        return uiText;
    }
}
