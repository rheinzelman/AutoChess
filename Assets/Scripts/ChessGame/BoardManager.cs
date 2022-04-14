using System;
using System.Collections.Generic;
using System.Linq;
using AutoChess;
using AutoChess.Utility.FENHandler;
using ChessGame.ChessPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ChessGame
{
    // Defines a Unity Event that takes a Vector2Int argument
    [Serializable]
    public class PieceEvent : UnityEvent<Vector2Int> { }
    [Serializable]
    public class PieceMoveEvent : UnityEvent<Vector2Int, Vector2Int> { }
    [Serializable]
    public class PieceTryEvent : UnityEvent<BaseChessPiece> { }

    public class BoardManager : MonoBehaviour
    {
        // BoardManager instance that anything can reference
        public static BoardManager Instance;

        // Square information
        public Square[,] Squares;
        public int horizontalSquares = 8;
        public int verticalSquares = 8;

        // Place to store en passant
        public Tuple<Vector2Int, BaseChessPiece> enPassantSquare;

        //These lists hold all pieces on the board based on color
        //the functionality of these lists will be implemented and maintained by Kaleb
        public List<BaseChessPiece> whitePieces = new List<BaseChessPiece>();
        public List<BaseChessPiece> blackPieces = new List<BaseChessPiece>();
        public List<char> graveyard = new List<char>();

        // Kings of both sides
        [SerializeField] private King whiteKing;
        [SerializeField] private King blackKing;

        // End Game Conditions
        private King kingInCheck;

        // Board Managers
        public GameManager gameManager;

        // FEN Utilities
        public char[,] BoardState;
        public FENHandler FenObject;

        // Unity events
        public UnityEvent boardUpdate = new UnityEvent();
        public UnityEvent boardRefresh = new UnityEvent();
        public PieceEvent pieceCreated = new PieceEvent();
        public PieceEvent pieceRemoved = new PieceEvent();
        public PieceMoveEvent pieceMoved = new PieceMoveEvent();
        public PieceTryEvent pieceTryMove = new PieceTryEvent();

        private static readonly Dictionary<char, Type> CharToPieceType = new Dictionary<char, Type> {
            { 'p', typeof(Pawn) },
            { 'n', typeof(Knight) },
            { 'b', typeof(Bishop) },
            { 'r', typeof(Rook) },
            { 'q', typeof(Queen) },
            { 'k', typeof(King) }
        };

        private static readonly Dictionary<Type, char> PieceTypeToChar = new Dictionary<Type, char> {
            { typeof(Pawn), 'p' },
            { typeof(Knight), 'n' },
            { typeof(Bishop), 'b' },
            { typeof(Rook), 'r' },
            { typeof(Queen), 'q' },
            { typeof(King), 'k' }
        };

        [Header("Debug")] 
        [SerializeField] private bool verboseDebug;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;

            ProcessFEN(FENHandler.DEFAULT_FEN);
            
            SetupSquares();
            
            InitializePiecesFromArray(FenObject.GetArray());

            boardRefresh.AddListener(UpdateBoardState);

            verboseDebug = gameManager.verboseDebug;
            
            gameManager.onVerboseDebugChanged.AddListener(SetVerboseDebug);
        }

        private void SetVerboseDebug(bool bEnabled)
        {
            verboseDebug = bEnabled;
        }
        
        private void ProcessFEN(string FENInput)
        {
            FenObject = new FENHandler(FENInput);
            BoardState = FenObject.GetArray();
        }

        // Performs a setup to create all the square objects
        private void SetupSquares()
        {
            // Initialize a 2D array of Squares
            Squares = new Square[horizontalSquares, verticalSquares];

            // Loops through each coordinate in the 2D array and creates a Square object at each x and y
            for (var y = 0; y < horizontalSquares; y++)
            {
                for (var x = 0; x < verticalSquares; x++)
                {
                    // Initializes a new GameObject to hold the square
                    var newObject = new GameObject
                    {
                        name = "Square( " + x + ", " + y + " )",
                        transform =
                        {
                            parent = transform
                        }
                    };
                    
                    // Adds a square to the new GameObject
                    Squares[x, y] = newObject.AddComponent<Square>();

                    var newSquare = Squares[x, y];

                    newSquare.board = this;

                    newSquare.coordinate = new Vector2Int(x, y);
                }
            }
        }

        // Checks if a coordinate is within the board's bounds
        public bool IsValidCoordinate(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < horizontalSquares && pos.y >= 0 && pos.y < verticalSquares;
        }

        // Checks if a piece exists at a given coordinate
        public bool HasPieceAt(Vector2Int pos)
        {
            return IsValidCoordinate(pos) && Squares[pos.x, pos.y].HasPiece();
        }

        // Returns a chess piece at a given coordinate
        public BaseChessPiece GetPieceAt(Vector2Int pos)
        {
            return !IsValidCoordinate(pos) ? null : Squares[pos.x, pos.y].piece;
        }

        // Clears the whitePieces and blackPieces lists and then loads all pieces back into them
        [Button]
        public void GetPieces()
        {
            //Clear lists
            whitePieces.Clear();
            blackPieces.Clear();
            
            //iterate through all squares and find pieces
            //when a piece is found use piece color to add to appropriate list
            foreach (var sq in Squares)
            {
                if (!sq.HasPiece()) continue;

                var piece = GetPieceAt(sq.coordinate);

                switch (piece.pieceColor)
                {
                    case PieceColor.White:
                        whitePieces.Add(GetPieceAt(sq.coordinate));
                        break;
                    case PieceColor.Black:
                        blackPieces.Add(GetPieceAt(sq.coordinate));
                        break;
                    case PieceColor.Unassigned:
                        Debug.LogError("Board Manager Error: " + nameof(piece.GetType) + " at " + 
                                       piece.currentPosition + " has a pieceColor of Unassigned!");
                        break;
                    default:
                        Debug.LogError("Board Manager Error: " + nameof(piece.GetType) + " at " + 
                                       piece.currentPosition + " has an invalid pieceColor!");
                        break;
                }
            }
        }

        public void CheckVictoryConditions()
        {

        }

        // Attempts to move a piece at coordinate 'from' to coordinate 'to' and returns true if successful, false otherwise.
        public bool MovePiece(Vector2Int from, Vector2Int to, MoveEventData moveData)
        {
            var piece = GetPieceAt(from);

            if (verboseDebug)
                Debug.Log("BoardManager: Attempting to move piece from: " + from + ", to: " + to + '.');

            if (!piece)
            {
                Debug.LogError("Board Manager Error: Piece does not exist at " + from + '!');
                return false;
            }

            if (!piece.MoveToPosition(to))
            {
                if (verboseDebug)
                    Debug.Log("Board Manager: " + piece.name + " at " + from + "can not move to" + to + '.');
                
                return false;
            }

            if (verboseDebug)
                Debug.Log("BoardManager: Piece successfully moved from: " + from + ", to: " + to + '.');
            
            boardUpdate.Invoke();
            
            pieceMoved.Invoke(from, to);

            boardRefresh.Invoke();

            return true;
        }

        // Forcefully moves a piece at coordinate 'from' to coordinate 'to'
        private void ForceMovePiece(Vector2Int from, Vector2Int to)
        {
            var piece = GetPieceAt(from);

            piece.ForceMoveToPosition(to);
        }

        // Moves a piece to a specified position, checking if moving to that position would put king in check
        public void TryMovePiece(Vector2Int from, Vector2Int to)
        {
            var toPiece = GetPieceAt(to);
            var fromPiece = GetPieceAt(from);

            if (fromPiece == null) return;

            if (toPiece != null)
                toPiece.square.piece = null;

            ForceMovePiece(from, to);

            ForceBoardUpdate(fromPiece);

            if (CheckForCheck(fromPiece.pieceColor))
                fromPiece.blockedMoves.Add(to);

            ForceMovePiece(to, from);

            if (toPiece != null)
                toPiece.square.piece = toPiece;
        }

        // Using a color, call FindChecks() for the appropriate team
        private bool CheckForCheck(PieceColor color)
        {
            return color == PieceColor.White ? FindChecks(blackPieces, whiteKing) : FindChecks(whitePieces, blackKing);
        }

        // Check each piece on the opposing team to see if it puts the kingToCheck in check
        private bool FindChecks(IEnumerable<BaseChessPiece> pieceList, King kingToCheck)
        {
            return pieceList.SelectMany(piece => piece.legalAttacks.Where(pos => pos == kingToCheck.currentPosition)).Any();
        }

        // Performs a take as a specified position, removing the piece from the game
        public void TakePiece(Vector2Int pos)
        {
            // Debug.Log("Taking piece at: " + pos);
            //
            // if (enPassantSquare != null) Debug.Log("En Passant At: " + enPassantSquare.Item1);

            // Remove piece from piece list
            if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.White)
                whitePieces.Remove(GetPieceAt(pos));
            if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.Black)
                blackPieces.Remove(GetPieceAt(pos));

            // Checks for en passant during a take from a pawn
            if (enPassantSquare != null && enPassantSquare.Item1 == pos)
                RemovePiece(enPassantSquare.Item2.currentPosition);
            else
                RemovePiece(pos);
        }
        
        // Removes a piece by deleting it and its GameObject
        private void RemovePiece(Vector2Int pos)
        {
            var square = Squares[pos.x, pos.y];
            var piece = square.piece;

            if (enPassantSquare != null && piece == enPassantSquare.Item2)
                enPassantSquare = null;

            square.piece = null;

            if (piece == null)
            {
                if (verboseDebug)
                    Debug.LogError("Board Manager Error: Can not remove piece at " + pos + 
                                   "because no piece exists!");
                return;
            }

            Destroy(piece.gameObject);

            pieceRemoved.Invoke(pos);
        }

        // Spawns pieces onto the board from a character on the boardState array
        private void InitializePiecesFromArray(char[,] boardState)
        {
            for (var y = 0; y < horizontalSquares; y++)
                for (var x = 0; x < verticalSquares; x++)
                    InitializePieceFromChar(x, y, boardState[x, y]);
        }

        // Checks the piece type and calls AddPiece with appropriate information
        private void InitializePieceFromChar(int x, int y, char type)
        {
            if (type == '-') return;

            var pieceType = CharToPieceType[char.ToLower(type)];
            var piecePos = new Vector2Int(x, y);
            var color = char.IsUpper(type) ? PieceColor.White : PieceColor.Black;

            AddPiece(pieceType, piecePos, color);
        }

        // Converts a piece at a position to it's fen character representation
        private char ConvertPieceToFen(Vector2Int pos)
        {
            var type = '-';
            var piece = GetPieceAt(pos);

            if (piece == null) return type;

            type = PieceTypeToChar[piece.GetType()];

            if (piece.pieceColor == PieceColor.White)
                type = char.ToUpper(type);

            return type;
        }

        // Initializes a piece from it's GameObject, position and color
        private void InitializePiece(GameObject newPiece, Vector2Int pos, PieceColor color)
        {
            newPiece.transform.parent = Squares[pos.x, pos.y].transform;

            var sq = Squares[pos.x, pos.y];

            sq.piece = newPiece.GetComponent<BaseChessPiece>();

            sq.piece.board = this;

            sq.piece.square = sq;

            sq.piece.pieceColor = color;

            sq.piece.currentPosition = pos;

            AddToLists(sq.piece, color);

            pieceCreated.Invoke(pos);

            boardUpdate.Invoke();
        }

        // Adds a piece to one of the lists depending on it's color
        private void AddToLists(BaseChessPiece piece, PieceColor color)
        {
            if (color == PieceColor.White)
            {
                whitePieces.Add(piece);

                if (piece is King king)
                    whiteKing = king;
            }
            else
            {
                blackPieces.Add(piece);

                if (piece is King king)
                    blackKing = king;
            }
        }

        // Forces all pieces on the board to perform an update and find new legal positions
        // If ignorePiece is set, the piece referenced byy ignorePiece will not receive an update
        private void ForceBoardUpdate(BaseChessPiece ignorePiece = null)
        {
            var allPieces = new List<BaseChessPiece>();
            allPieces.AddRange(whitePieces);
            allPieces.AddRange(blackPieces);

            if (ignorePiece != null) allPieces.Remove(ignorePiece);

            foreach (var p in allPieces)
                p.ForceUpdate();
        }
        
        // Forces all pieces on the board to perform a board refresh to find all legal positions and new blocked tiles
        private void ForceBoardRefresh()
        {
            var allPieces = new List<BaseChessPiece>();
            allPieces.AddRange(whitePieces);
            allPieces.AddRange(blackPieces);

            foreach (var p in allPieces)
                p.ForceRefresh();
            
            boardRefresh.Invoke();
        }
        
        // Updates the 2D board state array with new information
        private void UpdateBoardState()
        {
            var newState = new char[8, 8];

            for (var y = 0; y < verticalSquares; y++)
                for (var x = 0; x < horizontalSquares; x++)
                    newState[x, y] = ConvertPieceToFen(new Vector2Int(x, y));
        }

        // Adds a piece to the board by creating a new GameObject and Initializing it
        [Button]
        public void AddPiece(Type type, Vector2Int pos, PieceColor color)
        {
            if (Squares[pos.x, pos.y].piece != null) return;

            InitializePiece(new GameObject(type.Name, type), pos, color);
        }
    }
}