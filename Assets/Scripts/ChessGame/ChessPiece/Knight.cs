using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Knight : BaseChessPiece
    {
        private List<Vector2Int> squaresToCheck = new List<Vector2Int>()
        {
            new Vector2Int(-1, 2),
            new Vector2Int(1, 2),
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(1, -2),
            new Vector2Int(-1, -2),
            new Vector2Int(-2, -1),
            new Vector2Int(-2, 1)
        };

        protected override void FindLegalPositions()
        {
            foreach (var pos in squaresToCheck)
            {
                var coords = currentPosition + pos;

                if (!board.HasPieceAt(coords) && board.IsValidCoordinate(coords))
                    legalPositions.Add(coords);
                if (board.HasPieceAt(coords) && board.GetPieceAt(coords).pieceColor != pieceColor)
                    legalAttacks.Add(coords);
            }
        }
    }
}