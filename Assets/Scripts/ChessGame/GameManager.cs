using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        WhiteWin,
        BlackWin
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
        public PlayerColor playerTurn { get; private set; } = PlayerColor.White;
        public bool gameOver { get; private set; } = false;

        [Header("Turn Information")] public int halfMoveClock;
        public int fullMoveClock = 1;

        [Header("Internal Components")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private BaseInputHandler whiteInputHandler;
        [SerializeField] private BaseInputHandler blackInputHandler;
        [SerializeField] private List<BaseInputHandler> listeners = new List<BaseInputHandler>();

        [Header("Debug")] 
        public bool startWithVerboseDebug = false;

        [Header("Events")] public GameEndEvent onGameOver = new GameEndEvent();
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
        }

        [Button]
        private void ToggleVerboseDebug()
        {
            verboseDebug = !verboseDebug;
            onVerboseDebugChanged.Invoke(verboseDebug);
        }

        public bool PerformMove(Vector2Int from, Vector2Int to, BaseInputHandler sender)
        {
            if (sender == null || !boardManager.HasPieceAt(from))
            {
                Debug.LogError("Game Manager Error: PerformMove() called with invalid sender or invalid position!");
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
            
            if (playerTurn != sender.playerColor || !boardManager.HasPieceAt(from)) return false;

            var moveResult = boardManager.MovePiece(from, to);

            if (!moveResult.Item1) return false;

            SwapTurns();

            var moveData = new MoveEventData(sender, piece.pieceColor, moveResult.Item2, NotationsHandler.GenerateFEN(), boardManager.BoardState);
            
            listeners.ForEach(p => p.ReceiveMove(from, to, moveData));

            switch (playerTurn)
            {
                case PlayerColor.White:
                    whiteInputHandler.ReceiveMove(from, to, moveData);
                    break;
                case PlayerColor.Black:
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

            return true;
        }

        private void SwapTurns()
        {
            playerTurn = playerTurn == PlayerColor.White ? PlayerColor.Black : PlayerColor.White;

            halfMoveClock++;
            if (playerTurn == PlayerColor.White) fullMoveClock++;

            var activeInput = (playerTurn == PlayerColor.White ? whiteInputHandler : blackInputHandler);

            whiteInputHandler.AlternateTurn();
            blackInputHandler.AlternateTurn();

            print(NotationsHandler.GenerateFEN());

            onTurnSwapped.Invoke(playerTurn, activeInput);
        }
    }

}
