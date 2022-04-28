using System.Collections.Generic;
using System.Linq;
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

        private List<Vector2Int> qSideCastleSquares = new List<Vector2Int>()
        {
            Vector2Int.left,
            Vector2Int.left * 2
        };

        private Vector2Int qCastlePos;

        private List<Vector2Int> kSideCastleSquares = new List<Vector2Int>()
        {
            Vector2Int.right,
            Vector2Int.right * 2
        };
        
        private Vector2Int kCastlePos;

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

        public override void Initialize()
        {
            if (pieceColor == PieceColor.White)
                castlingRights = castlingRights.ToUpper();
            
            kSideRook = (Rook) board.GetPieceAt(currentPosition + Vector2Int.right * 3);
            kSideRook.king = this;
            kCastlePos = currentPosition + Vector2Int.right * 2;
            
            qSideRook = (Rook) board.GetPieceAt(currentPosition + Vector2Int.left * 4);
            qSideRook.king = this;
            qCastlePos = currentPosition + Vector2Int.left * 2;
            
            qSideCastleSquares.ForEach(pos => pos += currentPosition);
            kSideCastleSquares.ForEach(pos => pos += currentPosition);
            
            print("Q Castle squares: " + qSideCastleSquares[0] + " & " + qSideCastleSquares[1]);
            print("K Castle squares: " + qSideCastleSquares[0] + " & " + qSideCastleSquares[1]);

            var testString = "KQkq".Replace("kq", "");
            print("Test : " + testString);
        }

        private void CheckCastles()
        {
            CheckQueenCastle();
            CheckKingCastle();
        }

        private void CheckQueenCastle()
        {
            if (qSideRook == null || kSideRook.hasMoved) return;

            if (CheckMovesInLine(Vector2Int.left) == kSideRook)
                legalPositions.Add(currentPosition + Vector2Int.left * 2);
        }

        private void CheckKingCastle()
        {
            if (kSideRook == null || kSideRook.hasMoved) return;
            
            if (CheckMovesInLine(Vector2Int.right) == kSideRook)
                legalPositions.Add(currentPosition + Vector2Int.right * 2);
        }

        protected override void FindLegalPositions()
        {
            isCastling = false;
            
            CheckCastles();
            
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

            if (hasMoved) return true;
            
            hasMoved = false;
            board.castlingRights = board.castlingRights.Replace(castlingRights, "");
            
            if (newPos == qCastlePos || newPos == kCastlePos)
                isCastling = true;

            return true;
        }
    }
}
