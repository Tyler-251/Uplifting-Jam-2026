using UnityEngine;

public class TTTManager : MonoBehaviour
{
    public enum Turn { None, Player, Enemy }
    private TTTBoard activeBoard;
        private Turn currentTurn;
    void Start()
    {
        currentTurn = Random.value < 0.5f ? Turn.Player : Turn.Enemy;
        activeBoard = new TTTBoard();
        TTTRenderer.instance.RefreshBoard(activeBoard);
    }
    void Update()
    {
        if (currentTurn == Turn.Enemy)
        {
            var aiMove = TTTAI.GetAIPlacement(activeBoard, Piece.PieceType.O, .5f);
            if (aiMove != (-1, -1))
            {
                PlaySpot(aiMove.row, aiMove.col);
            }
        }
    }
    public void PlaySpot(int row, int col)
    {
        if (activeBoard.TryPlacePiece(row, col, currentTurn == Turn.Player ? Piece.PieceType.X : Piece.PieceType.O))
        {
            if (activeBoard.CheckWinner() == (currentTurn == Turn.Player ? Piece.PieceType.X : Piece.PieceType.O))
            {
                Debug.Log("Player wins!");
                activeBoard.Reset();
                TTTRenderer.instance.RefreshBoard(activeBoard);
                return;
            } else if (activeBoard.CheckWinner() == (currentTurn == Turn.Player ? Piece.PieceType.O : Piece.PieceType.X))
            {
                Debug.Log("Enemy wins!");
                activeBoard.Reset();
                TTTRenderer.instance.RefreshBoard(activeBoard);
                return;
            } else if (activeBoard.IsFull())
            {
                Debug.Log("It's a draw!");
                activeBoard.Reset();
                TTTRenderer.instance.RefreshBoard(activeBoard);
                return;
            }
            currentTurn = currentTurn == Turn.Player ? Turn.Enemy : Turn.Player;
        }
        TTTRenderer.instance.RefreshBoard(activeBoard);
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
