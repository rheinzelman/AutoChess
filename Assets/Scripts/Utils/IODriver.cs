using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;
using BoardDriverNamespace;

namespace IODriverNamespace
{
    public class IODriver : MonoBehaviour
    {

        public static SerialPort sp = new SerialPort("COM3", 115200);

        private string initial_state;
        private string current_state;

        private int[,] keyArray = new int[,]
        {
            /*{ 7, 6, 5, 4, 12 , 13, 14, 15 },
            { 23, 22, 21, 20, 28, 29, 30, 31 },
            { 39, 38, 37, 36, 44, 45, 46, 47 },
            { 55, 54, 53, 52, 60, 61, 62, 63 },
            { 59, 58, 57, 56, 48, 49, 50, 51 },
            { 43, 42, 41, 40, 32, 33, 34, 35 },
            { 27, 26, 25, 24, 16, 17, 18, 19 },
            { 11, 10, 9, 8, 0, 1, 2, 3 }*/

            { 0, 8, 16, 24, 25, 17, 9, 1 },
            { 2, 10, 18, 26, 27, 19, 11 ,3 },
            { 4, 12, 20, 28, 29, 21, 13, 5 },
            { 6, 14, 22, 30, 31, 23, 15, 7 },
            { 38, 46, 54, 62, 63, 55, 45, 37 },
            { 36, 44, 52, 60, 61, 53, 45, 35 },
            { 34, 42, 50, 58, 59, 51, 43, 35 },
            { 32, 40, 48, 56, 57, 49, 41, 33 }
        };

        // physical board parameters
        int step;
        float[,] positionArray;

        public IODriver()
        {

        }

        public int[,] boardToArray()
        {

            string board_state = ReadArray();

            int[,] returnArray = new int[8, 8];

            int arrayIndex = 0;

            for (int i = 0; i < returnArray.GetLength(0); i++)
            {
                for (int j = 0; j < returnArray.GetLength(1); j++)
                {
                    int arrayRow = keyArray[i, j] % 8;
                    int arrayCol = (int)Math.Floor((double)(keyArray[i, j] / 8));

                    returnArray[arrayRow, arrayCol] = board_state[arrayIndex] - '0';
                    arrayIndex++;
                }
            }
            return returnArray;
        }

        public bool checkDifference(int[,] arr)
        {
            int sum = 0;

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if(arr[i,j] != 0)
                    sum += 1;
                }
            }

            if(sum == 2) {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<Vector2Int> getDifference(int[,] initial, int[,] final)
        {

            //A list containing the initial and final piece movement
            List<Vector2Int> returnValue = new List<Vector2Int>();
            //placeholder in the list to access later
            returnValue.Add(new Vector2Int());
            returnValue.Add(new Vector2Int());

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //subtract initial and final board states
                    final[i, j] = final[i, j] - initial[i, j];
                    //-1 will represent the from square and 1 will represent the to square
                    if (final[i, j] == -1)
                    {
                        returnValue[0] = new Vector2Int(i, j);
                    }
                    else if (final[i, j] == 1)
                    {
                        returnValue[1] = new Vector2Int(i, j);
                    }
                }
            }

            //ensure that there is only one piece movement picked up from the physical board
            //if there isn't return an empty Vector2Int List
            if(checkDifference(final) == true)
            {
                return returnValue;
            }
            else
            {
                Debug.Log("getDifference Error");
                return new List<Vector2Int>();
            }
        }

        public void sendMove(string input)
        {
            
        }

        /*public void OpenConnection()
        {
            if (sp != null)
            {
                if (sp.IsOpen)
                {
                    sp.Close();
                    print("closing Port, Port was already open");
                }
                else
                {
                    sp.Open();
                    sp.WriteTimeout = 200;
                    sp.ReadTimeout = 200;
                }
            }
            else
            {
                if (sp.IsOpen)
                {
                    print("Port Is already Open");
                }
                else
                {
                    print("Port == null");
                }
            }
        }*/
            
        public static string ReadArray()
        {
            sp.Open();
            string Sensor = sp.ReadLine();
            sp.Close();

            return Sensor;
        }

    }
    

}







































