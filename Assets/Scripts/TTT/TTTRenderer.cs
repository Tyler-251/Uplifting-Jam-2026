using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TTTRenderer : MonoBehaviour
{
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Sprite xxSprite;
    [SerializeField] private Sprite ooSprite;
    [SerializeField] private VertBarBehavior vertBarBehavior;
    [SerializeField] private Color oColor = Color.blue;
    [SerializeField] private Color xColor = Color.red;
    [SerializeField] private List<Image> slotImages; // 00, 01, 02, 10, 11, 12, 20, 21, 22
    [SerializeField] private RectTransform winLineRect;
    [SerializeField] private float winLineDrawTime = 0.2f;
    [SerializeField] private float winLineThickness = 12f;
    [SerializeField] private float winLineOvershoot = 124f;
    public static TTTRenderer instance;
    private Coroutine winLineRoutine;
    private Image winLineImage;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        if (winLineRect != null)
        {
            winLineImage = winLineRect.GetComponent<Image>();
        }

        HideWinLine();
    }

    public void RefreshBoard(TTTBoard board) {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var slot = board.GetSlot(row, col);
                var imageIndex = row * 3 + col;
                var image = slotImages[imageIndex];
                
                switch (slot.Piece.Type)
                {
                    case Piece.PieceType.X:
                        image.sprite = xSprite;
                        image.color = xColor;
                        break;
                    case Piece.PieceType.O:
                        image.sprite = oSprite;
                        image.color = oColor;
                        break;
                    case Piece.PieceType.XX:
                        image.sprite = xxSprite;
                        image.color = xColor;
                        break;
                    case Piece.PieceType.OO:
                        image.sprite = ooSprite;
                        image.color = oColor;
                        break;
                    default:
                        image.sprite = null;
                        image.color = new Color(1, 1, 1, 0.0001f); // Nearly invisible but still accepts raycasts
                        break;
                }
            }
        }
        vertBarBehavior.RenderBar();
    }

    public void ShowWinLineAnimated(int startRow, int startCol, int endRow, int endCol, Piece.PieceType winner)
    {
        if (winLineRect == null)
        {
            return;
        }

        if (winLineRoutine != null)
        {
            StopCoroutine(winLineRoutine);
        }

        if (winLineImage != null)
        {
            winLineImage.color = winner == Piece.PieceType.X ? xColor : oColor;
        }

        winLineRoutine = StartCoroutine(AnimateWinLineRoutine(startRow, startCol, endRow, endCol));
    }

    public void HideWinLine()
    {
        if (winLineRoutine != null)
        {
            StopCoroutine(winLineRoutine);
            winLineRoutine = null;
        }

        if (winLineRect != null)
        {
            winLineRect.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateWinLineRoutine(int startRow, int startCol, int endRow, int endCol)
    {
        if (!TryGetSlotCenterInLineParent(startRow, startCol, out var from) ||
            !TryGetSlotCenterInLineParent(endRow, endCol, out var to))
        {
            yield break;
        }

        winLineRect.gameObject.SetActive(true);

        var delta = to - from;
        if (delta.sqrMagnitude <= Mathf.Epsilon)
        {
            yield break;
        }

        var direction = delta.normalized;
        from -= direction * winLineOvershoot;
        to += direction * winLineOvershoot;
        delta = to - from;
        var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        var midpoint = (from + to) * 0.5f;
        var length = delta.magnitude;

        winLineRect.anchoredPosition = midpoint;
        winLineRect.localEulerAngles = new Vector3(0f, 0f, angle);

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, winLineDrawTime);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentLength = Mathf.Lerp(0f, length, t);
            winLineRect.sizeDelta = new Vector2(currentLength, winLineThickness);
            yield return null;
        }

        winLineRect.sizeDelta = new Vector2(length, winLineThickness);
        winLineRoutine = null;
    }

    private bool TryGetSlotCenterInLineParent(int row, int col, out Vector2 localPoint)
    {
        localPoint = Vector2.zero;
        var index = row * 3 + col;
        if (index < 0 || index >= slotImages.Count || slotImages[index] == null || winLineRect == null || winLineRect.parent == null)
        {
            return false;
        }

        RectTransform slotRect = slotImages[index].rectTransform;
        RectTransform parentRect = winLineRect.parent as RectTransform;
        Vector3 worldCenter = slotRect.TransformPoint(slotRect.rect.center);
        Camera cam = slotRect.GetComponentInParent<Canvas>()?.renderMode == RenderMode.ScreenSpaceOverlay ? null : slotRect.GetComponentInParent<Canvas>()?.worldCamera;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldCenter);

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, cam, out localPoint);
    }
}
