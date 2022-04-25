using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class King : BaseChessPiece
    {
        public bool hasMoved = false;
        public List<Vector2Int> availableCastles = new List<Vector2Int>();

        private readonly List<Vector2Int> m_SquaresToCheck = new List<Vector2Int>
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
            foreach (var coords in m_SquaresToCheck.Select(pos => currentPosition + pos))
            {
                if (!board.HasPieceAt(coords) && board.IsValidCoordinate(coords))
                    legalPositions.Add(coords);
                if (board.HasPieceAt(coords) && board.GetPieceAt(coords).pieceColor != pieceColor)
                    legalAttacks.Add(coords);
            }
        }

        public override bool MoveToPosition(Vector2Int newPos)
        {
            if (!base.MoveToPosition(newPos)) return false;

            hasMoved = true;

            return true;
        }
    }
}
