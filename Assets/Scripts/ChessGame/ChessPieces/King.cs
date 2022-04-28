using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class King : BaseChessPiece
    {
        public bool hasMoved;
        public bool inCheck;
        public bool isCastling;
        public string castlingRights = "kq";

        public Rook qSideRook;
        public Rook kSideRook;

        public List<BaseChessPiece> attackers = new List<BaseChessPiece>();

        private List<Vector2Int> qSideCastleSquares = new List<Vector2Int>()
        {
            Vector2Int.left,
            Vector2Int.left * 2
        };

        private List<Vector2Int> kSideCastleSquares = new List<Vector2Int>()
        {
            Vector2Int.right,
            Vector2Int.right * 2
        };

        private readonly List<Vector2Int> _squaresToCheck = new List<Vector2Int>
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

        public override void Initialize()
        {
            if (pieceColor == PieceColor.White)
                castlingRights = castlingRights.ToUpper();
            
            kSideRook = (Rook) board.GetPieceAt(currentPosition + Vector2Int.right * 3);
            kSideRook.castlingRightChar = pieceColor == PieceColor.White ? "K" : "k";

            var kSide = new List<Vector2Int>();
            kSideCastleSquares.ForEach(pos => kSide.Add(currentPosition + pos));
            kSideCastleSquares = kSide;

            qSideRook = (Rook) board.GetPieceAt(currentPosition + Vector2Int.left * 4);
            qSideRook.castlingRightChar = pieceColor == PieceColor.White ? "Q" : "q";

            var qSide = new List<Vector2Int>();
            qSideCastleSquares.ForEach(pos => qSide.Add(currentPosition + pos));
            qSideCastleSquares = qSide;
        }

        private void CheckCastles()
        {
            if (hasMoved) return;
            
            CheckQueenCastle();
            CheckKingCastle();
        }

        private void CheckQueenCastle()
        {
            if (qSideRook == null || qSideRook.hasMoved) return;

            if (FindPieceInLine(Vector2Int.left) == qSideRook)
                legalPositions.Add(currentPosition + Vector2Int.left * 2);
        }

        private void CheckKingCastle()
        {
            if (kSideRook == null || kSideRook.hasMoved) return;
            
            if (FindPieceInLine(Vector2Int.right) == kSideRook)
                legalPositions.Add(currentPosition + Vector2Int.right * 2);
        }

        protected override void PostFindLegalPositions()
        {
            if (blockedMoves.Contains(qSideCastleSquares[0]))
                blockedMoves.Add(qSideCastleSquares[1]);
            
            if (blockedMoves.Contains(kSideCastleSquares[0]))
                blockedMoves.Add(kSideCastleSquares[1]);
        }

        protected override void FindLegalPositions()
        {
            isCastling = false;
            
            CheckCastles();
            
            foreach (var coords in _squaresToCheck.Select(pos => currentPosition + pos))
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

            if (hasMoved) return true;
            
            if (newPos == qSideCastleSquares[1])
            {
                isCastling = true;
                board.PerformCastle(qSideRook, qSideCastleSquares[0]);
            }
            if (newPos == kSideCastleSquares[1])
            {
                isCastling = true;
                board.PerformCastle(kSideRook, kSideCastleSquares[0]);
            }
            
            hasMoved = false;
            board.castlingRights = board.castlingRights.Replace(castlingRights, "");

            return true;
        }
    }
}
