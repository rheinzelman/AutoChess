using System;
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
        
        [Button]
        public void AddPiece(Type type, PieceColor color)
        {
            board.AddPiece(type, coordinate, color);
        }
    }
}