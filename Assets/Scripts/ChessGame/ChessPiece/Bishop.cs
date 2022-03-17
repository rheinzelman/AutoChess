using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    [Button]
    public override void FindLegalPositions()
    {
        //Clear lists
        LegalAttacks.Clear();
        LegalPositions.Clear();
        //look in the four diagonal directions and add moves to legal positions until you find board bounds or a piece,
        //if piece is of opposing color add move to LegalAttacks
        LookQuad1();
        LookQuad2();
        LookQuad3();
        LookQuad4();
    }

    private void LookQuad1()
    {
        Vector2Int newPos = new Vector2Int(currentPosition.x + 1, currentPosition.y + 1);
        while (board.IsValidCoordinate(newPos) && !board.HasPieceAt(newPos))
        {
            LegalPositions.Add(newPos);
                newPos = new Vector2Int(newPos.x + 1, newPos.y + 1);
        }
        if (board.HasPieceAt(newPos) && board.GetPieceAt(newPos).pieceColor != pieceColor)
            LegalAttacks.Add(newPos);
    }
    private void LookQuad2()
    {
        Vector2Int newPos = new Vector2Int(currentPosition.x - 1, currentPosition.y + 1);
        while (board.IsValidCoordinate(newPos) && !board.HasPieceAt(newPos))
        {
            LegalPositions.Add(newPos);
            newPos = new Vector2Int(newPos.x - 1, newPos.y + 1);
        }
        if (board.HasPieceAt(newPos) && board.GetPieceAt(newPos).pieceColor != pieceColor)
            LegalAttacks.Add(newPos);
    }
    private void LookQuad3()
    {
        Vector2Int newPos = new Vector2Int(currentPosition.x - 1, currentPosition.y - 1);
        while (board.IsValidCoordinate(newPos) && !board.HasPieceAt(newPos))
        {
            LegalPositions.Add(newPos);
            newPos = new Vector2Int(newPos.x - 1, newPos.y - 1);
        }
        if (board.HasPieceAt(newPos) && board.GetPieceAt(newPos).pieceColor != pieceColor)
            LegalAttacks.Add(newPos);
    }
    private void LookQuad4()
    {
        Vector2Int newPos = new Vector2Int(currentPosition.x + 1, currentPosition.y - 1);
        while (board.IsValidCoordinate(newPos) && !board.HasPieceAt(newPos))
        {
            LegalPositions.Add(newPos);
            newPos = new Vector2Int(newPos.x + 1, newPos.y - 1);
        }
        if (board.HasPieceAt(newPos) && board.GetPieceAt(newPos).pieceColor != pieceColor)
            LegalAttacks.Add(newPos);
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
