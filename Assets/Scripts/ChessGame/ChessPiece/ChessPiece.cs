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

        public PieceColor pieceColor;

        public BoardManager board;

        public Square square;

        public char fenCode;

        protected virtual void Start()
        {
            board.boardUpdate.AddListener(FindLegalPositions);

            FindLegalPositions();
        }

        public virtual void FindLegalPositions()
        {
            LegalPositions.Clear();
            LegalAttacks.Clear();
        }

        public abstract bool MoveToPosition(Vector2Int newPos);

        public bool CanMoveToPosition(Vector2Int newPosition)
        {
            return LegalPositions.Contains(newPosition);
        }
    }
}