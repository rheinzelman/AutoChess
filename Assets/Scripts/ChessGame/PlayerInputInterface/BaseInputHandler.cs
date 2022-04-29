using System;
using UnityEngine;
using UnityEngine.Events;

namespace ChessGame.PlayerInputInterface
{
    public class BaseInputHandler : MonoBehaviour, IHandleInputInterface
    {
        [Header("Game Manager")]
        private GameManager gameManager;

        [Header("Events")]
        public UnityEvent onTurnStart = new UnityEvent();
        public UnityEvent onTurnFinished = new UnityEvent();
        public UnityEvent onMoveSent = new UnityEvent();
        public UnityEvent onMoveReceived = new UnityEvent();

        // Interface member for player's color
        public PlayerColor playerColor { get; set; } = PlayerColor.Unassigned;


        // Interface member for active turn state
        private bool _bTurnActive = false;
        public bool bTurnActive
        {
            get { return _bTurnActive; }
            set { _bTurnActive = value; }
        }

        private void Start()
        {
            gameManager = GameManager.Instance;
        }

        // When the turns are alternated by the GameManager, start or end turn
        public void AlternateTurn()
        {
            bTurnActive = !bTurnActive;

            if (bTurnActive)
                StartTurn();
            else
                EndTurn();
        }

        // When the turn starts, do something
        public virtual void StartTurn()
        {
            bTurnActive = true;
            onTurnStart.Invoke();
        }

        // When the turn ends, do something
        public virtual void EndTurn()
        {
            bTurnActive = false;
            onTurnFinished.Invoke();
        }

        // When we want to send a move, send it to the GameManager and do something
        public virtual bool SendMove(Vector2Int from, Vector2Int to, MoveEventData moveData)
        {
            if (!gameManager.verboseDebug) return gameManager.PerformMove(@from, to, moveData);

            Debug.Log(name + " BaseInputHandler: Sending move to board from: " + @from + ", to: " + to + '.');

            Debug.Log("Move data values are: Sender - " + moveData.Sender.name +
                      ", Piece Color - " + Enum.GetName(typeof(PieceColor), moveData.PieceColor) +
                      ", Args - " + (moveData.Args == "" ? "none" : moveData.Args));

            return gameManager.PerformMove(from, to, moveData);
        }

        // When the game manager sends us a move, process it and do something
        public virtual void ReceiveMove(Vector2Int to, Vector2Int from, MoveEventData moveData)
        {
            if (!gameManager.verboseDebug) return;

            Debug.Log(name + " BaseInputHandler: Receiving move from: " + from + ", to: " + to + '.');

            Debug.Log("Move data values are: Sender - " + moveData.Sender.name +
                      ", Piece Color - " + Enum.GetName(typeof(PieceColor), moveData.PieceColor) +
                      ", Args - " + (moveData.Args == "" ? "none" : moveData.Args));
        }
    }
}
