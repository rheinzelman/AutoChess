using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FENNamespace
{
    public class FENHandler : MonoBehaviour
    {
        public string board_FEN;
        private string board_state;
        private string turn;
        private string castling;
        private string passant;

        public FENHandler(string FEN)
        {

            board_FEN = FEN;
            string[] split = FEN.Split(' ');

            //split: 0 - board state, 1 - turn designation, 2 - castling availability, 3 - en passant squares
            this.board_state = split[0];
            //this.turn = split[1];
            //this.castling = split[2];
            //this.passant = split[3];
        }

        public char[,] getArray()
        {
            //initialize our 8x8 array to return
            char[,] result = new char[8, 8];

            // Initialize x and y
            int x = 0;
            int y = 0;

            foreach(char c in board_state)
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
                if (char.IsDigit(c))
                    for (int i = 0; i < char.GetNumericValue(c); i++)
                        result[x++, y] = '-';
            }

            return result;
        }

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

