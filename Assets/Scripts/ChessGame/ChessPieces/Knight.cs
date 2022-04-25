using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Knight : BaseChessPiece
    {
        public bool hasMoved;
        
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
            foreach (var coords in squaresToCheck.Select(pos => currentPosition + pos))
            {
                if (!board.HasPieceAt(coords) && board.IsValidCoordinate(coords))
                    legalPositions.Add(coords);
                if (board.HasPieceAt(coords) && board.GetPieceAt(coords).pieceColor != pieceColor)
                    legalAttacks.Add(coords);
            }
        }
    }
}