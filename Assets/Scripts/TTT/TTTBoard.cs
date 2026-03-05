using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a game piece in tic-tac-toe
/// </summary>
public class Piece
{
    public enum PieceType { None, X, O, XX, OO }
    
    public PieceType Type { get; set; }
    
    public Piece(PieceType type = PieceType.None)
    {
        Type = type;
    }
    
    public bool IsEmpty => Type == PieceType.None;
    
    public override string ToString() => Type.ToString();
}

/// <summary>
/// Represents a single slot on the board where a piece can be placed
/// </summary>
public class GameSlot
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    private Piece piece;
    
    public Piece Piece
    {
        get => piece;
        set => piece = value;
    }
    
    public GameSlot(int row, int col)
    {
        Row = row;
        Col = col;
        piece = new Piece(Piece.PieceType.None);
    }
    
    public bool IsEmpty => piece.IsEmpty;
    
    public void Clear() => piece = new Piece(Piece.PieceType.None);
    
    public override string ToString() => $"[{Row},{Col}]: {piece}";
}

/// <summary>
/// Manages the 3x3 tic-tac-toe board
/// </summary>
public class TTTBoard
{
    private const int BOARD_SIZE = 3;
    private GameSlot[,] board;
    
    public TTTBoard()
    {
        board = new GameSlot[BOARD_SIZE, BOARD_SIZE];
        InitializeBoard();
    }
    
    private void InitializeBoard()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                board[row, col] = new GameSlot(row, col);
            }
        }
    }
    
    public bool TryPlacePiece(int row, int col, Piece.PieceType pieceType)
    {
        if (!IsValidPosition(row, col))
            return false;
        
        if (!board[row, col].IsEmpty)
            return false;
        
        board[row, col].Piece = new Piece(pieceType);
        return true;
    }
    
    public GameSlot GetSlot(int row, int col)
    {
        return IsValidPosition(row, col) ? board[row, col] : null;
    }
    
    public Piece GetPiece(int row, int col)
    {
        return IsValidPosition(row, col) ? board[row, col].Piece : null;
    }
    
    private bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < BOARD_SIZE && col >= 0 && col < BOARD_SIZE;
    }
    
    public void Reset()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                board[row, col].Clear();
            }
        }
    }
    
    public Piece.PieceType CheckWinner()
    {
        // Check rows
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            if (CheckLine(board[row, 0].Piece.Type, board[row, 1].Piece.Type, board[row, 2].Piece.Type))
                return board[row, 0].Piece.Type;
        }
        
        // Check columns
        for (int col = 0; col < BOARD_SIZE; col++)
        {
            if (CheckLine(board[0, col].Piece.Type, board[1, col].Piece.Type, board[2, col].Piece.Type))
                return board[0, col].Piece.Type;
        }
        
        // Check diagonals
        if (CheckLine(board[0, 0].Piece.Type, board[1, 1].Piece.Type, board[2, 2].Piece.Type))
            return board[0, 0].Piece.Type;
        
        if (CheckLine(board[0, 2].Piece.Type, board[1, 1].Piece.Type, board[2, 0].Piece.Type))
            return board[0, 2].Piece.Type;
        
        return Piece.PieceType.None;
    }
    
    private bool CheckLine(Piece.PieceType a, Piece.PieceType b, Piece.PieceType c)
    {
        return a != Piece.PieceType.None && a == b && b == c;
    }
    
    public bool IsFull()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col].IsEmpty)
                    return false;
            }
        }
        return true;
    }
    
    public void PrintBoard()
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            string line = "";
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                line += board[row, col].Piece.Type + " ";
            }
            Debug.Log(line);
        }
    }
}