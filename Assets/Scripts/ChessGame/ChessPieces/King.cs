using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class King : BaseChessPiece
    {
        public bool hasMoved = false;
        public List<Vector2Int> availableCastles = new List<Vector2Int>();
        
        private List<Vector2Int> squaresToCheck = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
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
