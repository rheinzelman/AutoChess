using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AutoChess.ChessPieces
{
    public class Pawn : ChessPiece
    {
        [SerializeField]
        private Vector2Int enPassantSquare = errorSquare;

        [SerializeField]
        private bool hasMoved = false;

        [Button]
        public override void FindLegalPositions()
        {
            base.FindLegalPositions();

            if (!hasMoved)
                CheckDoubleMove();
            else
                CheckSingleMove();

            CheckDiagonals();
        }

        private void CheckDiagonals()
        {
            if (pieceColor == PieceColor.Black)
            {
                Vector2Int attack1 = new Vector2Int(currentPosition.x + 1, currentPosition.y + 1);
                Vector2Int attack2 = new Vector2Int(currentPosition.x - 1, currentPosition.y + 1);

                FindAttacksAtDiagonals(attack1, attack2);
            }
            else
            {
                Vector2Int attack1 = new Vector2Int(currentPosition.x + 1, currentPosition.y - 1);
                Vector2Int attack2 = new Vector2Int(currentPosition.x - 1, currentPosition.y - 1);

                FindAttacksAtDiagonals(attack1, attack2);
            }
        }

        private void FindAttacksAtDiagonals(Vector2Int pos1, Vector2Int pos2)
        {
            if (CheckForAttackAt(pos1) || CheckForEnPassantAt(pos1))
                LegalAttacks.Add(pos1);

            if (CheckForAttackAt(pos2) || CheckForEnPassantAt(pos2))
                LegalAttacks.Add(pos2);
        }

        private bool CheckForAttackAt(Vector2Int pos)
        {
            return board.HasPieceAt(pos) && board.GetPieceAt(pos).pieceColor != pieceColor;
        }

        private bool CheckForEnPassantAt(Vector2Int pos)
        {
            if (board.EnPassantSquare == null || board.EnPassantSquare.Item1 != pos || board.EnPassantSquare.Item2.pieceColor == pieceColor) return false;

            return true;
        }

        private void CheckDoubleMove()
        {
            if (pieceColor == PieceColor.Black)
            {
                Vector2Int pos1 = new Vector2Int(currentPosition.x, currentPosition.y + 1);

                if (!board.IsValidCoordinate(pos1) || board.HasPieceAt(pos1)) return;

                LegalPositions.Add(pos1);

                Vector2Int pos2 = new Vector2Int(currentPosition.x, currentPosition.y + 2);

                if (!board.IsValidCoordinate(pos2) || board.HasPieceAt(pos2)) return;

                LegalPositions.Add(pos2);
            }
            else
            {
                Vector2Int pos1 = new Vector2Int(currentPosition.x, currentPosition.y - 1);

                if (!board.IsValidCoordinate(pos1) || board.HasPieceAt(pos1)) return;

                LegalPositions.Add(pos1);

                Vector2Int pos2 = new Vector2Int(currentPosition.x, currentPosition.y - 2);

                if (!board.IsValidCoordinate(pos2) || board.HasPieceAt(pos2)) return;

                LegalPositions.Add(pos2);
            }
        }

        private void CheckSingleMove()
        {
            if (pieceColor == PieceColor.Black)
            {
                Vector2Int pos = new Vector2Int(currentPosition.x, currentPosition.y + 1);

                if (!board.IsValidCoordinate(pos) || board.HasPieceAt(pos)) return;

                LegalPositions.Add(pos);
            }
            else
            {
                Vector2Int pos = new Vector2Int(currentPosition.x, currentPosition.y - 1);

                if (!board.IsValidCoordinate(pos) || board.HasPieceAt(pos)) return;

                LegalPositions.Add(pos);
            }
        }

        private void EnableEnPassant()
        {
            enPassantSquare = LegalPositions[0];

            board.EnPassantSquare = new Tuple<Vector2Int, ChessPiece>(enPassantSquare, this);

            print("En Passant set at: " +  enPassantSquare);
        }

        private void DisableEnPassant()
        {
            enPassantSquare = errorSquare;

            board.EnPassantSquare = null;

            print("En Passant removed");
        }

        [Button]
        public override bool MoveToPosition(Vector2Int newPos)
        {
            if (!CanMoveToPosition(newPos)) return false;

            if (!hasMoved && LegalPositions.Count > 1 && newPos == LegalPositions[1])
                EnableEnPassant();
            else if (enPassantSquare != errorSquare)
                DisableEnPassant();

            if (LegalAttacks.Contains(newPos))
                board.TakePiece(newPos);

            Square newSquare = board.squares[newPos.x, newPos.y];

            transform.parent = newSquare.transform;

            newSquare.piece = this;

            square.piece = null;

            square = newSquare;

            currentPosition = square.coordinate;

            hasMoved = true;

            return true;
        }
    }
}