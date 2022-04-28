using System;
using System.Collections.Generic;
using System.Linq;
using ChessGame.ChessPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace ChessGame
{
    // Defines a Unity Event that takes a Vector2Int argument
    [Serializable]
    public class PieceEvent : UnityEvent<Vector2Int> { }
    [Serializable]
    public class PieceCreateEvent : UnityEvent<Vector2Int, char> { }
    [Serializable]
    public class PieceMoveEvent : UnityEvent<Vector2Int, Vector2Int> { }
    [Serializable]
    public class PieceTryEvent : UnityEvent<BaseChessPiece> { }

    public class BoardManager : MonoBehaviour
    {
        // BoardManager instance that anything can reference
        public static BoardManager Instance;

        // Square information
        [Header("Squares")] public Square[,] Squares;
        public int horizontalSquares = 8;
        public int verticalSquares = 8;

        [Header("Game Information")]
        // Place to store en passant
        public Tuple<Vector2Int, BaseChessPiece> enPassantSquare;

        public string castlingRights = "KQkq";

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
        public char[,] BoardState = new char[8, 8];

        // Unity events
        public UnityEvent boardUpdate = new UnityEvent();
        public UnityEvent boardRefresh = new UnityEvent();
        public PieceCreateEvent pieceCreated = new PieceCreateEvent();
        public PieceCreateEvent pawnPromoted = new PieceCreateEvent();
        public PieceEvent pieceRemoved = new PieceEvent();
        public PieceEvent pieceTaken = new PieceEvent();
        public PieceMoveEvent pieceMoved = new PieceMoveEvent();
        public PieceTryEvent pieceTryMove = new PieceTryEvent();

        private static readonly Dictionary<char, Type> CharToPieceType = new Dictionary<char, Type>
        {
            {'p', typeof(Pawn)},
            {'n', typeof(Knight)},
            {'b', typeof(Bishop)},
            {'r', typeof(Rook)},
            {'q', typeof(Queen)},
            {'k', typeof(King)}
        };

        private static readonly Dictionary<Type, char> PieceTypeToChar = new Dictionary<Type, char>
        {
            {typeof(Pawn), 'p'},
            {typeof(Knight), 'n'},
            {typeof(Bishop), 'b'},
            {typeof(Rook), 'r'},
            {typeof(Queen), 'q'},
            {typeof(King), 'k'}
        };

        [Header("Debug")] [SerializeField] private bool verboseDebug;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
            
            SetupSquares();

            InitializeBoardState();

            InstantiatePiecesFromArray(NotationsHandler.GetArray());

            //boardRefresh.AddListener(UpdateBoardStateArray);

            verboseDebug = gameManager.verboseDebug;

            gameManager.onVerboseDebugChanged.AddListener(SetVerboseDebug);
        }

        private void SetVerboseDebug(bool bEnabled)
        {
            verboseDebug = bEnabled;
        }

        // Creates the board state from the default FEN
        private void InitializeBoardState(string FEN = "")
        {
            NotationsHandler.ProcessFEN(FEN);
            BoardState = NotationsHandler.GetArray();
        }
        
        // Spawns pieces onto the board from a character on the boardState array
        private void InstantiatePiecesFromArray(char[,] boardState)
        {
            for (var y = 0; y < horizontalSquares; y++)
                for (var x = 0; x < verticalSquares; x++)
                    InstantiatePieceFromChar(x, y, boardState[x, y]);
            
            InitializeAllPieces();
        }

        // Checks the piece type and calls AddPiece with appropriate information
        private void InstantiatePieceFromChar(int x, int y, char type)
        {
            if (type == '-') return;

            var pieceType = CharToPieceType[char.ToLower(type)];
            var piecePos = new Vector2Int(x, y);
            var color = char.IsUpper(type) ? PieceColor.White : PieceColor.Black;

            AddPiece(pieceType, piecePos, color);
        }

        // Initializes a piece from it's GameObject, position and color
        private void IntantiatePiece(GameObject newPiece, Vector2Int pos, PieceColor color)
        {
            newPiece.transform.parent = Squares[pos.x, pos.y].transform;

            var sq = Squares[pos.x, pos.y];

            sq.piece = newPiece.GetComponent<BaseChessPiece>();

            sq.piece.board = this;

            sq.piece.square = sq;

            sq.piece.pieceColor = color;

            sq.piece.currentPosition = pos;

            AddToLists(sq.piece, color);

            pieceCreated.Invoke(pos, GetPieceChar(pos));

            ForceBoardUpdate();
        }
        
        // Calls the initialize function in all pieces
        private void InitializeAllPieces()
        {
            var allPieces = whitePieces.Concat(blackPieces).ToList();
            allPieces.ForEach(p => p.Initialize());
        }

        private void InitializePiece(BaseChessPiece p)
        {
            p.Initialize();
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
            return IsValidCoordinate(pos.x, pos.y);
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < horizontalSquares && y >= 0 && y < verticalSquares;
        }

        // Checks if a piece exists at a given coordinate
        public bool HasPieceAt(Vector2Int pos)
        {
            return HasPieceAt(pos.x, pos.y);
        }

        public bool HasPieceAt(int x, int y)
        {
            return IsValidCoordinate(x, y) && Squares[x, y].HasPiece();
        }

        // Returns a chess piece at a given coordinate
        public BaseChessPiece GetPieceAt(Vector2Int pos)
        {
            return GetPieceAt(pos.x, pos.y);
        }

        public BaseChessPiece GetPieceAt(int x, int y)
        {
            return !IsValidCoordinate(x, y) ? null : Squares[x, y].piece;
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

        // Promotes pawns when they reach the end
        public void Promote(Pawn pawn)
        {
            var pos = pawn.currentPosition;
            var color = pawn.pieceColor;

            RemovePiece(pos);
            AddPiece(typeof(Queen), pos, color);
            
            pawnPromoted.Invoke(pos, GetPieceChar(pos));
        }

        // Attempts to move a piece at coordinate 'from' to coordinate 'to' and returns true if successful, false otherwise.
        public (bool, string) MovePiece(Vector2Int from, Vector2Int to)
        {
            var piece = GetPieceAt(from);

            var pieceHasMoved = piece.MoveToPosition(to);

            var args = "";

            #region MovePiece Debug

            if (verboseDebug)
                Debug.Log("BoardManager: Attempting to move piece from: " + from + ", to: " + to + '.');

            if (!piece)
            {
                Debug.LogError("Board Manager Error: Piece does not exist at " + from + '!');
                return (false, "");
            }

            if (!pieceHasMoved)
            {
                if (verboseDebug)
                    Debug.Log("Board Manager: " + piece.name + " at " + from + "can not move to" + to + '.');

                return (false, "");
            }

            if (verboseDebug)
                Debug.Log("BoardManager: Piece successfully moved from: " + from + ", to: " + to + '.');

            #endregion

            switch (piece)
            {
                case Pawn p:
                    gameManager.halfMoveClock = 0;
                    break;
                case Knight p:
                    args += "n, ";
                    break;
            }

            print(NotationsHandler.CoordinateToUCI(from) + NotationsHandler.CoordinateToUCI(to));

            ForceBoardUpdate();

            // boardUpdate.Invoke();
            //
            pieceMoved.Invoke(from, to);
            //
            // boardRefresh.Invoke();

            return (true, args);
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

            if (fromPiece is null) return;

            if (!(toPiece is null))
                toPiece.square.piece = null;

            ForceMovePiece(from, to);

            ForceBoardUpdate(fromPiece);

            if (CheckForCheck(fromPiece.pieceColor))
                fromPiece.blockedMoves.Add(to);

            ForceMovePiece(to, from);

            if (!(toPiece is null))
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
            return pieceList.SelectMany(piece => piece.legalAttacks.Where(pos => pos == kingToCheck.currentPosition))
                .Any();
        }

        // Performs a take as a specified position, removing the piece from the game
        public void TakePiece(BaseChessPiece piece)
        {
            if (piece == null) return;
            
            if (piece && piece.pieceColor == PieceColor.White)
                whitePieces.Remove(piece);
            
            if (piece && piece.pieceColor == PieceColor.Black)
                blackPieces.Remove(piece);

            // Checks for en passant during a take from a pawn
            // if (enPassantSquare != null && enPassantSquare.Item2 == piece)
            //     RemovePiece(enPassantSquare.Item2.currentPosition);
            // else
            //     RemovePiece(piece);
            
            RemovePiece(piece);

            gameManager.halfMoveClock = 0;
        }
        
        public void TakePiece(Vector2Int pos)
        {
            if (enPassantSquare != null && pos == enPassantSquare.Item1)
                TakePiece(enPassantSquare.Item2);
            else
                TakePiece(GetPieceAt(pos));

            // Debug.Log("Taking piece at: " + pos);
            //
            // if (enPassantSquare != null) Debug.Log("En Passant At: " + enPassantSquare.Item1);

            // Remove piece from piece list
            // if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.White)
            //     whitePieces.Remove(GetPieceAt(pos));
            // if (GetPieceAt(pos) && GetPieceAt(pos).pieceColor == PieceColor.Black)
            //     blackPieces.Remove(GetPieceAt(pos));
            //
            // // Checks for en passant during a take from a pawn
            // if (enPassantSquare != null && enPassantSquare.Item1 == pos)
            //     RemovePiece(enPassantSquare.Item2.currentPosition);
            // else
            //     RemovePiece(pos);
            //
            // gameManager.halfMoveClock = 0;
        }

        public void PerformCastle(Rook rook, Vector2Int pos)
        {
            ForceMovePiece(rook.currentPosition, pos);
            ForceUpdatePiece(rook);
        }

        // Removes a piece by deleting it and its GameObject
        private void RemovePiece(BaseChessPiece piece)
        {
            // var square = Squares[pos.x, pos.y];
            // var piece = square.piece;

            if (piece == null) return;

            var pos = piece.currentPosition;

            if (enPassantSquare != null && piece == enPassantSquare.Item2)
                enPassantSquare = null;

            piece.square.piece = null;

            Destroy(piece.gameObject);

            pieceRemoved.Invoke(pos);
        }
        
        private void RemovePiece(Vector2Int pos)
        {
            RemovePiece(Squares[pos.x, pos.y].piece);

            // var square = Squares[pos.x, pos.y];
            // var piece = square.piece;
            //
            // if (enPassantSquare != null && piece == enPassantSquare.Item2)
            //     enPassantSquare = null;
            //
            // square.piece = null;
            //
            // if (piece is null)
            // {
            //     if (verboseDebug)
            //         Debug.LogError("Board Manager Error: Can not remove piece at " + pos +
            //                        "because no piece exists!");
            //     return;
            // }
            //
            // Destroy(piece.gameObject);
            //
            // pieceRemoved.Invoke(pos);
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

            if (!(ignorePiece is null)) allPieces.Remove(ignorePiece);

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

            UpdateBoardStateArray();
            // boardRefresh.Invoke();
        }
        
        // Forces a singular piece to update
        private void ForceUpdatePiece(BaseChessPiece piece)
        {
            piece.ForceUpdate();
        }

        private void ForceUpdatePiece(Vector2Int pos)
        {
            if (!HasPieceAt(pos)) return;
            ForceUpdatePiece(GetPieceAt(pos));
        }

        // Converts a piece at a position to it's fen character representation
        private char GetPieceChar(Vector2Int pos)
        {
            return GetPieceChar(pos.x, pos.y);
        }

        // Converts a piece at a position to it's fen character representation
        private char GetPieceChar(int x, int y)
        {
            var type = '-';
            var piece = GetPieceAt(x, y);
            
            print("GetPieceChar Piece: " +  piece.name);

            if (piece == null) return type;

            type = PieceTypeToChar[piece.GetType()];
            
            print("GetPieceChar Type: " + type);

            if (piece.pieceColor == PieceColor.White)
                type = char.ToUpper(type);

            return type;
        }

        // Updates the 2D board state array with new information
        [Button]
        private void UpdateBoardStateArray()
        {
            var newState = new char[8, 8];

            for (var y = 0; y < verticalSquares; y++)
            for (var x = 0; x < horizontalSquares; x++)
                newState[x, y] = GetPieceChar(x, y);

            print("New board state: ");
            NotationsHandler.Print2DArray(newState);
            //
            // var str = NotationsHandler.GetPiecePlacement(newState);
            //
            // print(str);

            BoardState = newState;
        }

        // Adds a piece to the board by creating a new GameObject and Initializing it
        [Button]
        public void AddPiece(Type type, Vector2Int pos, PieceColor color)
        {
            if (!(Squares[pos.x, pos.y].piece is null)) return;

            IntantiatePiece(new GameObject(type.Name, type), pos, color);
        }

        [Button]
        public void PrintBoardState()
        {
            NotationsHandler.Print2DArray(BoardState);
        }
    }
}