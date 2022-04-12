using AutoChess.PlayerInput;
using UnityEngine;
using UnityEngine.Events;

namespace ChessGame
{
    public enum PlayerColor
    {
        Unassigned,
        White,
        Black
    }

    public enum EndState
    {
        Draw,
        WhiteWin,
        BlackWin
    }

    public enum PieceColor
    {
        White = 1,
        Black = 2
    }


    public class MoveEventData
    {
        public BaseInputHandler sender;
        public string args;

        public MoveEventData(BaseInputHandler sender)
        {
            this.sender = sender;
        }
    }

    [System.Serializable]
    public class PlayerTurnEvent : UnityEvent<PlayerColor, BaseInputHandler> { }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Game State")]
        [SerializeField] private PlayerColor playerTurn = PlayerColor.White;
        [SerializeField] private bool gameOver = false;

        [Header("Events")]
        public UnityEvent OnGameOver = new UnityEvent();
        public PlayerTurnEvent OnTurnSwapped = new PlayerTurnEvent();

        [Header("Internal Components")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private BaseInputHandler whiteInputHandler;
        [SerializeField] private BaseInputHandler blackInputHandler;

        private void Start()
        {
            Instance = this;  

            boardManager ??= GetComponent<BoardManager>();
            whiteInputHandler.gameManager = this;
            blackInputHandler.gameManager = this;
        }

        public bool PerformMove(Vector2Int from, Vector2Int to, MoveEventData eventData)
        {
            if (playerTurn != eventData.sender.playerColor || !boardManager.GetPieceAt(from) || boardManager.GetPieceAt(to).pieceColor != (PieceColor) eventData.sender.playerColor) return false;

            var bMoveSuccess = boardManager.MovePiece(to, from, eventData.args);

            if (bMoveSuccess) SwapTurns();

            return bMoveSuccess;
        }

        public void SwapTurns()
        {
            if (playerTurn == PlayerColor.White)
                playerTurn = PlayerColor.Black;
            else
                playerTurn = PlayerColor.White;

            BaseInputHandler activeInput = (playerTurn == PlayerColor.White ? whiteInputHandler : blackInputHandler);

            whiteInputHandler.AlternateTurn();
            blackInputHandler.AlternateTurn();

            OnTurnSwapped.Invoke(playerTurn, activeInput);
        }
    }

}
