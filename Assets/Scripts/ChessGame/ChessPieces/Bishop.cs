using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Bishop : BaseChessPiece
    {
        private readonly List<Vector2Int> _directionsToCheck = new List<Vector2Int>
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1)
        };
        
        protected override void FindLegalPositions()
        {
            foreach (var dir in _directionsToCheck)
                CheckMovesInLine(dir);
        }
    }
}