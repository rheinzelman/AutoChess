using System;
using AutoChess;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame.ChessPiece
{
    public class Pawn : BaseChessPiece
    {
        [SerializeField] private Vector2Int enPassantSquare = ErrorSquare;
        [SerializeField] private bool hasMoved = false;

        protected override void FindLegalPositions()
        {
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
                legalAttacks.Add(pos1);

            if (CheckForAttackAt(pos2) || CheckForEnPassantAt(pos2))
                legalAttacks.Add(pos2);
        }

        private bool CheckForAttackAt(Vector2Int pos)
        {
            return board.HasPieceAt(pos) && board.GetPieceAt(pos).pieceColor != pieceColor;
        }

        private bool CheckForEnPassantAt(Vector2Int pos)
        {
            if (board.enPassantSquare == null || board.enPassantSquare.Item1 != pos || board.enPassantSquare.Item2.pieceColor == pieceColor) return false;

            return true;
        }

        private void CheckDoubleMove()
        {
            if (pieceColor == PieceColor.Black)
            {
                Vector2Int pos1 = new Vector2Int(currentPosition.x, currentPosition.y + 1);

                if (!board.IsValidCoordinate(pos1) || board.HasPieceAt(pos1)) return;

                legalPositions.Add(pos1);

                Vector2Int pos2 = new Vector2Int(currentPosition.x, currentPosition.y + 2);

                if (!board.IsValidCoordinate(pos2) || board.HasPieceAt(pos2)) return;

                legalPositions.Add(pos2);
            }
            else
            {
                Vector2Int pos1 = new Vector2Int(currentPosition.x, currentPosition.y - 1);

                if (!board.IsValidCoordinate(pos1) || board.HasPieceAt(pos1)) return;

                legalPositions.Add(pos1);

                Vector2Int pos2 = new Vector2Int(currentPosition.x, currentPosition.y - 2);

                if (!board.IsValidCoordinate(pos2) || board.HasPieceAt(pos2)) return;

                legalPositions.Add(pos2);
            }
        }

        private void CheckSingleMove()
        {
            if (pieceColor == PieceColor.Black)
            {
                Vector2Int pos = new Vector2Int(currentPosition.x, currentPosition.y + 1);

                if (!board.IsValidCoordinate(pos) || board.HasPieceAt(pos)) return;

                legalPositions.Add(pos);
            }
            else
            {
                Vector2Int pos = new Vector2Int(currentPosition.x, currentPosition.y - 1);

                if (!board.IsValidCoordinate(pos) || board.HasPieceAt(pos)) return;

                legalPositions.Add(pos);
            }
        }

        private void EnableEnPassant()
        {
            enPassantSquare = legalPositions[0];

            board.enPassantSquare = new Tuple<Vector2Int, BaseChessPiece>(enPassantSquare, this);

            print("En Passant set at: " +  enPassantSquare);
        }

        private void DisableEnPassant()
        {
            enPassantSquare = ErrorSquare;

            board.enPassantSquare = null;

            print("En Passant removed");
        }

        [Button]
        public override bool MoveToPosition(Vector2Int newPos)
        {
            if (!CanMoveToPosition(newPos)) return false;

            if (!hasMoved && legalPositions.Count > 1 && newPos == legalPositions[1])
                EnableEnPassant();
            else if (enPassantSquare != ErrorSquare)
                DisableEnPassant();

            if (legalAttacks.Contains(newPos))
                board.TakePiece(newPos);

            Square newSquare = board.Squares[newPos.x, newPos.y];

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