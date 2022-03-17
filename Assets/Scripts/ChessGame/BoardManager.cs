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

    //These lists hold all pieces on the board based on color
    //the functionality of these lists will be implemented and maintained by Kaleb
    public List<ChessPiece> WhitePieces = new List<ChessPiece>();
    public List<ChessPiece> BlackPieces = new List<ChessPiece>();

    [SerializeField]
    private King WhiteKing;
    [SerializeField]
    private King BlackKing;

    // Board Managers!
    public ChessManager chessManager;
    public Board2D board2D; 

    public UnityEvent boardUpdate = new UnityEvent();

    void Start()
    {
        SetupSquares();

        InitializePiecesFromArray(chessManager.board_state);
    }

    [Button]
    public void GetPieces()
    {
        //Clear lists
        WhitePieces.Clear();
        BlackPieces.Clear();
        //iterate through all squares and find pieces
        //when a piece is found use piece color to add to appropriate list
        foreach(Square sq in squares)
        {
            if (sq.HasPiece() && GetPieceAt(sq.coordinate).pieceColor == PieceColor.White)
                WhitePieces.Add(GetPieceAt(sq.coordinate));
            else if (sq.HasPiece() && GetPieceAt(sq.coordinate).pieceColor == PieceColor.Black)
                BlackPieces.Add(GetPieceAt(sq.coordinate));
            else return;
        }
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
        //Remove piece from piece list
        if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.White)
            WhitePieces.Remove(GetPieceAt(pos));
        if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.Black)
            BlackPieces.Remove(GetPieceAt(pos));

        if (EnPassantSquares.ContainsKey(pos))
            RemovePiece(EnPassantSquares[pos].currentPosition);
        else
            RemovePiece(pos);

    }

    public void InitializePiecesFromArray(char[,] boardState)
    {
        for (int y = 0; y < horizontalSquares; y++)
            for (int x = 0; x < verticalSquares; x++)
                InitializePieceFromChar(x, y, boardState[x, y]);
    }

    private void InitializePieceFromChar(int x, int y, char type)
    {
        Vector2Int piecePos = new Vector2Int(x, y);   

        if (type == 'p')
            AddPawn(piecePos, PieceColor.Black);

        if (type == 'P')
            AddPawn(piecePos, PieceColor.White);

        if (type == 'n')
            AddKnight(piecePos, PieceColor.Black);

        if (type == 'N')
            AddKnight(piecePos, PieceColor.White);

        if (type == 'b')
            AddBishop(piecePos, PieceColor.Black);

        if (type == 'B')
            AddBishop(piecePos, PieceColor.White);

        if (type == 'r')
            AddRook(piecePos, PieceColor.Black);

        if (type == 'R')
            AddRook(piecePos, PieceColor.White);

        if (type == 'q')
            AddQueen(piecePos, PieceColor.Black);

        if (type == 'Q')
            AddQueen(piecePos, PieceColor.White);

        if (type == 'k')
            AddKing(piecePos, PieceColor.Black);

        if (type == 'K')
            AddKing(piecePos, PieceColor.White);
    }

    public bool MovePiece(Vector2Int from, Vector2Int to)
    {
        ChessPiece piece = GetPieceAt(from);

        if (!piece || !piece.MoveToPosition(to)) return false;

        print("Moving a " + piece.name + ": " + from + " -> " + to);

        return true;
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

        if (color == PieceColor.White)
        {
            WhitePieces.Add(newPiece.GetComponent<ChessPiece>());

            if (sq.piece is King)
                WhiteKing = sq.piece as King;
        }
        else
        {
            BlackPieces.Add(newPiece.GetComponent<ChessPiece>());

            if (sq.piece is King)
                BlackKing = sq.piece as King;
        }

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
