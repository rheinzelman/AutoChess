using AutoChess.ManagerComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoChess.ChessPieces
{
    public abstract class ChessPiece : MonoBehaviour
    {
        public Vector2Int currentPosition = Vector2Int.zero;

        public List<Vector2Int> LegalPositions = new List<Vector2Int>();

        public List<Vector2Int> LegalAttacks = new List<Vector2Int>();

        public List<Vector2Int> BlockedSquares = new List<Vector2Int>();

        public PieceColor pieceColor;

        public BoardManager board;

        public Square square;

        public static Vector2Int errorSquare = new Vector2Int(-1, -1);

        protected virtual void Start()
        {
            board.boardUpdate.AddListener(FindLegalPositions);
        }

        public virtual void FindLegalPositions()
        {
            LegalPositions.Clear();
            LegalAttacks.Clear();
        }

        public void ForceMoveToPosition(Vector2Int newPos)
        {
            Square newSquare = board.squares[newPos.x, newPos.y];

            transform.parent = newSquare.transform;

            newSquare.piece = this;

            square.piece = null;

            square = newSquare;

            currentPosition = square.coordinate;
        }

        public abstract bool MoveToPosition(Vector2Int newPos);

        public bool CanMoveToPosition(Vector2Int newPosition)
        {
            return !BlockedSquares.Contains(newPosition) && LegalAttacks.Contains(newPosition) || LegalPositions.Contains(newPosition);
        }
    }
}