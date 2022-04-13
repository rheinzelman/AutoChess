using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AutoChess.Utility.FENHandler
{
    public class FENHandler// : MonoBehaviour
    {
        public static string DEFAULT_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        
        public string board_FEN;
        private string board_state;
        private string turn;
        private string castling;
        private string passant;

        public FENHandler(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {

            board_FEN = fen;
            string[] split = fen.Split(' ');

            //split: 0 - board state, 1 - turn designation, 2 - castling availability, 3 - en passant squares
            this.board_state = split[0];
            //this.turn = split[1];
            //this.castling = split[2];
            //this.passant = split[3];
        }

        public char[,] GetArray()
        {
            //initialize our 8x8 array to return
            var result = new char[8, 8];

            // Initialize x and y
            var x = 0;
            var y = 0;

            foreach(var c in board_state)
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

        //Convert the board state portion of the FEN to an 8x8 array
        //public char[,] getArray()
        //{

        //    //initialize our 8x8 array to return
        //    char[,] result = new char[8, 8];

        //    //this keeps track of where we will be placing piece characters in the 8x8 array
        //    int arrayIndex = 0;

        //    //iterate through the entire board_state_str
        //    for (int i = 0; i < board_state.Length; i++)
        //    {
        //        //convert our position in the board_state_str into an 8x8 array readable form
        //        int arrayRow = arrayIndex % 8;
        //        int arrayCol = (int)Math.Floor((double)(arrayIndex / 8));

        //        //if we encounter a letter, assign it to the appropriate spot in the array and increment the arrayIndex
        //        if (Char.IsLetter(board_state[i]))
        //        {
        //            result[arrayCol, arrayRow] = board_state[i];
        //            arrayIndex++;
        //        }
        //        //if we encounter a slash do nothing. Here for completeness sake
        //        else if (board_state[i] == '/') { }
        //        //if we encounter a number, iterate the arrayIndex by that many times
        //        else if (Char.IsNumber(board_state[i]))
        //        {
        //            //what a load of bullshit, python would never hurt me like this
        //            for (int k = 0; k < (int)board_state[i] - '0'; k++)
        //            {
        //                result[arrayCol, arrayRow] = '-';
        //                arrayIndex++;
        //                arrayRow = arrayIndex % 8;
        //                arrayCol = (int)Math.Floor((double)(arrayIndex / 8));
        //            }
        //        }
        //    }

        //    return result;
        //}

        public char[,] BoolToChar(string bool_string)
        {

            char[,] result = new char[8, 8];
            int arrayIndex = 0;

            for (int i = 0; i < bool_string.Length; i++)
            {
                //convert our position in the board_state_str into an 8x8 array readable form
                int arrayRow = arrayIndex % 8;
                int arrayCol = (int)Math.Floor((double)(arrayIndex / 8));

                result[arrayCol, arrayRow] = bool_string[i];
                arrayIndex++;
            }

            return result;
        }

        // returns the current board_state as a FEN
        public string getCurrentFEN(char [,] board_state)
        {
            string returnFEN = "";
            int emptySpaces = 0;

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(board_state[j, i] != '-')
                    {
                        if(emptySpaces > 0)
                        {
                            returnFEN += emptySpaces;
                            emptySpaces = 0;
                        }
                        returnFEN += board_state[j, i];
                    }
                    else
                    {
                        emptySpaces++;
                    }
                }
                if(emptySpaces > 0)
                {
                    returnFEN += emptySpaces;
                    emptySpaces = 0;
                }
                if(i != 7)
                {
                    returnFEN += "/";
                }
            }

            return returnFEN;
        }


    }
}

