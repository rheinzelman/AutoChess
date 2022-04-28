using ChessGame;
using UnityEngine;

namespace Utils
{
    public static class NotationsHandler // : MonoBehaviour
    {
        public const string DEFAULT_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        private static int _horizontalSquares = 8;
        private static int _verticalSquares = 8;

        private static string _boardFen;
        private static string _piecePlacement;
        private static string _turn;
        private static string _castling;
        private static string _passant;

        public static void ProcessFEN(string FEN = "")
        {
            _boardFen = !string.IsNullOrEmpty(FEN) ? FEN : DEFAULT_FEN;

            var split = _boardFen.Split(' ');

            //split: 0 - board state, 1 - turn designation, 2 - castling availability, 3 - en passant squares
            _piecePlacement = split.Length > 1 ? split[0] : "";
            _turn = split.Length > 1 ? split[1] : "";
            _castling = split.Length > 2 ? split[2] : "";
            ;
            _passant = split.Length > 3 ? split[3] : "";
            ;
        }

        private static void SetBoardDimensions(int x, int y)
        {
            _horizontalSquares = x;
            _verticalSquares = y;
        }

        public static string GetPiecePlacement(char[,] charArray)
        {
            var placementStr = "";

            var whiteSpaces = 0;

            // var x = 0;
            // var y = 0;

            // while (y < _verticalSquares)
            // {
            //     if (x >= _horizontalSquares)
            //     {
            //         x = 0;
            //         whiteSpaces = 0;
            //         placementStr += '/';
            //         y++;
            //     }
            //
            //     placementStr += charArray[x, y];
            // }

            for (var y = 0; y < charArray.GetLength(1); y++)
            {
                for (var x = 0; x < charArray.GetLength(0); x++)
                    if (charArray[x, y] == '-')
                    {
                        whiteSpaces++;
                    }
                    else
                    {
                        // if (whiteSpaces > 0)
                        //     placementStr += whiteSpaces + charArray[x, y].ToString();
                        // else
                        //     placementStr += charArray[x, y];

                        placementStr += (whiteSpaces > 0 ? whiteSpaces.ToString() : "") + charArray[x, y];

                        whiteSpaces = 0;
                    }

                placementStr += (whiteSpaces > 0 ? whiteSpaces.ToString() : "") + "/";

                // if (whiteSpaces > 0)
                //     placementStr += whiteSpaces + "/";
                // else
                //     placementStr += "/";

                whiteSpaces = 0;
            }

            return placementStr.TrimEnd('/');
        }

        public static char[,] GetArray(string FEN = "")
        {
            if (string.IsNullOrEmpty(FEN))
            {
                if (string.IsNullOrEmpty(_piecePlacement))
                    ProcessFEN();
            }
            else
            {
                ProcessFEN(FEN);
            }

            //initialize our 8x8 array to return
            var result = new char[8, 8];

            // Initialize x and y
            var x = 0;
            var y = 0;

            foreach (var c in _piecePlacement)
            {
                // If the character is a '/' then start the next row and skip this iteration
                if (c == '/')
                {
                    x = 0;
                    y++;
                    continue;
                }

                // If the character is a letter, put it in the array
                if (char.IsLetter(c))
                    result[x++, y] = c;

                // If the character is a digit, place that many '-' in the array
                if (!char.IsDigit(c)) continue;
                for (var i = 0; i < char.GetNumericValue(c); i++)
                    result[x++, y] = '-';
            }

            return result;
        }

        public static void Print2DArray<T>(T[,] arr)
        {
            var printStr = "[\n";

            for (var y = 0; y < arr.GetLength(1); y++)
            {
                for (var x = 0; x < arr.GetLength(0); x++)
                    printStr += arr[x, y] + " ";

                printStr += '\n';
            }

            printStr += "]\n";

            Debug.Log("Array = " + printStr);
        }

        public static string GenerateFEN()
        {
            if (!(GameManager.Instance && BoardManager.Instance)) return "";

            var game = GameManager.Instance;
            var board = BoardManager.Instance;

            // Array of FEN components
            var args = new[]
            {
                /*   Piece Placement: */ GetPiecePlacement(board.BoardState),
                /*       Player Turn: */ game.PlayerTurn == PlayerColor.White ? "w" : "b",
                /*   Castling Rights: */ board.castlingRights,
                /* En Passant Square: */ board.enPassantSquare != null ? CoordinateToUCI(board.enPassantSquare.Item1) : "-",
                /*   Half Move Clock: */ game.halfMoveClock.ToString(),
                /*   Full Move Clock: */ game.fullMoveClock.ToString()
            };

            return string.Join(" ", args);
        }

        public static string CoordinateToUCI(Vector2Int vec)
        {
            return "abcdefgh"[vec.x] + (8 - vec.y).ToString();
        }

        public static Vector2Int UCIToCoordinate(string UCI)
        {
            var x = UCI[0] - 'a';
            var y = 8 - (UCI[1] - '0');

            return new Vector2Int(x, y);
        }

        // public char[,] BoolToChar(string bool_string)
        // {
        //
        //     char[,] result = new char[8, 8];
        //     int arrayIndex = 0;
        //
        //     for (int i = 0; i < bool_string.Length; i++)
        //     {
        //         //convert our position in the board_state_str into an 8x8 array readable form
        //         int arrayRow = arrayIndex % 8;
        //         int arrayCol = (int)Math.Floor((double)(arrayIndex / 8));
        //
        //         result[arrayCol, arrayRow] = bool_string[i];
        //         arrayIndex++;
        //     }
        //
        //     return result;
        // }
        //
        // // returns the current board_state as a FEN
        // public string getCurrentFEN(char [,] board_state)
        // {
        //     string returnFEN = "";
        //     int emptySpaces = 0;
        //
        //     for(int i = 0; i < 8; i++)
        //     {
        //         for(int j = 0; j < 8; j++)
        //         {
        //             if(board_state[j, i] != '-')
        //             {
        //                 if(emptySpaces > 0)
        //                 {
        //                     returnFEN += emptySpaces;
        //                     emptySpaces = 0;
        //                 }
        //                 returnFEN += board_state[j, i];
        //             }
        //             else
        //             {
        //                 emptySpaces++;
        //             }
        //         }
        //         if(emptySpaces > 0)
        //         {
        //             returnFEN += emptySpaces;
        //             emptySpaces = 0;
        //         }
        //         if(i != 7)
        //         {
        //             returnFEN += "/";
        //         }
        //     }
        //
        //     return returnFEN;
        // }
    }
}