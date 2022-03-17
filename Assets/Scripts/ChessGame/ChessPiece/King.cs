using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    private List<Vector2Int> possibleMoves = new List<Vector2Int>();

    [Button]
    public override void FindLegalPositions()
    {
        possibleMoves.Clear();
        LegalAttacks.Clear();
        LegalPositions.Clear();

        //generate the 8 possible moves for a king from current position
        Vector2Int _pos = new Vector2Int(currentPosition.x, currentPosition.y + 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x + 1, currentPosition.y + 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x + 1, currentPosition.y);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x + 1, currentPosition.y - 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x, currentPosition.y - 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 1, currentPosition.y - 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 1, currentPosition.y);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 1, currentPosition.y + 1);
        possibleMoves.Add(_pos);

        //iterate through each position and check for valid coordinate and for a piece
        //if a piece is of opposite color exists then move is added to legal attacks
        //if move is valid coordinate and no piece then move is added to legal positions
        foreach (Vector2Int pos in possibleMoves)
        {
            if (HasAttackers(pos)) return;
            if (board.HasPieceAt(pos))
                if (board.GetPieceAt(pos).pieceColor != pieceColor)
                    LegalAttacks.Add(pos);
                else return;
            else if (board.IsValidCoordinate(pos))
                LegalPositions.Add(pos);

        }
    }

    public bool HasAttackers(Vector2Int _pos)
    {
        if (pieceColor == PieceColor.White)
        {
            foreach (ChessPiece piece in board.BlackPieces)
            {
                if (piece.LegalPositions.Contains(_pos))
                    return true;
            }
            return false;
        }
        else if (pieceColor == PieceColor.Black)
        {
            foreach (ChessPiece piece in board.WhitePieces)
            {
                if (piece.LegalPositions.Contains(_pos))
                    return true;
            }
            return false;
        }
        else return false;
    }


    [Button]
    public override bool MoveToPosition(Vector2Int newPos)
    {
        //if position does not exist in legal positions or attacks then it is not a legal move
        if (!LegalPositions.Contains(newPos) && !LegalAttacks.Contains(newPos)) return false;

        //take piece if move is an attack
        if (LegalAttacks.Contains(newPos))
            board.TakePiece(newPos);

        //update piece information to new positions
        Square newSquare = board.squares[newPos.x, newPos.y];
        transform.parent = newSquare.transform;
        newSquare.piece = this;
        square.piece = null;
        square = newSquare;
        currentPosition = square.coordinate;
        board.boardUpdate.Invoke();
        //move was successfull
        return true;
    }
}
