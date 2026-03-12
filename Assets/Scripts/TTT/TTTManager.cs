using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TTTManager : MonoBehaviour
{
    public enum Turn { None, Player, Enemy }
    public float oldManMoveDelay = 1.0f;
    [SerializeField] private bool autoStartFirstMatch = false;
    [SerializeField] private CoinBehavior coinBehavior;
    [SerializeField] private float postMatchPauseTime = 2.0f;
    public bool freezeBetweenMatches = false;
    [SerializeField] private GameObject youThinker;
    [SerializeField] private GameObject oldManThinker;
    [SerializeField] private VertBarBehavior vertBarBehavior;
    [Header("Pot Display")]
    [SerializeField] private TMP_Text potText;
    [SerializeField] private Image potImage;
    [SerializeField] private Sprite fullPotSprite;
    [SerializeField] private Sprite halfPotSprite;
    [SerializeField] private Sprite emptyPotSprite;

    private int potAmount;


    private TTTBoard activeBoard;
    private Turn currentTurn;
    private bool isEnemyTakingTurn;
    private bool hasGameStarted;
    private Coroutine startGameRoutine;

    void Start()
    {
        activeBoard = new TTTBoard();
        currentTurn = Turn.None;
        TTTRenderer.instance.RefreshBoard(activeBoard);
        UpdatePotDisplay();
        UpdateThinkers();

        if (autoStartFirstMatch)
        {
            StartGame();
        }
    }

    void Update()
    {
        if (!hasGameStarted)
        {
            return;
        }

        if (currentTurn == Turn.Enemy && !isEnemyTakingTurn)
        {
            StartCoroutine(EnemyPlaceChipWithDelay());
        }
    }

    public void StartGame()
    {
        activeBoard.Reset();
        currentTurn = Turn.None;
        hasGameStarted = false;
        isEnemyTakingTurn = false;
        TTTRenderer.instance.RefreshBoard(activeBoard);
        TTTRenderer.instance.HideWinLine();
        UpdateThinkers();

        if (startGameRoutine != null)
        {
            StopCoroutine(startGameRoutine);
        }

        startGameRoutine = StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        if (coinBehavior == null)
        {
            currentTurn = Random.value < 0.5f ? Turn.Player : Turn.Enemy;
            hasGameStarted = true;
            UpdateThinkers();
            startGameRoutine = null;
            yield break;
        }

        bool landed = false;
        Turn landedTurn = Turn.None;
        var firstPiece = Random.value < 0.5f ? Piece.PieceType.X : Piece.PieceType.O;

        coinBehavior.FlipCoinTo(firstPiece == Piece.PieceType.X ? "X" : "O", result =>
        {
            landedTurn = result == Piece.PieceType.X ? Turn.Player : Turn.Enemy;
            landed = true;
        });

        while (!landed)
        {
            yield return null;
        }

        currentTurn = landedTurn;
        hasGameStarted = true;
        UpdateThinkers();
        startGameRoutine = null;
    }

    public IEnumerator EnemyPlaceChipWithDelay()
    {
        if (!hasGameStarted)
        {
            yield break;
        }

        isEnemyTakingTurn = true;
        yield return new WaitForSeconds(oldManMoveDelay);

        if (currentTurn != Turn.Enemy)
        {
            isEnemyTakingTurn = false;
            yield break;
        }

        var aiMove = TTTAI.GetAIPlacement(activeBoard, Piece.PieceType.O, .5f);
        if (aiMove != (-1, -1))
        {
            PlaySpot(aiMove.row, aiMove.col);
        }

        isEnemyTakingTurn = false;
    }
    public void PlaySpot(int row, int col)
    {
        if (!hasGameStarted)
        {
            return;
        }

        if (activeBoard.TryPlacePiece(row, col, currentTurn == Turn.Player ? Piece.PieceType.X : Piece.PieceType.O))
        {
            if (activeBoard.TryGetWinningLine(out var winStart, out var winEnd, out var winner))
            {
                ResolveWin(winner);
                Debug.Log(winner == Piece.PieceType.X ? "Player wins!" : "Enemy wins!");
                TTTRenderer.instance.RefreshBoard(activeBoard);
                TTTRenderer.instance.ShowWinLineAnimated(winStart.x, winStart.y, winEnd.x, winEnd.y, winner);
                EndMatch();
                return;
            }

            if (activeBoard.IsFull())
            {
                ResolveDraw();
                Debug.Log("It's a draw!");
                TTTRenderer.instance.HideWinLine();
                EndMatch();
                return;
            }
            currentTurn = currentTurn == Turn.Player ? Turn.Enemy : Turn.Player;
            UpdateThinkers();
        }
        TTTRenderer.instance.RefreshBoard(activeBoard);
    }

    private void EndMatch()
    {
        hasGameStarted = false;
        UpdateThinkers();
        TTTRenderer.instance.RefreshBoard(activeBoard);
        StartCoroutine(EndMatchRoutine());
    }

    private void ResolveDraw()
    {
        potAmount += activeBoard.GetPlacedPieceCount();
        UpdatePotDisplay();
        RecordMatchResult(isDraw: true, winner: Piece.PieceType.None);
    }

    private void ResolveWin(Piece.PieceType winner)
    {
        RecordMatchResult(isDraw: false, winner: winner);

        if (winner != Piece.PieceType.X)
        {
            potAmount = 0;
            UpdatePotDisplay();
            return;
        }

        int payout = activeBoard.GetPlacedPieceCount() + potAmount;

        if (ShopManager.instance != null)
        {
            ShopManager.instance.AddXos(payout);
        }

        potAmount = 0;
        UpdatePotDisplay();
    }

    private void RecordMatchResult(bool isDraw, Piece.PieceType winner)
    {
        if (vertBarBehavior == null)
        {
            return;
        }

        vertBarBehavior.totalGames++;

        if (isDraw)
        {
            vertBarBehavior.draws++;
        }
        else if (winner == Piece.PieceType.X)
        {
            vertBarBehavior.xWins++;
        }
        else if (winner == Piece.PieceType.O)
        {
            vertBarBehavior.oWins++;
        }

        vertBarBehavior.RenderBar();
    }

    private void UpdatePotDisplay()
    {
        if (potText != null)
        {
            potText.text = potAmount.ToString();
        }

        if (potImage == null)
        {
            return;
        }

        if (potAmount <= 0)
        {
            potImage.sprite = emptyPotSprite;
            return;
        }

        potImage.sprite = potAmount <= 9 ? halfPotSprite : fullPotSprite;
    }

    private void UpdateThinkers()
    {
        bool showPlayerThinker = hasGameStarted && currentTurn == Turn.Player;
        bool showEnemyThinker = hasGameStarted && currentTurn == Turn.Enemy;

        if (youThinker != null)
        {
            youThinker.SetActive(showPlayerThinker);
        }

        if (oldManThinker != null)
        {
            oldManThinker.SetActive(showEnemyThinker);
        }
    }

    private IEnumerator EndMatchRoutine()
    {
        yield return new WaitForSeconds(postMatchPauseTime);

        while (freezeBetweenMatches)
        {
            yield return null;
        }

        StartGame();
    }
    
    // Button-friendly methods for Unity UI
    public void PlaySpot00() => PlaySpot(0, 0);
    public void PlaySpot01() => PlaySpot(0, 1);
    public void PlaySpot02() => PlaySpot(0, 2);
    public void PlaySpot10() => PlaySpot(1, 0);
    public void PlaySpot11() => PlaySpot(1, 1);
    public void PlaySpot12() => PlaySpot(1, 2);
    public void PlaySpot20() => PlaySpot(2, 0);
    public void PlaySpot21() => PlaySpot(2, 1);
    public void PlaySpot22() => PlaySpot(2, 2);
    
    //

}
