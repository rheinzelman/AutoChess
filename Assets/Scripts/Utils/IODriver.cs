using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

namespace IODriverNamespace
{
    public class IODriver : MonoBehaviour
    {

        public static SerialPort sp = new SerialPort("COM3", 115200);

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
            OpenConnection();
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
                    sum += arr[i, j];
                }
            }

            if(sum == 0) {
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
            Debug.Log("sendMove");
            sp.Write("$H");
            sp.Write("X100Y100");
        }

        public void OpenConnection()
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

            sp.Write("$H");
            sp.Write("X100Y100");
        }
            
        public static string ReadArray()
        {
            sp.Open();
            string Sensor = sp.ReadLine();
            sp.Close();

            return Sensor;
        }

    }
    

}







































