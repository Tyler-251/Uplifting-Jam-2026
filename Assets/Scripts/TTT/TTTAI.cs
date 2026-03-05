using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AI logic for tic-tac-toe placement decisions
/// Strategy priority: Win > Block > Center > Corners > Sides > Random
/// </summary>
public static class TTTAI
{
    /// <summary>
    /// Determines the best AI placement on the board based on strategy priority and confidence
    /// Lower confidence increases likelihood of random placement
    /// </summary>
    /// <param name="board">The current game board</param>
    /// <param name="aiPieceType">The piece type the AI is using (X or O)</param>
    /// <param name="confidence">Confidence level from 0 to 1. 1 = always use strategy, 0 = always random</param>
    /// <param name="rng">Random number generator for determining confidence roll</param>
    /// <returns>A tuple of (row, col) for the AI's placement, or (-1, -1) if no valid move exists</returns>
    public static (int row, int col) GetAIPlacement(TTTBoard board, Piece.PieceType aiPieceType, float confidence)
    {
        // Roll confidence check: if random value exceeds confidence, place randomly
        if (Random.value > confidence)
        {
            return GetRandomEmptySquare(board);
        }

        // Priority 1: Try to win
        var winMove = FindWinningMove(board, aiPieceType);
        if (winMove != (-1, -1))
            return winMove;

        // Priority 2: Block opponent from winning
        var opponentPieceType = aiPieceType == Piece.PieceType.X ? Piece.PieceType.O : Piece.PieceType.X;
        var blockMove = FindWinningMove(board, opponentPieceType);
        if (blockMove != (-1, -1))
            return blockMove;

        // Priority 3: Take center
        if (board.GetSlot(1, 1).IsEmpty)
            return (1, 1);

        // Priority 4: Take a corner
        var cornerMove = GetBestCorner(board);
        if (cornerMove != (-1, -1))
            return cornerMove;

        // Priority 5: Take a side
        var sideMove = GetBestSide(board);
        if (sideMove != (-1, -1))
            return sideMove;

        // Priority 6: Should never reach here if board has empty spaces, but return invalid
        return (-1, -1);
    }

    /// <summary>
    /// Finds a move that would result in a win for the given piece type
    /// </summary>
    private static (int row, int col) FindWinningMove(TTTBoard board, Piece.PieceType pieceType)
    {
        // Test placement at each empty position
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (!board.GetSlot(row, col).IsEmpty)
                    continue;

                // Simulate placing piece
                board.GetSlot(row, col).Piece = new Piece(pieceType);
                bool isWin = board.CheckWinner() == pieceType;
                board.GetSlot(row, col).Clear();

                if (isWin)
                    return (row, col);
            }
        }

        return (-1, -1);
    }

    /// <summary>
    /// Gets the best available corner position
    /// Prefers corners that are closer to the center or defended
    /// </summary>
    private static (int row, int col) GetBestCorner(TTTBoard board)
    {
        var corners = new[] { (0, 0), (0, 2), (2, 0), (2, 2) };
        
        foreach (var (row, col) in corners)
        {
            if (board.GetSlot(row, col).IsEmpty)
                return (row, col);
        }

        return (-1, -1);
    }

    /// <summary>
    /// Gets any available side position (non-corner, non-center)
    /// </summary>
    private static (int row, int col) GetBestSide(TTTBoard board)
    {
        var sides = new[] { (0, 1), (1, 0), (1, 2), (2, 1) };
        
        foreach (var (row, col) in sides)
        {
            if (board.GetSlot(row, col).IsEmpty)
                return (row, col);
        }

        return (-1, -1);
    }

    /// <summary>
    /// Returns a random empty square on the board
    /// </summary>
    private static (int row, int col) GetRandomEmptySquare(TTTBoard board)
    {
        var emptySquares = new List<(int, int)>();

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board.GetSlot(row, col).IsEmpty)
                    emptySquares.Add((row, col));
            }
        }

        if (emptySquares.Count == 0)
            return (-1, -1);

        return emptySquares[Random.Range(0, emptySquares.Count)];
    }
}
