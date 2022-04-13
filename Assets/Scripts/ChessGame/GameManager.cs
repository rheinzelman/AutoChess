using System;
using System.ComponentModel;
using AutoChess.PlayerInput;
using ChessGame.PlayerInputInterface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ChessGame
{
    public enum PlayerColor
    {
        Unassigned = -1,
        White = 1,
        Black = 2
    }

    public enum EndState
    {
        Draw,
        WhiteWin,
        BlackWin
    }

    public enum PieceColor
    {
        Unassigned = -1,
        White = 1,
        Black = 2
    }

    public class MoveEventData
    {
        [DefaultValue(null)]
        public readonly BaseInputHandler Sender;
        [DefaultValue(PlayerColor.Unassigned)]
        public readonly PieceColor PieceColor;
        [DefaultValue("")]
        public readonly string Args;

        public MoveEventData()
        {
            this.Sender = null;
            this.PieceColor = PieceColor.Unassigned;
            this.Args = "";
        }
        public MoveEventData(BaseInputHandler sender, PieceColor pieceColor, string args)
        {
            Sender = sender;
            PieceColor = pieceColor;
            Args = args;
        }
    }

    [System.Serializable]
    public class GameEndEvent : UnityEvent<EndState, PlayerColor> {}

    [System.Serializable]
    public class PlayerTurnEvent : UnityEvent<PlayerColor, BaseInputHandler> {}
    
    [System.Serializable]
    public class DebugChangeEvent : UnityEvent<bool> {}

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [field: Header("Game State")]
        public PlayerColor playerTurn { get; private set; } = PlayerColor.White;
        public bool gameOver { get; private set; } = false;

        [Header("Events")]
        public GameEndEvent onGameOver = new GameEndEvent();
        public PlayerTurnEvent onTurnSwapped = new PlayerTurnEvent();

        [Header("Internal Components")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private BaseInputHandler whiteInputHandler;
        [SerializeField] private BaseInputHandler blackInputHandler;

        [Header("Debug")] 
        public bool startWithVerboseDebug = false;
        
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
        }

        [Button]
        private void ToggleVerboseDebug()
        {
            verboseDebug = !verboseDebug;
            onVerboseDebugChanged.Invoke(verboseDebug);
        }

        public bool PerformMove(Vector2Int from, Vector2Int to, MoveEventData moveData)
        {
            if (moveData.Sender == null)
            {
                Debug.LogError("Game Manager Error: PerformMove() called without valid sender!");
                return false;
            }
            
            if ((int) moveData.PieceColor != (int) moveData.Sender.playerColor)
            {
                if (verboseDebug)
                    Debug.LogError("Game Manager Error: PerformMove() called with a PieceColor of " + 
                                   Enum.GetName(typeof(PieceColor), moveData.PieceColor) + 
                                   " but a PlayerColor of " + 
                                   Enum.GetName(typeof(PlayerColor),  moveData.Sender.playerColor) + 
                                   '!');

                return false;
            }
            
            if (playerTurn != moveData.Sender.playerColor || !boardManager.HasPieceAt(from)) return false;

            if (!boardManager.MovePiece(from, to, moveData)) return false;

            SwapTurns();

            switch (playerTurn)
            {
                case PlayerColor.White:
                    whiteInputHandler.ReceiveMove(to, @from, moveData);
                    break;
                case PlayerColor.Black:
                    blackInputHandler.ReceiveMove(to, @from, moveData);
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

            var activeInput = (playerTurn == PlayerColor.White ? whiteInputHandler : blackInputHandler);

            whiteInputHandler.AlternateTurn();
            blackInputHandler.AlternateTurn();

            onTurnSwapped.Invoke(playerTurn, activeInput);
        }
    }

}
