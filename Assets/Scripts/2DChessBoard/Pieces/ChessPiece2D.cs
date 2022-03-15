using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ChessPieceType
{
    p = 1,
    r = 2,
    n = 3,
    b = 4,
    q = 5,
    k = 6

}


public class ChessPiece2D : MonoBehaviour
{
    public int team;
    public int col;
    public int row;
    public ChessPieceType type;

    public void DestroyChessPiece()
    {
        Debug.Log("one: " + this.gameObject);
        Destroy(this.gameObject);
        Debug.Log("two: " + this.gameObject);
    }

}
