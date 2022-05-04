using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMax : MonoBehaviour
{
    //take in player turn board state depth of search
    //uses player turn to keep track of prefered move outcome
    //assumes optimal play from opponent
    //generates moves
    //evaulates moves
    //returns move with best expected outcome for player

    //mate in 5 better find the 5
    //openings

    public float maxEval;
    public float minEval;
    public float newEval;
    public string bestMove;
    public string temp;


    public (string _move, float eval) MiniMax(string x, int turn, int depth)
    {
        if (depth == 0 || GameOver(x))
        {
            return (null, Evaluate(x, turn));
        }
        List<string> newMoves = new List<string>();
        newMoves = GenerateMoves(x, turn);
        bestMove = newMoves[1];
        if (turn == 1)//white
        {
            maxEval = -999999;
            foreach (string move in newMoves)
            {
                (temp, newEval) = MiniMax(move, 0, depth - 1);
                if (newEval > maxEval)
                {
                    maxEval = newEval;
                    bestMove = move;
                }
            }
            return (bestMove, maxEval);
        }
        else //black
        {
            minEval = 999999;
            foreach (string move in newMoves)
            {
                (temp, newEval) = MiniMax(move, 1, depth - 1);
                if (newEval < minEval)
                {
                    minEval = newEval;
                    bestMove = move;
                }
            }
            return (bestMove, minEval);
        }

    }

    //take in board state and player turn
    //evaluates piece values white pos black neg 
    //evaluates piece locations
    //pieces at risk worth half 
    public char piece;
    public float evaluation;

    Dictionary<char, float> pieceValues = new Dictionary<char, float>()
    {
        { 'p', -1f}, {'q', -9f}, {'r', -5f}, {'b', -3.2f}, {'n', -3f}, {'k', -100f},
        { 'P', 1f}, {'Q', 9f}, {'R', 5f}, {'B', 3.2f}, {'N', 3f}, {'K', 100f}
    };

    Dictionary<int, float> BlackPawnLocationValue = new Dictionary<int, float>()
    {
        {0, -5f}, {1, -5f}, {2, -5f}, {3, -5f}, {4, -5f}, {5, -5f}, {6, -5f}, {7, -5f}, 
        {8, -4f}, {9, -4f}, {10, -4f}, {11, -4f}, {12, -4f}, {13, -4f}, {14, -4f}, {15, -4f},
        {16, -3f}, {17, -3f}, {18, -3f}, {19, -3f}, {20, -3f}, {21, -3f}, {22, -3f}, {23, -3f},
        {24, -2f}, {25, -2f}, {26, -2f}, {27, -2f}, {28, -2f}, {29, -2f}, {30, -2f}, {31, -2f},
        {32, -1f}, {33, -1f}, {34, -1f}, {35, -1f}, {36, -1f}, {37, -1f}, {38, -1f}, {39, -1f},
        {40, 0f}, {41, 0f}, {42, 0f}, {43, 0f}, {44, 0f}, {45, 0f}, {46, 0f}, {47, 0f},
        {48, 0f}, {49, 0f}, {50, 0f}, {51, 0f}, {52, 0f}, {53, 0f}, {54, 0f}, {55, 0f},
        {56, 0f}, {57, 0f}, {58, 0f}, {59, 0f}, {60, 0f}, {61, 0f}, {62, 0f}, {63, 0f}
    };

    Dictionary<int, float> WhitePawnLocationValue = new Dictionary<int, float>()
    {
        {0, 0f}, {1, 0f}, {2, 0f}, {3, 0f}, {4, 0f}, {5, 0f}, {6, 0f}, {7, 0f},
        {8, 0f}, {9, 0f}, {10, 0f}, {11, 0f}, {12, 0f}, {13, 0f}, {14, 0f}, {15, 0f},
        {16, 0f}, {17, 0f}, {18, 0f}, {19, 0f}, {20, 0f}, {21, 0f}, {22, 0f}, {23, 0f},
        {24, 1f}, {25, 1f}, {26, 1f}, {27, 1f}, {28, 1f}, {29, 1f}, {30, 1f}, {31, 1f},
        {32, 2f}, {33, 2f}, {34, 2f}, {35, 2f}, {36, 2f}, {37, 2f}, {38, 2f}, {39, 2f},
        {40, 3f}, {41, 3f}, {42, 3f}, {43, 3f}, {44, 3f}, {45, 3f}, {46, 3f}, {47, 3f},
        {48, 4f}, {49, 4f}, {50, 4f}, {51, 4f}, {52, 4f}, {53, 4f}, {54, 1f}, {55, 4f},
        {56, 5f}, {57, 5f}, {58, 5f}, {59, 5f}, {60, 5f}, {61, 5f}, {62, 5f}, {63, 5f}
    };

    Dictionary<int, float> BlackKnightLocationValue = new Dictionary<int, float>()
    {
       {0, -0f}, {1, -0f}, {2, -1f}, {3, -2f}, {4, -2f}, {5, -1f}, {6, -0f}, {7, -0f},
       {8, -0f}, {9, -1f}, {10, -2f}, {11, -3f}, {12, -3f}, {13, -2f}, {14, -1f}, {15, -0f},
       {16, -1f}, {17, -2f}, {18, -3f}, {19, -4f}, {20, -4f}, {21, -3f}, {22, -2f}, {23, -1f},
       {24, -2f}, {25, -3f}, {26, -4f}, {27, -5f}, {28, -5f}, {29, -4f}, {30, -3f}, {31, -2f},
       {32, -2f}, {33, -3f}, {34, -4f}, {35, -5f}, {36, -5f}, {37, -4f}, {38, -3f}, {39, -2f},
       {40, -1f}, {41, -2f}, {42, -3f}, {43, -4f}, {44, -4f}, {45, -3f}, {46, -2f}, {47, -1f},
       {48, -0f}, {49, -1f}, {50, -2f}, {51, -3f}, {52, -3f}, {53, -2f}, {54, -1f}, {55, -0f},
       {56, -0f}, {57, -0f}, {58, -1f}, {59, -2f}, {60, -2f}, {61, -1f}, {62, -0f}, {63, -0f}
    };

    Dictionary<int, float> WhiteKnightLocationValue = new Dictionary<int, float>()
    {
       {0, 0f}, {1, 0f}, {2, 1f}, {3, 2f}, {4, 2f}, {5, 1f}, {6, 0f}, {7, 0f},
       {8, 0f}, {9, 1f}, {10, 2f}, {11, 3f}, {12, 3f}, {13, 2f}, {14, 1f}, {15, 0f},
       {16, 1f}, {17, 2f}, {18, 3f}, {19, 4f}, {20, 4f}, {21, 3f}, {22, 2f}, {23, 1f},
       {24, 2f}, {25, 3f}, {26, 4f}, {27, 5f}, {28, 5f}, {29, 4f}, {30, 3f}, {31, 2f},
       {32, 2f}, {33, 3f}, {34, 4f}, {35, 5f}, {36, 5f}, {37, 4f}, {38, 3f}, {39, 2f},
       {40, 1f}, {41, 2f}, {42, 3f}, {43, 4f}, {44, 4f}, {45, 3f}, {46, 2f}, {47, 1f},
       {48, 0f}, {49, 1f}, {50, 2f}, {51, 3f}, {52, 3f}, {53, 2f}, {54, 1f}, {55, 0f},
       {56, 0f}, {57, 0f}, {58, 1f}, {59, 2f}, {60, 2f}, {61, 1f}, {62, 0f}, {63, 0f}
    };

    Dictionary<int, float> BlackBishopLocationValue = new Dictionary<int, float>()
    {
       {0, -0f}, {1, -0f}, {2, -1f}, {3, -2f}, {4, -2f}, {5, -1f}, {6, -0f}, {7, -0f},
       {8, -0f}, {9, -1f}, {10, -2f}, {11, -3f}, {12, -3f}, {13, -2f}, {14, -1f}, {15, -0f},
       {16, -1f}, {17, -2f}, {18, -3f}, {19, -4f}, {20, -4f}, {21, -3f}, {22, -2f}, {23, -1f},
       {24, -2f}, {25, -3f}, {26, -4f}, {27, -5f}, {28, -5f}, {29, -4f}, {30, -3f}, {31, -2f},
       {32, -2f}, {33, -3f}, {34, -4f}, {35, -5f}, {36, -5f}, {37, -4f}, {38, -3f}, {39, -2f},
       {40, -1f}, {41, -2f}, {42, -3f}, {43, -4f}, {44, -4f}, {45, -3f}, {46, -2f}, {47, -1f},
       {48, -0f}, {49, -0f}, {50, -0f}, {51, -0f}, {52, -0f}, {53, -0f}, {54, -0f}, {55, -0f},
       {56, -0f}, {57, -0f}, {58, -0f}, {59, -0f}, {60, -0f}, {61, -0f}, {62, -0f}, {63, -0f}
    };

    Dictionary<int, float> WhiteBishopLocationValue = new Dictionary<int, float>()
    {
       {0, 0f}, {1, 0f}, {2, 1f}, {3, 2f}, {4, 2f}, {5, 1f}, {6, 0f}, {7, 0f},
       {8, 0f}, {9, 1f}, {10, 2f}, {11, 3f}, {12, 3f}, {13, 2f}, {14, 1f}, {15, 0f},
       {16, 1f}, {17, 2f}, {18, 3f}, {19, 4f}, {20, 4f}, {21, 3f}, {22, 2f}, {23, 1f},
       {24, 2f}, {25, 3f}, {26, 4f}, {27, 5f}, {28, 5f}, {29, 4f}, {30, 3f}, {31, 2f},
       {32, 2f}, {33, 3f}, {34, 4f}, {35, 5f}, {36, 5f}, {37, 4f}, {38, 3f}, {39, 2f},
       {40, 1f}, {41, 2f}, {42, 3f}, {43, 4f}, {44, 4f}, {45, 3f}, {46, 2f}, {47, 1f},
       {48, 0f}, {49, 1f}, {50, 2f}, {51, 3f}, {52, 3f}, {53, 2f}, {54, 1f}, {55, 0f},
       {56, 0f}, {57, 0f}, {58, 1f}, {59, 2f}, {60, 2f}, {61, 1f}, {62, 0f}, {63, 0f}
    };

    Dictionary<int, float> BlackRookLocationValue = new Dictionary<int, float>()
    {
        {0, -5f}, {1, -5f}, {2, -5f}, {3, -4f}, {4, -4f}, {5, -5f}, {6, -5f}, {7, -5f},
        {8, -4f}, {9, -4f}, {10, -4f}, {11, -3f}, {12, -3f}, {13, -4f}, {14, -4f}, {15, -4f},
        {16, -3f}, {17, -3f}, {18, -3f}, {19, -2f}, {20, -2f}, {21, -3f}, {22, -3f}, {23, -3f},
        {24, -2f}, {25, -2f}, {26, -1f}, {27, -0f}, {28, -0f}, {29, -1f}, {30, -2f}, {31, -2f},
        {32, -1f}, {33, -1f}, {34, -0f}, {35, -0f}, {36, -0f}, {37, -0f}, {38, -1f}, {39, -1f},
        {40, -1f}, {41, -0f}, {42, -0f}, {43, -0f}, {44, -0f}, {45, -0f}, {46, -0f}, {47, -1f},
        {48, -0f}, {49, -0f}, {50, -0f}, {51, -0f}, {52, -0f}, {53, -0f}, {54, -0f}, {55, -0f},
        {56, -0f}, {57, -0f}, {58, -0f}, {59, -0f}, {60, -0f}, {61, -0f}, {62, -0f}, {63, -0f}
    };

    Dictionary<int, float> WhiteRookLocationValue = new Dictionary<int, float>()
    {
        {0, 0f}, {1, 0f}, {2, 0f}, {3, 0f}, {4, 0f}, {5, 0f}, {6, 0f}, {7, 0f},
        {8, 0f}, {9, 0f}, {10, 0f}, {11, 0f}, {12, 0f}, {13, 0f}, {14, 0f}, {15, 0f},
        {16, 1f}, {17, 0f}, {18, 0f}, {19, 0f}, {20, 0f}, {21, 0f}, {22, 0f}, {23, 1f},
        {24, 1f}, {25, 1f}, {26, 0f}, {27, 0f}, {28, 0f}, {29, 0f}, {30, 1f}, {31, 1f},
        {32, 2f}, {33, 2f}, {34, 1f}, {35, 0f}, {36, 0f}, {37, 1f}, {38, 2f}, {39, 2f},
        {40, 3f}, {41, 3f}, {42, 3f}, {43, 2f}, {44, 2f}, {45, 3f}, {46, 3f}, {47, 3f},
        {48, 4f}, {49, 4f}, {50, 4f}, {51, 3f}, {52, 3f}, {53, 4f}, {54, 1f}, {55, 4f},
        {56, 5f}, {57, 5f}, {58, 5f}, {59, 4f}, {60, 4f}, {61, 5f}, {62, 5f}, {63, 5f}
    };

    Dictionary<int, float> BlackQueenLocationValue = new Dictionary<int, float>()
    {
       {0, -0f}, {1, -0f}, {2, -1f}, {3, -2f}, {4, -2f}, {5, -1f}, {6, -0f}, {7, -0f},
       {8, -0f}, {9, -1f}, {10, -2f}, {11, -3f}, {12, -3f}, {13, -2f}, {14, -1f}, {15, -0f},
       {16, -1f}, {17, -2f}, {18, -3f}, {19, -4f}, {20, -4f}, {21, -3f}, {22, -2f}, {23, -1f},
       {24, -2f}, {25, -3f}, {26, -4f}, {27, -5f}, {28, -5f}, {29, -4f}, {30, -3f}, {31, -2f},
       {32, -2f}, {33, -3f}, {34, -4f}, {35, -5f}, {36, -5f}, {37, -4f}, {38, -3f}, {39, -2f},
       {40, -1f}, {41, -2f}, {42, -3f}, {43, -4f}, {44, -4f}, {45, -3f}, {46, -2f}, {47, -1f},
       {48, -0f}, {49, -0f}, {50, -0f}, {51, -0f}, {52, -0f}, {53, -0f}, {54, -0f}, {55, -0f},
       {56, -0f}, {57, -0f}, {58, -0f}, {59, -0f}, {60, -0f}, {61, -0f}, {62, -0f}, {63, -0f}
    };

    Dictionary<int, float> WhiteQueenLocationValue = new Dictionary<int, float>()
    {
       {0, 0f}, {1, 0f}, {2, 1f}, {3, 2f}, {4, 2f}, {5, 1f}, {6, 0f}, {7, 0f},
       {8, 0f}, {9, 1f}, {10, 2f}, {11, 3f}, {12, 3f}, {13, 2f}, {14, 1f}, {15, 0f},
       {16, 1f}, {17, 2f}, {18, 3f}, {19, 4f}, {20, 4f}, {21, 3f}, {22, 2f}, {23, 1f},
       {24, 2f}, {25, 3f}, {26, 4f}, {27, 5f}, {28, 5f}, {29, 4f}, {30, 3f}, {31, 2f},
       {32, 2f}, {33, 3f}, {34, 4f}, {35, 5f}, {36, 5f}, {37, 4f}, {38, 3f}, {39, 2f},
       {40, 1f}, {41, 2f}, {42, 3f}, {43, 4f}, {44, 4f}, {45, 3f}, {46, 2f}, {47, 1f},
       {48, 0f}, {49, 1f}, {50, 2f}, {51, 3f}, {52, 3f}, {53, 2f}, {54, 1f}, {55, 0f},
       {56, 0f}, {57, 0f}, {58, 1f}, {59, 2f}, {60, 2f}, {61, 1f}, {62, 0f}, {63, 0f}
    };

    Dictionary<int, float> BlackKingLocationValue = new Dictionary<int, float>()
    {
       {0, -0f}, {1, -0f}, {2, -0f}, {3, -0f}, {4, -0f}, {5, -0f}, {6, -0f}, {7, -0f},
       {8, -0f}, {9, -0f}, {10, -0f}, {11, -0f}, {12, -0f}, {13, -0f}, {14, -0f}, {15, -0f},
       {16, -0f}, {17, -0f}, {18, -0f}, {19, -0f}, {20, -0f}, {21, -0f}, {22, -0f}, {23, -0f},
       {24, -0f}, {25, -0f}, {26, -0f}, {27, -0f}, {28, -0f}, {29, -0f}, {30, -0f}, {31, -0f},
       {32, -0f}, {33, -0f}, {34, -0f}, {35, -0f}, {36, -0f}, {37, -0f}, {38, -0f}, {39, -0f},
       {40, -1f}, {41, -1f}, {42, -1f}, {43, -1f}, {44, -1f}, {45, -1f}, {46, -1f}, {47, -1f},
       {48, -2f}, {49, -2f}, {50, -2f}, {51, -2f}, {52, -2f}, {53, -2f}, {54, -2f}, {55, -2f},
       {56, -4f}, {57, -4f}, {58, -4f}, {59, -3f}, {60, -3f}, {61, -4f}, {62, -5f}, {63, -4f}
    };

    Dictionary<int, float> WhiteKingLocationValue = new Dictionary<int, float>()
    {
       {0, 4f}, {1, 4f}, {2, 4f}, {3, 3f}, {4, 3f}, {5, 4f}, {6, 5f}, {7, 4f},
       {8, 2f}, {9, 3f}, {10, 2f}, {11, 2f}, {12, 2f}, {13, 2f}, {14, 2f}, {15, 2f},
       {16, 1f}, {17, 1f}, {18, 1f}, {19, 1f}, {20, 1f}, {21, 1f}, {22, 1f}, {23, 1f},
       {24, 0f}, {25, 0f}, {26, 0f}, {27, 0f}, {28, 0f}, {29, 0f}, {30, 0f}, {31, 0f},
       {32, 0f}, {33, 0f}, {34, 0f}, {35, 0f}, {36, 0f}, {37, 0f}, {38, 0f}, {39, 0f},
       {40, 0f}, {41, 0f}, {42, 0f}, {43, 0f}, {44, 0f}, {45, 0f}, {46, 0f}, {47, 0f},
       {48, 0f}, {49, 0f}, {50, 0f}, {51, 0f}, {52, 0f}, {53, 0f}, {54, 0f}, {55, 0f},
       {56, 0f}, {57, 0f}, {58, 0f}, {59, 0f}, {60, 0f}, {61, 0f}, {62, 0f}, {63, 0f}
    };



    //example string "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"
    float Evaluate(string _board, int _turn)
    {
        evaluation = 0;
        boardArr = ConvertToArr(_board);
        int squareSpace = 0;
        char piece;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {   

                if (pieceValues.ContainsKey(boardArr[i,j]))
                {
                    evaluation += pieceValues[boardArr[i,j]];
                    piece= boardArr[i,j];
                    switch (piece)
                    { 
                        case ('p'):
                            evaluation += BlackPawnLocationValue[squareSpace];
                            break;
                        case ('P'):
                            evaluation += WhitePawnLocationValue[squareSpace];
                            break;
                        case ('n'):
                            evaluation += BlackKnightLocationValue[squareSpace];
                            break;
                        case ('N'):
                            evaluation += WhiteKnightLocationValue[squareSpace];
                            break;
                        case ('b'):
                            evaluation += BlackBishopLocationValue[squareSpace];
                            break;
                        case ('B'):
                            evaluation += WhiteBishopLocationValue[squareSpace];
                            break;
                        case ('r'):
                            evaluation += BlackRookLocationValue[squareSpace];
                            break;
                        case ('R'):
                            evaluation += WhiteRookLocationValue[squareSpace];
                            break;
                        case ('q'):
                            evaluation += BlackQueenLocationValue[squareSpace];
                            break;
                        case ('Q'):
                            evaluation += WhiteQueenLocationValue[squareSpace];
                            break;
                        case ('k'):
                            evaluation += BlackKingLocationValue[squareSpace];
                            break;
                        case ('K'):
                            evaluation += WhiteKingLocationValue[squareSpace];
                            break;
                        default:
                            break;
                    }
                }
            squareSpace++;
            }
        }
        return evaluation;
    }

    //if either king is missing the game is over
    public bool GameOver(string x)
    {
        if (!(x.Contains("k")) || !(x.Contains("K")))
            return true;
        else
            return false;
    }

    //take in player turn and board state
    //generator all possible moves
    public char _piece;
    public char[,] boardArr;
    public string color;
    public string _type;

    //InitializePiece(new GameObject("Pawn", typeof(Pawn)), color);
    public List<string> GenerateMoves(string _board, int _turn)
    {
        boardArr = ConvertToArr(_board);
        List<string> moves = new List<string>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _piece = char.ToLower(boardArr[i,j]);
                if (pieceValues.ContainsKey(_piece))
                {
                    switch (_piece)
                    {
                        case ('p'):
                            ChessPiece BlackPawn = InitializePiece(new GameObject("Pawn", typeof(Pawn)), PieceColor.Black);
                            break;
                        case ('P'):
                            ChessPiece WhitePawn = InitializePiece(new GameObject("Pawn", typeof(Pawn)), PieceColor.White);
                            break;
                        case ('n'):
                            ChessPiece BlackKnight = InitializePiece(new GameObject("Knight", typeof(Knight)), PieceColor.Black);
                            break;
                        case ('N'):
                            ChessPiece WhiteKnight = InitializePiece(new GameObject("Knight", typeof(Knight)), PieceColor.White);
                            break;
                        case ('b'):
                            ChessPiece BlackBishop = InitializePiece(new GameObject("Bishop", typeof(Bishop)), PieceColor.Black);
                            break;
                        case ('B'):
                            ChessPiece WhiteBishop = InitializePiece(new GameObject("Bishop", typeof(Bishop)), PieceColor.White);
                            break;
                        case ('r'):
                            ChessPiece BlackRook = InitializePiece(new GameObject("Rook", typeof(Rook)), PieceColor.Black);
                            break;
                        case ('R'):
                            ChessPiece WhiteRook = InitializePiece(new GameObject("Rook", typeof(Rook)), PieceColor.White);
                            break;
                        case ('q'):
                            ChessPiece BlackQueen = InitializePiece(new GameObject("Queen", typeof(Queen)), PieceColor.Black);
                            break;
                        case ('Q'):
                            ChessPiece WhiteQueen = InitializePiece(new GameObject("Queen", typeof(Queen)), PieceColor.White);
                            break;
                        case ('k'):
                            ChessPiece BlackKing = InitializePiece(new GameObject("King", typeof(King)), PieceColor.Black);
                            break;
                        case ('K'):
                            ChessPiece WhiteKing = InitializePiece(new GameObject("King", typeof(King)), PieceColor.White);
                            break;
                        default:
                            break;
                    }
                    //moves.Add(piece.FindLegalPositions());

                }

            }

        }
        return moves;
    }

    public char[,] ConvertToArr(string x)
    {
        string[] subStrings = x.Split('/');
        char[,] charArr = new char['-', '-'];
        char fenChar;
        int spaces;
        for (int i = 8; i > 0; i--)
        {
            for (int j = 0; j < 8; j++)
            {
                fenChar = subStrings[i][j];
                if (int.TryParse(fenChar.ToString(), out spaces))
                {
                    for (int k = 0; k < spaces; k++)
                    {
                        charArr[i, j] = '-';
                        j++;
                    }
                }
                else
                    charArr[i, j] = subStrings[i][j];
            }
        }
        return charArr;

    }
}
