using AutoChess.ManagerComponents;
using AutoChess.PlayerInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoChess
{
    public enum PlayerColor
    {
        Unassigned,
        White,
        Black
    }

    public enum EndState
    {
        Stalemate,
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
        public BasePlayerInput sender;
        public string args;
    }

    [System.Serializable]
    public class PlayerTurnEvent : UnityEvent<PlayerColor, BasePlayerInput> { }

    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        [SerializeField] private PlayerColor playerTurn = PlayerColor.White;
        [SerializeField] private bool gameOver = false;

        [Header("Events")]
        public UnityEvent OnGameOver = new UnityEvent();
        public PlayerTurnEvent OnTurnSwapped = new PlayerTurnEvent();

        [Header("Internal Components")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private BasePlayerInput whiteInputHandler;
        [SerializeField] private BasePlayerInput blackInputHandler;

        public bool PerformMove(Vector2Int to, Vector2Int from, MoveEventData eventData)
        {
            if (playerTurn != eventData.sender.playerColor || !boardManager.GetPieceAt(to) || boardManager.GetPieceAt(to).pieceColor != (PieceColor) eventData.sender.playerColor) return false;

            bool bMoveSuccess = boardManager.MovePiece(to, from, eventData.args);

            if (bMoveSuccess) SwapTurns();

            return bMoveSuccess;
        }

        public void SwapTurns()
        {
            if (playerTurn == PlayerColor.White)
                playerTurn = PlayerColor.Black;
            else
                playerTurn = PlayerColor.White;

            BasePlayerInput activeInput = (playerTurn == PlayerColor.White ? whiteInputHandler : blackInputHandler);

            whiteInputHandler.AlternateTurn();
            blackInputHandler.AlternateTurn();

            OnTurnSwapped.Invoke(playerTurn, activeInput);
        }
    }
}
