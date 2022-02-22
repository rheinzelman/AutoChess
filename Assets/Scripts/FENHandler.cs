using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;
using System;

namespace FEN
{
    public class FENHandler
    {

        private string board_state;
        private string turn;
        private string castling;
        private string passant;

        public FENHandler(string FEN)
        {
            string[] split = FEN.Split(' ');
            //split: 0 - board state, 1 - turn designation, 2 - castling availability, 3 - en passant squares
            this.board_state = split[0];
            this.turn = split[1];
            this.castling = split[2];
            this.passant = split[3];
        }

        //Convert the board state portion of the FEN to an 8x8 array
        public char[,] getArray()
        {

            //initialize our 8x8 array to return
            char[,] result = new char[8, 8];

            //this keeps track of where we will be placing piece characters in the 8x8 array
            int arrayIndex = 0;

            //iterate through the entire board_state_str
            for (int i = 0; i < board_state.Length; i++)
            {   
                //convert our position in the board_state_str into an 8x8 array readable form
                int arrayRow = arrayIndex % 8;
                int arrayCol = (int)Math.Floor((double)(arrayIndex / 8));

                //if we encounter a letter, assign it to the appropriate spot in the array and increment the arrayIndex
                if (Char.IsLetter(board_state[i]))
                {
                    result[arrayCol, arrayRow] = board_state[i];
                    arrayIndex++;
                }
                //if we encounter a slash do nothing. Here for completeness sake
                else if (board_state[i] == '/') { }
                //if we encounter a number, iterate the arrayIndex by that many times
                else if (Char.IsNumber(board_state[i]))
                {
                    //what a load of bullshit, python would never hurt me like this
                    for (int k = 0; k < (int)board_state[i] - '0'; k++)
                    {
                        result[arrayCol, arrayRow] = '-';
                        arrayIndex++;
                        arrayRow = arrayIndex % 8;
                        arrayCol = (int)Math.Floor((double)(arrayIndex / 8));

                    }

                }

            }

            return result;

        }

        public string getTurn()
        {
            return turn;
        } 
    }
}

