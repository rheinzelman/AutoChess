using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ChessGame.PlayerInputInterface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace ChessGame
{
    [Serializable]
    public enum PlayerColor
    {
        Unassigned = -1,
        White = 1,
        Black = 2
    }

    [Serializable]
    public enum EndState
    {
        Draw,
        Win
    }

    [Serializable]
    public enum PieceColor
    {
        Unassigned = -1,
        White = 1,
        Black = 2
    }

    [Serializable]
    public class MoveEventData
    {
        public readonly BaseInputHandler Sender;
        public readonly PieceColor PieceColor;
        public readonly string Args;
        public readonly string Fen;
        public readonly char[,] BoardState;

        public MoveEventData()
        {
            Sender = null;
            PieceColor = PieceColor.Unassigned;
            Args = "";
            Fen = "";
            BoardState = null;
        }
        public MoveEventData(BaseInputHandler sender, PieceColor pieceColor, string args, string fen, char[,] boardState)
        {
            Sender = sender;
            PieceColor = pieceColor;
            Args = args;
            Fen = fen;
            BoardState = boardState;
        }
    }

    [Serializable]
    public class GameEndEvent : UnityEvent<EndState, PlayerColor> {}

    [Serializable]
    public class PlayerTurnEvent : UnityEvent<PlayerColor, BaseInputHandler> {}
    
    [Serializable]
    public class DebugChangeEvent : UnityEvent<bool> {}

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [field: Header("Game State")]
        public PlayerColor PlayerTurn { get; private set; } = PlayerColor.White;
        public bool GameOver { get; private set; } = false;

        [Header("Turn Information")] public int halfMoveClock;
        public int fullMoveClock = 1;
        public Dictionary<string, int> pastStates = new Dictionary<string, int>();

        [Header("Internal Components")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private BaseInputHandler whiteInputHandler;
        [SerializeField] private BaseInputHandler blackInputHandler;
        
        [Header("Listening")]
        [SerializeField] private bool allowListening = true;
        [SerializeField] private List<BaseInputHandler> listeners = new List<BaseInputHandler>();

        [Header("Debug")] 
        public bool startWithVerboseDebug = false;

        [Header("Events")] 
        public GameEndEvent onGameOver = new GameEndEvent();
        public PlayerTurnEvent onTurnSwapped = new PlayerTurnEvent();
        
        [HideInInspector] public bool verboseDebug;
        [HideInInspector] public DebugChangeEvent onVerboseDebugChanged = new DebugChangeEvent();

        private void Awake()
        {
            Instance = this;  
        }

        private void Start()
        {
            verboseDebug = startWithVerboseDebug;
            boardManager = BoardManager.Instance;
            whiteInputHandler.playerColor = PlayerColor.White;
            blackInputHandler.playerColor = PlayerColor.Black;
            whiteInputHandler.gameManager = this;
            blackInputHandler.gameManager = this;

            switch (PlayerTurn)
            {
                case PlayerColor.White:
                    whiteInputHandler.TurnActive = true;
                    break;
                case PlayerColor.Black:
                    blackInputHandler.TurnActive = true;
                    break;
                case PlayerColor.Unassigned:
                    Debug.LogWarning("Player Turn is Unassigned! Setting to White.");
                    PlayerTurn = PlayerColor.White;
                    whiteInputHandler.TurnActive = true;
                    break;
                default:
                    Debug.LogWarning("Player Turn is Null! Setting to White.");
                    PlayerTurn = PlayerColor.White;
                    whiteInputHandler.TurnActive = true;
                    break;
            }
        }

        [Button]
        private void ToggleVerboseDebug()
        {
            verboseDebug = !verboseDebug;
            onVerboseDebugChanged.Invoke(verboseDebug);
        }

        public bool PerformMove(Vector2Int from, Vector2Int to, BaseInputHandler sender)
        {
            if (GameOver) return false;
            
            if (sender == null || !boardManager.HasPieceAt(from))
            {
                if (!verboseDebug) return false;
                    if (sender == null) Debug.LogError("Game Manager Error: PerformMove() called with invalid sender!");
                    if (!boardManager.HasPieceAt(from))
                        Debug.LogError("Game Manager Error: PerformMove() called with invalid position!");
                    
                return false;
            }

            var piece = boardManager.GetPieceAt(from);
            
            if ((int) piece.pieceColor != (int) sender.playerColor)
            {
                if (verboseDebug)
                    Debug.LogError("Game Manager Error: PerformMove() called with a PieceColor of " + 
                                   Enum.GetName(typeof(PieceColor), piece.pieceColor) + 
                                   " but a PlayerColor of " + 
                                   Enum.GetName(typeof(PlayerColor),  sender.playerColor) + 
                                   '!');

                return false;
            }
            
            if (PlayerTurn != sender.playerColor || !boardManager.HasPieceAt(from)) return false;

            var moveResult = boardManager.MovePiece(from, to);

            if (!moveResult.Item1) return false;

            SwapTurns();

            var moveData = new MoveEventData(sender, piece.pieceColor, moveResult.Item2, NotationsHandler.GenerateFEN(), boardManager.BoardState);
            
            if (allowListening) listeners.ForEach(p => p.ReceiveMove(from, to, moveData));

            switch (PlayerTurn)
            {
                case PlayerColor.White:
                    blackInputHandler.TurnActive = false;
                    whiteInputHandler.TurnActive = true;
                    whiteInputHandler.ReceiveMove(from, to, moveData);
                    break;
                case PlayerColor.Black:
                    whiteInputHandler.TurnActive = false;
                    blackInputHandler.TurnActive = true;
                    blackInputHandler.ReceiveMove(from, to, moveData);
                    break;
                case PlayerColor.Unassigned:
                    Debug.LogError("Game Manager Error: " +
                                   "PlayerColor in playerTurn is set to PlayerColor.Unassigned!");
                    break;
                default:
                    Debug.LogError("Game Manager Error: Invalid PlayerColor in playerTurn!");
                    break;
            }

            CheckGameEnd();

            return true;
        }

        private void CheckGameEnd()
        {
            boardManager.CheckVictoryConditions();

            var curState = NotationsHandler.GetPiecePlacement(boardManager.BoardState);

            if (pastStates.ContainsKey(curState))
                pastStates[curState] += 1;
            else
                pastStates.Add(curState, 1);

            if (pastStates[curState] >= 3)
                DeclareGameOver(EndState.Draw, PlayerColor.Unassigned);

            switch (PlayerTurn)
            {
                case PlayerColor.White when boardManager.whiteKing.inCheck && !boardManager.whiteHasMoves:
                    DeclareGameOver(EndState.Win, PlayerColor.Black);
                    break;
                case PlayerColor.White when !boardManager.whiteKing.inCheck && !boardManager.whiteHasMoves:
                    DeclareGameOver(EndState.Draw, PlayerColor.Unassigned);
                    return;
                case PlayerColor.Black when boardManager.blackKing.inCheck && !boardManager.blackHasMoves:
                    DeclareGameOver(EndState.Win, PlayerColor.White);
                    break;
                case PlayerColor.Black when !boardManager.blackKing.inCheck && !boardManager.blackHasMoves:
                    DeclareGameOver(EndState.Draw, PlayerColor.Unassigned);
                    return;
            }
        }

        private void DeclareGameOver(EndState endState, PlayerColor playerColor)
        {
            GameOver = true;
            onGameOver.Invoke(endState, playerColor);
            
            switch (endState)
            {
                case EndState.Draw:
                    print("Game has ended in a draw!");
                    break;
                case EndState.Win:
                    print("Game has ended with a victory for" + (playerColor == PlayerColor.White ? " white" : " black") + "!");
                    break;
            }
        }

        private void SwapTurns()
        {
            PlayerTurn = PlayerTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

            halfMoveClock++;
            if (PlayerTurn == PlayerColor.White) fullMoveClock++;

            var activeInput = (PlayerTurn == PlayerColor.White ? whiteInputHandler : blackInputHandler);

            whiteInputHandler.AlternateTurn();
            blackInputHandler.AlternateTurn();

            print(NotationsHandler.GenerateFEN());

            onTurnSwapped.Invoke(PlayerTurn, activeInput);
        }
    }

}
