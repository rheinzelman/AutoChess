using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoChess.PlayerInput
{
    public class BasePlayerInput : MonoBehaviour, IPlayerInputInterface
    {
        [Header("Game Manager")]
        [SerializeField] private GameManager gameManager;

        [Header("Events")]
        public UnityEvent OnTurnStart = new UnityEvent();
        public UnityEvent OnTurnFinished = new UnityEvent();
        public UnityEvent OnMoveSent = new UnityEvent();
        public UnityEvent OnMoveReceived = new UnityEvent();

        // Interface member for player's color
        private PlayerColor _playerColor = PlayerColor.Unassigned;
        public PlayerColor playerColor 
        { 
            get { return _playerColor; }
            set { _playerColor = value; } 
        }

        // Interface member for active turn state
        private bool _bTurnActive = false;
        public bool bTurnActive
        {
            get { return _bTurnActive; }
            set { _bTurnActive = value; }
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
            OnTurnStart.Invoke();
        }

        // When the turn ends, do something
        public virtual void EndTurn()
        {
            bTurnActive = false;
            OnTurnFinished.Invoke();
        }

        // When we want to send a move, send it to the GameManager and do something
        public virtual void SendMove(Vector2Int to, Vector2Int from) { }

        // When the game manager sends us a move, process it and do something
        public virtual void ReceiveMove(Vector2Int to, Vector2Int from) { }
    }
}
