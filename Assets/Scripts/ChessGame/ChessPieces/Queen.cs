using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Queen : BaseChessPiece
    {
        private List<Vector2Int> directionsToCheck = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
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