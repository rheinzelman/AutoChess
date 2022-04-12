using System.Collections.Generic;
using AutoChess;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public abstract class BaseChessPiece : MonoBehaviour
    {
        public Vector2Int currentPosition = Vector2Int.zero;

        public List<Vector2Int> legalPositions = new List<Vector2Int>();

        public List<Vector2Int> legalAttacks = new List<Vector2Int>();

        public List<Vector2Int> blockedMoves = new List<Vector2Int>();

        public PieceColor pieceColor;

        public BoardManager board;

        public Square square;

        protected static Vector2Int ErrorSquare = new Vector2Int(-1, -1);

        private void Start()
        {
            board.boardUpdate.AddListener(_FindLegalPositions);
            board.pieceTryMove.AddListener(TryMoveUpdate);
            //board.boardRefresh.AddListener(OnBoardRefresh);

            _FindLegalPositions();
        }

        // Checks all positions in a line given a direction to check in
        protected void CheckMovesInLine(Vector2Int direction)
        {
            var i = 1;
            var nextPos = currentPosition + direction;

            while (!board.HasPieceAt(nextPos) && board.IsValidCoordinate(nextPos))
            {
                legalPositions.Add(nextPos);
                nextPos = currentPosition + ++i * direction;
            }
            
            if (board.HasPieceAt(nextPos) && board.GetPieceAt(nextPos).pieceColor != pieceColor)
                legalAttacks.Add(nextPos);
        }

        // Checks for moves that would put the friendly king in check
        [Button]
        private void FindBlockedMoves()
        {
            var allMoves = new List<Vector2Int>();
            allMoves.AddRange(legalPositions);
            allMoves.AddRange(legalAttacks);

            foreach (var pos in allMoves)
                board.TryMovePiece(currentPosition, pos);
        }

        // When pieces are performing TryMovePiece calls in board manager, find legal moves
        // When this piece is performing TryMovePiece calls, do not generate new moves
        private void TryMoveUpdate(BaseChessPiece piece)
        {
            if (piece == this) return;

            _FindLegalPositions();
        }

        // When the board refreshes, clear blocked moves list and find new legal positions
        private void OnBoardRefresh()
        {
            Debug.Log("Board refresh in " + name + " at " + currentPosition);
            
            PerformRefresh();
        }

        // Forces a refresh on the piece, checking blocked moves and legal moves
        public void ForceRefresh()
        {
            PerformRefresh();
        }

        // Performs a refresh for the piece, checking for legal positions and finding blocked moves
        private void PerformRefresh()
        {
            blockedMoves.Clear();

            _FindLegalPositions();

            FindBlockedMoves();
        }

        // Forces the piece to find new legal positions
        public void ForceUpdate()
        {
            _FindLegalPositions();
        }
        
        // Internal function for finding legal positions
        private void _FindLegalPositions()
        {
            legalPositions.Clear();
            legalAttacks.Clear();

            if (square.piece != this) return;

            FindLegalPositions();
        }

        // Unique abstract function that each piece implements to find their legal moves
        protected abstract void FindLegalPositions();

        // Forces the piece to move to a specific position
        public void ForceMoveToPosition(Vector2Int newPos)
        {
            var newSquare = board.Squares[newPos.x, newPos.y];

            transform.parent = newSquare.transform;

            newSquare.piece = this;

            square.piece = null;

            square = newSquare;

            currentPosition = square.coordinate;
        }

        // Tries to move the piece to a new position and returns false on fail, true on success
        public virtual bool MoveToPosition(Vector2Int newPos)
        {
            //if position does not exist in legal positions or attacks then it is not a legal move
            if (!CanMoveToPosition(newPos)) return false;

            //take piece if move is an attack
            if (legalAttacks.Contains(newPos))
                board.TakePiece(newPos);

            //update piece information to new positions
            var newSquare = board.Squares[newPos.x, newPos.y];
            transform.parent = newSquare.transform;
            newSquare.piece = this;
            square.piece = null;
            square = newSquare;
            currentPosition = square.coordinate;
            //board.boardUpdate.Invoke();
            //move was successful
            
            OnBoardRefresh();
            
            return true;
        }

        // Checks if the piece can legally move to a position
        public bool CanMoveToPosition(Vector2Int newPosition)
        {
            return !blockedMoves.Contains(newPosition) && (legalAttacks.Contains(newPosition) || legalPositions.Contains(newPosition));
        }
    }
}