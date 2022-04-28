using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Rook : BaseChessPiece
    {
        public bool hasMoved;
        public string castlingRightChar = "-";

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
        }

        public override bool MoveToPosition(Vector2Int newPos)
        {
            if (!base.MoveToPosition(newPos)) return false;

            if (!hasMoved) return true;
            
            hasMoved = false;
            
            board.castlingRights = board.castlingRights.Replace(castlingRightChar, "");

            return true;
        }
    }
}