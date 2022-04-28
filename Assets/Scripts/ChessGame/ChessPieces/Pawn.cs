using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChessGame.ChessPieces
{
    public class Pawn : BaseChessPiece
    {
        [SerializeField] private Vector2Int enPassantSquare = ErrorSquare;
        [SerializeField] private bool hasMoved;
        [SerializeField] private int rank = 2;
        public bool isEnPassanting;

        protected override void FindLegalPositions()
        {
            isEnPassanting = false;
            
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
                var attack1 = new Vector2Int(currentPosition.x + 1, currentPosition.y + 1);
                var attack2 = new Vector2Int(currentPosition.x - 1, currentPosition.y + 1);

                FindAttacksAtDiagonals(attack1, attack2);
            }
            else
            {
                var attack1 = new Vector2Int(currentPosition.x + 1, currentPosition.y - 1);
                var attack2 = new Vector2Int(currentPosition.x - 1, currentPosition.y - 1);

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
            return board.enPassantSquare != null &&
                   board.enPassantSquare.Item1 == pos &&
                   board.enPassantSquare.Item2.pieceColor != pieceColor;
        }

        private void CheckDoubleMove()
        {
            if (pieceColor == PieceColor.Black)
            {
                var pos1 = new Vector2Int(currentPosition.x, currentPosition.y + 1);

                if (!board.IsValidCoordinate(pos1) || board.HasPieceAt(pos1)) return;

                legalPositions.Add(pos1);

                var pos2 = new Vector2Int(currentPosition.x, currentPosition.y + 2);

                if (!board.IsValidCoordinate(pos2) || board.HasPieceAt(pos2)) return;

                legalPositions.Add(pos2);
            }
            else
            {
                var pos1 = new Vector2Int(currentPosition.x, currentPosition.y - 1);

                if (!board.IsValidCoordinate(pos1) || board.HasPieceAt(pos1)) return;

                legalPositions.Add(pos1);

                var pos2 = new Vector2Int(currentPosition.x, currentPosition.y - 2);

                if (!board.IsValidCoordinate(pos2) || board.HasPieceAt(pos2)) return;

                legalPositions.Add(pos2);
            }
        }

        private void CheckSingleMove()
        {
            if (pieceColor == PieceColor.Black)
            {
                var pos = new Vector2Int(currentPosition.x, currentPosition.y + 1);

                if (!board.IsValidCoordinate(pos) || board.HasPieceAt(pos)) return;

                legalPositions.Add(pos);
            }
            else
            {
                var pos = new Vector2Int(currentPosition.x, currentPosition.y - 1);

                if (!board.IsValidCoordinate(pos) || board.HasPieceAt(pos)) return;

                legalPositions.Add(pos);
            }
        }

        private void EnableEnPassant()
        {
            enPassantSquare = legalPositions[0];

            board.enPassantSquare = new Tuple<Vector2Int, BaseChessPiece>(enPassantSquare, this);

            ++rank;

            //print("En Passant set at: " +  enPassantSquare);
        }

        private void DisableEnPassant()
        {
            enPassantSquare = ErrorSquare;

            board.enPassantSquare = null;

            //print("En Passant removed");
        }

        [Button]
        public override bool MoveToPosition(Vector2Int newPos)
        {
            if (board.enPassantSquare != null && board.enPassantSquare.Item1 == newPos)
                isEnPassanting = true;
            
            if (!base.MoveToPosition(newPos))
            {
                isEnPassanting = false;
                return false;
            }

            if (!hasMoved && legalPositions.Count > 1 && newPos == legalPositions[1])
                EnableEnPassant();
            else if (enPassantSquare != ErrorSquare)
                DisableEnPassant();
            
            if (++rank >= 8)
                board.Promote(this);

            // //if position does not exist in legal positions or attacks then it is not a legal move
            // if (!CanMoveToPosition(newPos)) return false;
            //
            // if (!hasMoved && legalPositions.Count > 1 && newPos == legalPositions[1])
            //     EnableEnPassant();
            // else if (enPassantSquare != ErrorSquare)
            //     DisableEnPassant();
            //
            // hasMoved = true;
            //
            // //take piece if move is an attack
            // if (legalAttacks.Contains(newPos))
            //     board.TakePiece(newPos);
            //
            // //update piece information to new positions
            // var newSquare = board.Squares[newPos.x, newPos.y];
            // transform.parent = newSquare.transform;
            // newSquare.piece = this;
            // square.piece = null;
            // square = newSquare;
            // currentPosition = square.coordinate;
            // //board.boardUpdate.Invoke();
            // //move was successful
            //
            // if (++rank >= 8)
            //     board.Promote(this);

            return true;
        }
    }
}