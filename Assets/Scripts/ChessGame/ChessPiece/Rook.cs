using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame.ChessPiece
{
    public class Rook : BaseChessPiece
    {
        public bool hasMoved;
        public Vector2Int availableCastle = ErrorSquare;
        
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
    }
}