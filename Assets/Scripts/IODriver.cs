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
            { 1, 9, 17, 25, 24, 16, 8, 0 },
            { 3, 11, 19, 27, 26, 18, 10, 2 },
            { 5, 13, 21, 29, 28, 20, 12, 4 },
            { 7, 15, 23, 31, 30, 22, 14, 6 },
            { 39, 47, 55, 63, 62, 54, 46, 38 },
            { 37, 45, 53, 61, 60, 52, 44, 36 },
            { 35, 43, 51, 59, 58, 50, 42, 34 },
            { 33, 41, 49, 57, 56, 48, 40, 32 }
        };

        public IODriver()
        {
            this.initial_state = null;
            this.current_state = null;
        }

        public int[,] boardToArray(string board_state)
        {
            int[,] returnArray = new int[8, 8];

            int arrayIndex = 0;

            for (int i = 0; i < returnArray.GetLength(0); i++)
            {
                for (int j = 0; j < returnArray.GetLength(1); j++)
                {
                    int arrayRow = keyArray[i,j] % 8;
                    int arrayCol = (int)Math.Floor((double)(keyArray[i,j] / 8));
                  
                    returnArray[arrayRow, arrayCol] = board_state[arrayIndex] - '0';
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