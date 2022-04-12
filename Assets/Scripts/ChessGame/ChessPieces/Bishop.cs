using UnityEngine;
using System.Collections.Generic;

namespace ChessGame.ChessPieces
{
    public class Bishop : BaseChessPiece
    {
        private List<Vector2Int> directionsToCheck = new List<Vector2Int>
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1)
        };
        
        protected override void FindLegalPositions()
        {
            foreach (var dir in directionsToCheck)
                CheckMovesInLine(dir);
        }
    }
}