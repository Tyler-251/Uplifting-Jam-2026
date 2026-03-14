using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class TTTManager : MonoBehaviour
{
    public static TTTManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public enum Turn { None, Player, Enemy }
    public float oldManMoveDelay = 1.0f;
    [SerializeField] private bool autoStartFirstMatch = false;
    [SerializeField] private CoinBehavior coinBehavior;
    [SerializeField] private float postMatchPauseTime = 2.0f;
    public bool freezeBetweenMatches = false;
    [SerializeField] private GameObject youThinker;
    [SerializeField] private GameObject oldManThinker;
    [SerializeField] private VertBarBehavior vertBarBehavior;
    [Header("SFX")]
    [SerializeField] private AudioClip xplayed;
    [SerializeField] private AudioClip oplayed;
    [SerializeField] private AudioClip xwon;
    [FormerlySerializedAs("ywon")]
    [SerializeField] private AudioClip owon;
    [SerializeField] private float xplayedVolume = 0.6f;
    [SerializeField] private float oplayedVolume = 0.6f;
    [SerializeField] private float xwonVolume = 1.0f;
    [FormerlySerializedAs("ywonVolume")]
    [SerializeField] private float owonVolume = 1.0f;
    [Header("Pot Display")]
    [SerializeField] private TMP_Text potText;
    [SerializeField] private Image potImage;
    [SerializeField] private Sprite fullestPotSprite;
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

        //Get enemy movespeed upgrades
        float amtToAppendTurnSpeed = 0f;
        foreach (UpgradeSO upgrade in ShopManager.instance.acquiredUpgrades)
        {
            if (upgrade.category == UpgradeCategory.TurnSpeed && upgrade.type == UpgradeType.Additive)
            {
                amtToAppendTurnSpeed += upgrade.value;
            }
        }

        // Get trickiness upgrades
        float amtToAppendTrickiness = 0f;
        foreach (UpgradeSO upgrade in ShopManager.instance.acquiredUpgrades)        {
            if (upgrade.category == UpgradeCategory.Trickiness && upgrade.type == UpgradeType.AdditiveMultiplier)
            {
                amtToAppendTrickiness += upgrade.value / 100f;
            }
        }
        // calculate final trickiness multiplier
        float difficultyHelper = (vertBarBehavior.xWins - vertBarBehavior.oWins) / 100f;

        isEnemyTakingTurn = true;
        yield return new WaitForSeconds(oldManMoveDelay + amtToAppendTurnSpeed + difficultyHelper);

        if (currentTurn != Turn.Enemy)
        {
            isEnemyTakingTurn = false;
            yield break;
        }

        var aiMove = TTTAI.GetAIPlacement(activeBoard, Piece.PieceType.O, .6f - amtToAppendTrickiness);
        if (aiMove != (-1, -1))
        {
            PlaySpotInternal(aiMove.row, aiMove.col, true);
        }

        isEnemyTakingTurn = false;
    }
    public void PlaySpot(int row, int col)
    {
        PlaySpotInternal(row, col, false);
    }

    private void PlaySpotInternal(int row, int col, bool allowEnemyTurn)
    {
        if (!hasGameStarted)
        {
            return;
        }

        // Ignore UI input unless it is the player's turn.
        if (!allowEnemyTurn && currentTurn != Turn.Player)
        {
            return;
        }

        Piece.PieceType playedPiece = currentTurn == Turn.Player ? Piece.PieceType.X : Piece.PieceType.O;

        if (activeBoard.TryPlacePiece(row, col, playedPiece))
        {
            PlaySfx(
                playedPiece == Piece.PieceType.X ? xplayed : oplayed,
                playedPiece == Piece.PieceType.X ? xplayedVolume : oplayedVolume
            );

            if (UltManager.instance != null)
            {
                UltManager.instance.ChargeUlt(0.025f);
            }

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

    public void MultiplyPot(float multiplier)
    {
        if (multiplier <= 0f)
        {
            return;
        }

        potAmount = Mathf.RoundToInt(potAmount * multiplier);
        UpdatePotDisplay();
    }

    private void ResolveWin(Piece.PieceType winner)
    {
        RecordMatchResult(isDraw: false, winner: winner);
        PlaySfx(
            winner == Piece.PieceType.X ? xwon : owon,
            winner == Piece.PieceType.X ? xwonVolume : owonVolume
        );

        if (winner != Piece.PieceType.X)
        {
            potAmount = 0;
            UpdatePotDisplay();
            return;
        }

        if (UltManager.instance != null)
        {
            UltManager.instance.ChargeUlt(0.05f);
        }

        int payout = activeBoard.GetPlacedPieceCount() + potAmount;

        if (ShopManager.instance != null)
        {
            ShopManager.instance.AddXos(payout);
        }

        potAmount = 0;
        UpdatePotDisplay();
    }

    private void PlaySfx(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null || SFXAudioManager.instance == null)
        {
            return;
        }

        SFXAudioManager.instance.PlayClip(clip, volume);
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
        if (potAmount >= 32 && fullestPotSprite != null)
        {
            potImage.sprite = fullestPotSprite;
        }

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
        // Give timeline/dialogue systems a frame to react to match end.
        yield return null;

        yield return new WaitForSeconds(postMatchPauseTime);

        while (freezeBetweenMatches || (DialogueManager.instance != null && DialogueManager.instance.IsDialogueOpen))
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
