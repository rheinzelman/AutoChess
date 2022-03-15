using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardManager : MonoBehaviour
{
    public Square[,] squares;
    public int horizontalSquares = 8;
    public int verticalSquares = 8;

    public Dictionary<Vector2Int, ChessPiece> EnPassantSquares = new Dictionary<Vector2Int, ChessPiece>();

    public UnityEvent boardUpdate = new UnityEvent();

    void Start()
    {
        SetupSquares();
    }

    private void SetupSquares()
    {
        squares = new Square[horizontalSquares, verticalSquares];

        for (int j = 0; j < horizontalSquares; j++)
        {
            for (int i = 0; i < verticalSquares; i++)
            {
                GameObject newObject = new GameObject();

                newObject.name = "Square( " + i + ", " + j + " )";

                newObject.transform.parent = transform;

                squares[i, j] = newObject.AddComponent<Square>();

                Square newSquare = squares[i, j];

                newSquare.board = this;

                newSquare.coordinate = new Vector2Int(i, j);
            }
        }
    }

    public bool IsValidCoordinate(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < horizontalSquares && pos.y >= 0 && pos.y < verticalSquares;
    }

    public bool HasPieceAt(Vector2Int pos)
    {
        return IsValidCoordinate(pos) && squares[pos.x, pos.y].HasPiece();
    }

    public ChessPiece GetPieceAt(Vector2Int pos)
    {
        if (!IsValidCoordinate(pos)) return null;

        return squares[pos.x, pos.y].piece;
    }

    public void TakePiece(Vector2Int pos)
    {
        if (EnPassantSquares.ContainsKey(pos))
            RemovePiece(EnPassantSquares[pos].currentPosition);
        else
            RemovePiece(pos);
    }

    private void RemovePiece(Vector2Int pos)
    {
        Square square = squares[pos.x, pos.y];

        ChessPiece piece = square.piece;

        square.piece = null;

        Destroy(piece.gameObject);
    }

    private void InitializePiece(GameObject newPiece, Vector2Int pos, PieceColor color)
    {
        newPiece.transform.parent = squares[pos.x, pos.y].transform;

        Square sq = squares[pos.x, pos.y];

        sq.piece = newPiece.GetComponent<ChessPiece>();//newPiece.AddComponent<Pawn>();

        sq.piece.board = this;

        sq.piece.square = sq;

        sq.piece.pieceColor = color;

        sq.piece.currentPosition = pos;

        boardUpdate.Invoke();
    }

    [Button]
    public void AddPawn(Vector2Int pos, PieceColor color)
    {
        if (squares[pos.x, pos.y].piece != null) return;

        InitializePiece(new GameObject("Pawn", typeof(Pawn)), pos, color);
    }

    [Button]
    public void AddBishop(Vector2Int pos, PieceColor color)
    {
        if (squares[pos.x, pos.y].piece != null) return;

        InitializePiece(new GameObject("Bishop", typeof(Bishop)), pos, color);
    }

    [Button]
    public void AddKnight(Vector2Int pos, PieceColor color)
    {
        if (squares[pos.x, pos.y].piece != null) return;

        InitializePiece(new GameObject("Knight", typeof(Knight)), pos, color);
    }

    [Button]
    public void AddRook(Vector2Int pos, PieceColor color)
    {
        if (squares[pos.x, pos.y].piece != null) return;

        InitializePiece(new GameObject("Rook", typeof(Rook)), pos, color);
    }

    [Button]
    public void AddKing(Vector2Int pos, PieceColor color)
    {
        if (squares[pos.x, pos.y].piece != null) return;

        InitializePiece(new GameObject("King", typeof(King)), pos, color);
    }

    [Button]
    public void AddQueen(Vector2Int pos, PieceColor color)
    {
        if (squares[pos.x, pos.y].piece != null) return;

        InitializePiece(new GameObject("Queen", typeof(Queen)), pos, color);
    }
}
