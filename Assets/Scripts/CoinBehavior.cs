using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinBehavior : MonoBehaviour
{
    [SerializeField] private Image coinImage;
    [SerializeField] private GameObject coinFlipText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private int spinCount = 5;
    [SerializeField] private float spinDuration = 0.2f;
    [SerializeField] private float minTextScale = 0.7f;
    [SerializeField] private float maxTextScale = 1.15f;
    [SerializeField] private float maxCoinPerspectiveScale = 1.3f;

    private Coroutine flipRoutine;
    private Vector3 initialTextScale = Vector3.one;
    private bool hasCachedInitialTextScale;
    private Color initialTextColor = Color.white;
    private bool hasCachedInitialTextColor;

    private void Awake()
    {
        CacheInitialTextScale();
        CacheInitialTextColor();
    }

    public Piece.PieceType FlipCoin()
    {
        return FlipCoin(null);
    }

    public Piece.PieceType FlipCoin(Action<Piece.PieceType> onLanded)
    {
        Piece.PieceType result = Random.value < 0.5f ? Piece.PieceType.X : Piece.PieceType.O;
        StartFlip(result, onLanded);
        return result;
    }

    public void FlipCoinTo(string result)
    {
        FlipCoinTo(result, null);
    }

    public void FlipCoinTo(string result, Action<Piece.PieceType> onLanded)
    {
        Piece.PieceType finalResult;
        switch (result.Trim().ToUpperInvariant())
        {
            case "X":
                finalResult = Piece.PieceType.X;
                break;
            case "O":
                finalResult = Piece.PieceType.O;
                break;
            default:
                finalResult = Random.value < 0.5f ? Piece.PieceType.X : Piece.PieceType.O;
                break;
        }

        StartFlip(finalResult, onLanded);
    }

    private void StartFlip(Piece.PieceType result, Action<Piece.PieceType> onLanded)
    {
        CacheInitialTextScale();

        if (flipRoutine != null)
        {
            StopCoroutine(flipRoutine);
        }

        ResetTextScale();

        flipRoutine = StartCoroutine(ShowCoinFlipResult(result, onLanded));
    }

    private IEnumerator ShowCoinFlipResult(Piece.PieceType result, Action<Piece.PieceType> onLanded)
    {
        coinFlipText.SetActive(true);

        var rect = coinImage.rectTransform;
        Vector3 baseScale = rect.localScale;
        Piece.PieceType oppositeResult = GetOppositeResult(result);
        float maxAllowedScale = Mathf.Min(maxTextScale, 1f);

        UpdateResultText(result, maxAllowedScale);

        float totalDuration = Mathf.Max(1, spinCount) * spinDuration;
        float globalElapsed = 0f;

        for (int i = 0; i < Mathf.Max(1, spinCount); i++)
        {
            float elapsed = 0f;
            while (elapsed < spinDuration)
            {
                float dt = Time.deltaTime;
                elapsed += dt;
                globalElapsed += dt;

                float t = Mathf.Clamp01(elapsed / spinDuration);
                float globalT = Mathf.Clamp01(globalElapsed / totalDuration);

                // Quadratic arc: peaks at globalT=0.5 (spin 2.5), giving gravity feel
                float arcScale = 1f + (maxCoinPerspectiveScale - 1f) * 4f * globalT * (1f - globalT);

                float flip = Mathf.Cos(t * Mathf.PI * 2f);
                rect.localScale = new Vector3(baseScale.x * flip, baseScale.y * arcScale, baseScale.z * arcScale);

                Piece.PieceType visibleFace = flip >= 0f ? result : oppositeResult;
                float textScale = Mathf.Lerp(Mathf.Min(minTextScale, 1f), maxAllowedScale, Mathf.Abs(flip));
                UpdateResultText(visibleFace, textScale);
                yield return null;
            }
        }

        rect.localScale = baseScale;

        UpdateResultText(result, 1f);

        onLanded?.Invoke(result);
        flipRoutine = null;
    }

    private void UpdateResultText(Piece.PieceType visibleFace, float scaleMultiplier)
    {
        if (resultText == null)
        {
            return;
        }

        resultText.text = visibleFace == Piece.PieceType.X ? "X" : "O";
        resultText.rectTransform.localScale = initialTextScale * scaleMultiplier;
        resultText.color = visibleFace == Piece.PieceType.X
            ? new Color(Mathf.Min(1f, initialTextColor.r + 0.25f), initialTextColor.g * 0.75f, initialTextColor.b * 0.75f, initialTextColor.a)
            : new Color(initialTextColor.r * 0.75f, initialTextColor.g * 0.75f, Mathf.Min(1f, initialTextColor.b + 0.25f), initialTextColor.a);
    }

    private void CacheInitialTextColor()
    {
        if (resultText == null || hasCachedInitialTextColor)
        {
            return;
        }

        initialTextColor = resultText.color;
        hasCachedInitialTextColor = true;
    }

    private void CacheInitialTextScale()
    {
        if (resultText == null || hasCachedInitialTextScale)
        {
            return;
        }

        initialTextScale = resultText.rectTransform.localScale;
        hasCachedInitialTextScale = true;
    }

    private void ResetTextScale()
    {
        if (resultText == null)
        {
            return;
        }

        resultText.rectTransform.localScale = initialTextScale;
    }

    private Piece.PieceType GetOppositeResult(Piece.PieceType result)
    {
        return result == Piece.PieceType.X ? Piece.PieceType.O : Piece.PieceType.X;
    }

}
