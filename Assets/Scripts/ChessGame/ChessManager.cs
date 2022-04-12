using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FENNamespace;
using AutoChess.ManagerComponents;

public class ChessManager : MonoBehaviour
{
    private BoardManager board;
    private Board2D board2D;

    // Chess Settings
    public int horizontalSquares = 8;
    public int verticalSquares = 8;

    // Board code from Ray
    private string DEFAULT_FEN = "r3kb1r/pppnpppp/4qnb1/8/2NP1BP1/2N4P/PPP2P2/R2QKB1R";
    public char[,] board_state;

    private void Awake()
    {
        FENHandler FENObject = new FENHandler(DEFAULT_FEN);
        board_state = FENObject.getArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        board ??= GetComponentInChildren<BoardManager>();
        board2D ??= GetComponentInChildren<Board2D>();

        board.chessManager = this;
        board.board2D = board2D;

        board2D.chessManager = this;
        board2D.boardManager = board;

        board.horizontalSquares = horizontalSquares;
        board.verticalSquares = verticalSquares;

        board2D.TILE_COUNT_X = horizontalSquares;
        board2D.TILE_COUNT_Y = verticalSquares;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
