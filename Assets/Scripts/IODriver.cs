using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Collections;
using System.Collections.Generic;
using System;

namespace IODriverNamespace
{

    public class IODriver : MonoBehaviour
    {

        private string initial_state;
        private string current_state;

        private int[,] keyArray = new int[,]
        {
            { 7, 6, 5, 4, 63, 62, 61, 60 },
            { 0, 1, 2, 3, 56, 57, 58, 59 },
            { 15, 14, 13, 12, 55, 54, 53, 52},
            { 8, 9, 10, 11, 48, 49, 50, 51 },
            { 23, 22, 21, 20, 47, 46, 45, 44 },
            { 16, 17, 18, 19, 40, 41, 42 ,43 },
            { 31, 30, 29, 28, 39, 38, 37, 36 },
            { 24, 25, 26, 27, 32, 33, 34 ,35 },
        };

        private int[,] keyArray2 = new int[,]
        {
            { 1, 9, 17, 25, 24, 16, 8, 0 },
            { 3, 11, 19, 27, 26, 18, 10, 2 },
            { 5, 13, 21, 29, 28, 20, 12, 4 },
            { 7, 15, 23, 31, 30, 22, 14, 6 },
            { 39, 47, 55, 63, 62, 54, 46, 38 },
            { 37, 45, 53, 61, 60, 52, 44, 36 },
            { 35, 43, 51, 59, 58, 50, 42, 34 },
            { 33, 41, 49, 57, 56, 48, 40, 32 }
        };

        public IODriver(string initial, string current)
        {
            this.initial_state = initial;
            this.current_state = current;
        }

        public int[,] boardToArray(string board_state)
        {
            int[,] returnArray = new int[8, 8];
            int[,] tempArray = new int[8, 8];

            int arrayIndex = 0;

            for(int i = 0; i < returnArray.GetLength(0); i++)
            {
                for(int j = 0; j < returnArray.GetLength(1); j++)
                {
                    tempArray[i, j] = board_state[arrayIndex] - '0'; 
                    arrayIndex++;
                }
            }

            arrayIndex = 0;

            for (int i = 0; i < returnArray.GetLength(0); i++)
            {
                for (int j = 0; j < returnArray.GetLength(1); j++)
                {
                    int arrayRow = keyArray2[i,j] % 8;
                    int arrayCol = (int)Math.Floor((double)(keyArray2[i,j] / 8));

                    //Debug.Log(arrayRow + "," + arrayCol);
                  
                    returnArray[arrayRow, arrayCol] = tempArray[i,j];
                    arrayIndex++;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Debug.Log(string.Format("{0},{1}: ", i, j) + returnArray[i,j]);
                }
            }

            return null;
        }

        public string GetUCIMove()
        {



            return null;

        }

        public string getInit()
        {
            return this.initial_state;
        }

    }
    

}