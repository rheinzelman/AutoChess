using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
Dictionary<char, int> ChessPieceType = new Dictionary<char, int>();

ChessPieceType.Add('p', 1);
ChessPieceType.Add('r', 2);
ChessPieceType.Add('k', 3);
ChessPieceType.Add('b', 4);
ChessPieceType.Add('q', 5);
ChessPieceType.Add('k', 6);

*/

public enum ChessPieceType
{
    p = 1,
    r = 2,
    n = 3,
    b = 4,
    q = 5,
    k = 6

}


public class ChessPiece : MonoBehaviour
{
    public int team;
    public int col;
    public int row;
    public ChessPieceType type;

}
