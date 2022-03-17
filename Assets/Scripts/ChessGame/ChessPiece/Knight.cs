using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    private List<Vector2Int> possibleMoves = new List<Vector2Int>();
    
    [Button]
    public override void FindLegalPositions()
    {
        //Clear lists
        possibleMoves.Clear();
        LegalAttacks.Clear();
        LegalPositions.Clear();
        
        //generate the 8 possible moves for a knight from current position
        Vector2Int _pos = new Vector2Int(currentPosition.x + 1, currentPosition.y + 2);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x + 2, currentPosition.y + 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x + 1, currentPosition.y - 2);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x + 2, currentPosition.y - 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 1, currentPosition.y + 2);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 2, currentPosition.y + 1);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 1, currentPosition.y - 2);
        possibleMoves.Add(_pos);
        _pos = new Vector2Int(currentPosition.x - 2, currentPosition.y - 1);
        possibleMoves.Add(_pos);

        //iterate through each position and check for valid coordinate and for a piece
        //if a piece is of opposite color exists then move is added to legal attacks
        //if move is valid coordinate and no piece then move is added to legal positions
        foreach (Vector2Int pos in possibleMoves)
        {
            if (board.HasPieceAt(pos) && board.GetPieceAt(pos).pieceColor != pieceColor)
                    LegalAttacks.Add(pos);
            if (board.IsValidCoordinate(pos) && !board.HasPieceAt(pos))
                LegalPositions.Add(pos);
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
