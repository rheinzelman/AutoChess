using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Rook : BaseChessPiece
    {
        public bool hasMoved;
        public Vector2Int availableCastle = ErrorSquare;
        public Vector2Int castlePos = ErrorSquare;

        private List<Vector2Int> directionsToCheck = new List<Vector2Int>
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        protected override void FindLegalPositions()
        {
            foreach(var dir in directionsToCheck)
                CheckMovesInLine(dir);

            FindCastlePos();
        }

        private void FindCastlePos()
        {
            CheckCastleInDirection(Vector2Int.left);
            CheckCastleInDirection(Vector2Int.right);

            if (castlePos != ErrorSquare)
                legalPositions.Add(castlePos);
        }

        private void CheckCastleInDirection(Vector2Int direction)
        {
            var chessPiece = CheckMovesInLine(direction);

            if (chessPiece != null && chessPiece is King {hasMoved: false} king && king.pieceColor == pieceColor)
                castlePos = king.currentPosition + 2 * Vector2Int.right;
        }

        public override bool MoveToPosition(Vector2Int newPos)
        {
            if (!base.MoveToPosition(newPos)) return false;

            hasMoved = true;

            return true;
        }
    }
}