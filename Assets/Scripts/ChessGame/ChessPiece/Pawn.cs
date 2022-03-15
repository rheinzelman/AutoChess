using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    [SerializeField]
    private Vector2Int enPassantSquare = new Vector2Int(-1, -1);

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
        if (!board.EnPassantSquares.ContainsKey(pos) || board.EnPassantSquares[pos].pieceColor == pieceColor ) return false;

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

        board.EnPassantSquares.Add(this.enPassantSquare, this);
    }

    private void DisableEnPassant()
    {
        enPassantSquare = new Vector2Int(-1, -1);

        board.EnPassantSquares.Remove(this.enPassantSquare);
    }

    [Button]
    public override bool MoveToPosition(Vector2Int newPos)
    {
        if (!LegalPositions.Contains(newPos) && !LegalAttacks.Contains(newPos)) return false;

        if (!hasMoved && LegalPositions.Count > 1 && newPos == LegalPositions[1])
            EnableEnPassant();
        else
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

        board.boardUpdate.Invoke();

        return true;
    }
}
