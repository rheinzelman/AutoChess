using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    private List<Vector2Int> possibleMoves = new List<Vector2Int>();
    private List<Vector2Int> castleSquares = new List<Vector2Int>();

    [Button]
    public override void FindLegalPositions()
    {
        possibleMoves.Clear();
        LegalAttacks.Clear();
        LegalPositions.Clear();
        castleSquares.Clear();
        if (pieceColor == PieceColor.White)
        {
            if (!moved && !board.HasPieceAt(new Vector2Int(5, 7)) && !board.HasPieceAt(new Vector2Int(6, 7)) && board.HasPieceAt(new Vector2Int(7, 7)) && !board.GetPieceAt(new Vector2Int(7, 7)).moved)
            {
                castleSquares.Add(new Vector2Int(6, 7));
                LegalPositions.Add(new Vector2Int(6, 7));
            }
            if (!moved && !board.HasPieceAt(new Vector2Int(3, 7)) && !board.HasPieceAt(new Vector2Int(2, 7)) && !board.HasPieceAt(new Vector2Int(1, 7)) && board.HasPieceAt(new Vector2Int(0, 7)) && !board.GetPieceAt(new Vector2Int(0, 7)).moved)
            {
                castleSquares.Add(new Vector2Int(2, 7));
                LegalPositions.Add(new Vector2Int(2, 7));
            }
        }
        if (pieceColor == PieceColor.Black)
        {
            if (!moved && !board.HasPieceAt(new Vector2Int(5, 0)) && !board.HasPieceAt(new Vector2Int(6, 0)) && board.HasPieceAt(new Vector2Int(7, 0)) && !board.GetPieceAt(new Vector2Int(7, 0)).moved)
            {
                castleSquares.Add(new Vector2Int(6, 0));
                LegalPositions.Add(new Vector2Int(6, 0));
            }
            if (!moved && !board.HasPieceAt(new Vector2Int(3, 0)) && !board.HasPieceAt(new Vector2Int(2, 0)) && !board.HasPieceAt(new Vector2Int(1, 0)) && board.HasPieceAt(new Vector2Int(0, 0)) && !board.GetPieceAt(new Vector2Int(0, 0)).moved)
            {
                castleSquares.Add(new Vector2Int(2, 0));
                LegalPositions.Add(new Vector2Int(2, 0));
            }
        }
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
            {
                if (board.HasPieceAt(pos) && board.GetPieceAt(pos).pieceColor != pieceColor)
                    LegalAttacks.Add(pos);
                if (board.IsValidCoordinate(pos) && !board.HasPieceAt(pos))
                    LegalPositions.Add(pos);
            }
        }
    }

    [Button]
    public override bool MoveToPosition(Vector2Int newPos)
    {
        //if position does not exist in legal positions or attacks then it is not a legal move
        if (!LegalPositions.Contains(newPos) && !LegalAttacks.Contains(newPos)) return false;
        //take piece if move is an attack
        if (LegalAttacks.Contains(newPos))
            board.TakePiece(newPos);
        if (castleSquares.Contains(newPos))
            board.Castle(newPos);
        //update piece information to new positions
        Square newSquare = board.squares[newPos.x, newPos.y];
        transform.parent = newSquare.transform;
        newSquare.piece = this;
        square.piece = null;
        square = newSquare;
        currentPosition = square.coordinate;
        board.boardUpdate.Invoke();
        moved = true;
        //move was successfull
        return true;
    }
}
