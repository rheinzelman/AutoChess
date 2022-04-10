using AutoChess.ManagerComponents;
using Sirenix.OdinInspector;
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

        public List<Vector2Int> BlockedMoves = new List<Vector2Int>();

        public PieceColor pieceColor;

        public BoardManager board;

        public Square square;

        public static Vector2Int errorSquare = new Vector2Int(-1, -1);

        void Start()
        {
            board.boardUpdate.AddListener(_FindLegalPositions);
            board.pieceTryMove.AddListener(TryMoveUpdate);
            board.boardRefresh.AddListener(OnBoardRefresh);

            _FindLegalPositions();
        }

        [Button]
        private void FindBlockedMoves()
        {
            List<Vector2Int> allMoves = new List<Vector2Int>();
            allMoves.AddRange(LegalPositions);
            allMoves.AddRange(LegalAttacks);

            foreach (Vector2Int pos in allMoves)
                board.TryMovePiece(currentPosition, pos);
        }

        private void TryMoveUpdate(ChessPiece piece)
        {
            if (piece == this) return;

            _FindLegalPositions();
        }

        private void OnBoardRefresh()
        {
            BlockedMoves.Clear();

            _FindLegalPositions();

            FindBlockedMoves();
        }

        [Button]
        private void _FindLegalPositions()
        {
            LegalPositions.Clear();
            LegalAttacks.Clear();

            if (square.piece != this) return;

            FindLegalPositions();
        }

        public abstract void FindLegalPositions();

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
            return !BlockedMoves.Contains(newPosition) && (LegalAttacks.Contains(newPosition) || LegalPositions.Contains(newPosition));
        }
    }
}