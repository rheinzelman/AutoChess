using AutoChess;
using ChessGame.ChessPieces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame
{
    public class Square : MonoBehaviour
    {
        public BoardManager board;

        public Vector2Int coordinate = Vector2Int.zero;

        public BaseChessPiece piece;

        public bool HasPiece()
        {
            return piece != null;
        }

        private void InitializePiece(GameObject newPiece, PieceColor color)
        {
            newPiece.transform.parent = transform;

            piece = newPiece.GetComponent<BaseChessPiece>();//newPiece.AddComponent<Pawn>();

            piece.board = board;

            piece.square = this;

            piece.pieceColor = color;

            piece.currentPosition = coordinate;

            board.boardUpdate.Invoke();
        }

        [Button]
        public void AddPawn(PieceColor color)
        {
            if (piece != null) return;

            InitializePiece(new GameObject("Pawn", typeof(Pawn)), color);
        }

        [Button]
        public void AddBishop(PieceColor color)
        {
            if (piece != null) return;

            InitializePiece(new GameObject("Bishop", typeof(Bishop)), color);
        }

        [Button]
        public void AddKnight(PieceColor color)
        {
            if (piece != null) return;

            InitializePiece(new GameObject("Knight", typeof(Knight)), color);
        }

        [Button]
        public void AddRook(PieceColor color)
        {
            if (piece != null) return;

            InitializePiece(new GameObject("Rook", typeof(Rook)), color);
        }

        [Button]
        public void AddKing(PieceColor color)
        {
            if (piece != null) return;

            InitializePiece(new GameObject("King", typeof(King)), color);
        }

        [Button]
        public void AddQueen(PieceColor color)
        {
            if (piece != null) return;

            InitializePiece(new GameObject("Queen", typeof(Queen)), color);
        }
    }
}