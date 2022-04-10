using AutoChess.ChessPieces;
using AutoChess.Utility.FENHandler;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoChess.ManagerComponents
{
    // Defines a Unity Event that takes a Vector2Int argument
    [System.Serializable]
    public class PieceEvent : UnityEvent<Vector2Int> { }
    [System.Serializable]
    public class PieceMoveEvent : UnityEvent<Vector2Int, Vector2Int> { }
    [System.Serializable]
    public class PieceTryEvent : UnityEvent<ChessPiece> { }

    public class BoardManager : MonoBehaviour
    {
        // Square information
        public Square[,] squares;
        public int horizontalSquares = 8;
        public int verticalSquares = 8;

        // Place to store en passant
        public Tuple<Vector2Int, ChessPiece> EnPassantSquare;

        //These lists hold all pieces on the board based on color
        //the functionality of these lists will be implemented and maintained by Kaleb
        public List<ChessPiece> WhitePieces = new List<ChessPiece>();
        public List<ChessPiece> BlackPieces = new List<ChessPiece>();
        public List<char> Graveyard = new List<char>();

        // Kings of both sides
        [SerializeField] private King WhiteKing;
        [SerializeField] private King BlackKing;

        // End Game Conditions
        private King kingInCheck;

        // Board Managers
        public GameManager gameManager;
        public Board2D board2D;

        // FEN Utilities
        private string DEFAULT_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public char[,] board_state;
        public FENHandler FENObject = null;

        // Unity events
        public UnityEvent boardUpdate = new UnityEvent();
        public UnityEvent boardRefresh = new UnityEvent();
        public PieceEvent pieceCreated = new PieceEvent();
        public PieceEvent pieceRemoved = new PieceEvent();
        public PieceMoveEvent pieceMoved = new PieceMoveEvent();
        public PieceTryEvent pieceTryMove = new PieceTryEvent();

        public static Dictionary<char, Type> charToPieceType = new Dictionary<char, Type> {
            { 'p', typeof(Pawn) },
            { 'n', typeof(Knight) },
            { 'b', typeof(Bishop) },
            { 'r', typeof(Rook) },
            { 'q', typeof(Queen) },
            { 'k', typeof(King) }
        };

        public static Dictionary<Type, char> pieceTypeToChar = new Dictionary<Type, char> {
            { typeof(Pawn), 'p' },
            { typeof(Knight), 'n' },
            { typeof(Bishop), 'b' },
            { typeof(Rook), 'r' },
            { typeof(Queen), 'q' },
            { typeof(King), 'k' }
        };

        private void Awake()
        {
            ProcessFEN(DEFAULT_FEN);
        }

        void Start()
        {
            SetupSquares();

            gameManager = GameManager.instance;

            InitializePiecesFromArray(FENObject.getArray());

            boardRefresh.AddListener(UpdateBoardState);
        }

        private void ProcessFEN(string FENInput)
        {
            FENObject = new FENHandler(FENInput);
            board_state = FENObject.getArray();
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

        [Button]
        public void GetPieces()
        {
            //Clear lists
            WhitePieces.Clear();
            BlackPieces.Clear();
            //iterate through all squares and find pieces
            //when a piece is found use piece color to add to appropriate list
            foreach (Square sq in squares)
            {
                if (sq.HasPiece() && GetPieceAt(sq.coordinate).pieceColor == PieceColor.White)
                    WhitePieces.Add(GetPieceAt(sq.coordinate));
                else if (sq.HasPiece() && GetPieceAt(sq.coordinate).pieceColor == PieceColor.Black)
                    BlackPieces.Add(GetPieceAt(sq.coordinate));
                else return;
            }
        }

        public void CheckVictoryConditions()
        {

        }

        public bool MovePiece(Vector2Int from, Vector2Int to, string eventDataArgs = null)
        {
            ChessPiece piece = GetPieceAt(from);

            Debug.Log("BoardManager: MovePiece from: " + from + ", to: " + to);

            if (!piece || !piece.MoveToPosition(to)) return false;

            pieceMoved.Invoke(from, to);

            boardRefresh.Invoke();

            return true;
        }

        public void ForceMovePiece(Vector2Int from, Vector2Int to)
        {
            ChessPiece piece = GetPieceAt(from);

            Debug.Log("BoardManager: ForceMovePiece from: " + from + ", to: " + to);

            piece.ForceMoveToPosition(to);
        }

        public void TryMovePiece(Vector2Int from, Vector2Int to)
        {
            ChessPiece toPiece = GetPieceAt(to);
            ChessPiece fromPiece = GetPieceAt(from);

            if (fromPiece == null) return;

            if (toPiece != null)
                toPiece.square.piece = null;

            ForceMovePiece(from, to);

            pieceTryMove.Invoke(fromPiece);

            if (CheckForCheck(fromPiece.pieceColor))
                fromPiece.BlockedMoves.Add(to);

            ForceMovePiece(to, from);

            if (toPiece != null)
                toPiece.square.piece = toPiece;
        }

        private bool CheckForCheck(PieceColor color)
        {
            if (color == PieceColor.White)
                return FindChecks(BlackPieces, WhiteKing);
            else
                return FindChecks(WhitePieces, BlackKing);
        }

        private bool FindChecks(List<ChessPiece> pieceList, King kingToCheck)
        {
            foreach (ChessPiece piece in pieceList)
                foreach (Vector2Int pos in piece.LegalAttacks)
                    if (pos == kingToCheck.currentPosition)
                    {
                        print("Legal Attack from " + piece.name + " at " + pos);
                        return true;
                    }

            return false;
        }

        public void TakePiece(Vector2Int pos)
        {
            Debug.Log("Taking piece at: " + pos);
            if (EnPassantSquare != null) Debug.Log("En Passant At: " + EnPassantSquare.Item1);

            //Remove piece from piece list
            if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.White)
                WhitePieces.Remove(GetPieceAt(pos));
            if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.Black)
                BlackPieces.Remove(GetPieceAt(pos));

            if (EnPassantSquare != null && EnPassantSquare.Item1 == pos)
                RemovePiece(EnPassantSquare.Item2.currentPosition);
            else
                RemovePiece(pos);

            //boardUpdate.Invoke();
        }
        private void RemovePiece(Vector2Int pos)
        {
            Square square = squares[pos.x, pos.y];

            ChessPiece piece = square.piece;

            if (EnPassantSquare != null && piece == EnPassantSquare.Item2)
                EnPassantSquare = null;

            square.piece = null;

            if (piece == null)
            {
                Debug.LogError("Piece does not exist at " + pos + '!');
                return;
            }

            Destroy(piece.gameObject);

            pieceRemoved.Invoke(pos);
        }

        public void InitializePiecesFromArray(char[,] boardState)
        {
            for (int y = 0; y < horizontalSquares; y++)
                for (int x = 0; x < verticalSquares; x++)
                    InitializePieceFromChar(x, y, boardState[x, y]);
        }

        private void InitializePieceFromChar(int x, int y, char type)
        {
            if (type == '-') return;

            Type pieceType = charToPieceType[char.ToLower(type)];
            Vector2Int piecePos = new Vector2Int(x, y);
            PieceColor color = char.IsUpper(type) ? PieceColor.White : PieceColor.Black;

            AddPiece(pieceType, piecePos, color);
        }

        private char ConvertPieceToFen(Vector2Int pos)
        {
            char type = '-';

            ChessPiece piece = GetPieceAt(pos);

            if (piece == null) return type;

            type = pieceTypeToChar[piece.GetType()];

            if (piece.pieceColor == PieceColor.White)
                type = char.ToUpper(type);

            return type;
        }

        private void InitializePiece(GameObject newPiece, Vector2Int pos, PieceColor color)
        {
            newPiece.transform.parent = squares[pos.x, pos.y].transform;

            Square sq = squares[pos.x, pos.y];

            sq.piece = newPiece.GetComponent<ChessPiece>();

            sq.piece.board = this;

            sq.piece.square = sq;

            sq.piece.pieceColor = color;

            sq.piece.currentPosition = pos;

            AddToLists(sq.piece, color);

            pieceCreated.Invoke(pos);

            boardUpdate.Invoke();
        }

        private void AddToLists(ChessPiece piece, PieceColor color)
        {
            if (color == PieceColor.White)
            {
                WhitePieces.Add(piece);

                if (piece is King)
                    WhiteKing = piece as King;
            }
            else
            {
                BlackPieces.Add(piece);

                if (piece is King)
                    BlackKing = piece as King;
            }
        }
        private void UpdateBoardState()
        {
            char[,] newState = new char[8, 8];

            for (int y = 0; y < verticalSquares; y++)
                for (int x = 0; x < horizontalSquares; x++)
                    newState[x, y] = ConvertPieceToFen(new Vector2Int(x, y));
        }

        [Button]
        public void AddPiece(Type type, Vector2Int pos, PieceColor color)
        {
            if (squares[pos.x, pos.y].piece != null) return;

            InitializePiece(new GameObject(type.Name, type), pos, color);
        }
    }
}