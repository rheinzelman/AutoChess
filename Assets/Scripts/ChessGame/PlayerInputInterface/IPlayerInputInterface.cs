using UnityEngine;

namespace ChessGame.PlayerInputInterface
{
    public interface IHandleInputInterface
    {
        public PlayerColor playerColor { get; set; }
        public bool bTurnActive { get; set; }

        // Will be called when the turn is alternated by the GameManager
        public void AlternateTurn();
        // Will be called on the start of the turn
        public void StartTurn();
        // Will be called on the end of the turn
        public void EndTurn();
        // Used to send a move to the GameManager
        public bool SendMove(Vector2Int to, Vector2Int from, MoveEventData moveData);
        // Used when a move is received from the opponent
        public void ReceiveMove(Vector2Int to, Vector2Int from, MoveEventData moveData);
    }
}
