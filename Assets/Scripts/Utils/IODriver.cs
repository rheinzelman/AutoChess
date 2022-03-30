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

        public static SerialPort sp;// = new SerialPort("COM3", 115200);

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

        public Dictionary<string, string> GRBLDict = new Dictionary<string, string>();

        private void Start()
        {

            // GRBL COORDINATE DICTIONARY
            // h notates a half square position, used for knights

            // A COLS

            GRBLDict.Add("a1","X100Y200");
            GRBLDict.Add("a2","");
            GRBLDict.Add("a3","");
            GRBLDict.Add("a4","");
            GRBLDict.Add("a5","");
            GRBLDict.Add("a6","");
            GRBLDict.Add("a7","");
            GRBLDict.Add("a8","");

            GRBLDict.Add("a1h", "");
            GRBLDict.Add("a2h", "");
            GRBLDict.Add("a3h", "");
            GRBLDict.Add("a4h", "");
            GRBLDict.Add("a5h", "");
            GRBLDict.Add("a6h", "");
            GRBLDict.Add("a7h", "");
            GRBLDict.Add("a8h", "");
            
            // B COLS

            GRBLDict.Add("b1", "");
            GRBLDict.Add("b2", "");
            GRBLDict.Add("b3", "");
            GRBLDict.Add("b4", "");
            GRBLDict.Add("b5", "");
            GRBLDict.Add("b6", "");
            GRBLDict.Add("b7", "");
            GRBLDict.Add("b8", "");

            GRBLDict.Add("b1h", "");
            GRBLDict.Add("b2h", "");
            GRBLDict.Add("b3h", "");
            GRBLDict.Add("b4h", "");
            GRBLDict.Add("b5h", "");
            GRBLDict.Add("b6h", "");
            GRBLDict.Add("b7h", "");
            GRBLDict.Add("b8h", "");

            // C COLS

            GRBLDict.Add("c1", "");
            GRBLDict.Add("c2", "");
            GRBLDict.Add("c3", "");
            GRBLDict.Add("c4", "");
            GRBLDict.Add("c5", "");
            GRBLDict.Add("c6", "");
            GRBLDict.Add("c7", "");
            GRBLDict.Add("c8", "");

            GRBLDict.Add("c1h", "");
            GRBLDict.Add("c2h", "");
            GRBLDict.Add("c3h", "");
            GRBLDict.Add("c4h", "");
            GRBLDict.Add("c5h", "");
            GRBLDict.Add("c6h", "");
            GRBLDict.Add("c7h", "");
            GRBLDict.Add("c8h", "");

            // D COLS

            GRBLDict.Add("d1", "");
            GRBLDict.Add("d2", "");
            GRBLDict.Add("d3", "");
            GRBLDict.Add("d4", "");
            GRBLDict.Add("d5", "");
            GRBLDict.Add("d6", "");
            GRBLDict.Add("d7", "");
            GRBLDict.Add("d8", "");
           
            GRBLDict.Add("d1h", "");
            GRBLDict.Add("d2h", "");
            GRBLDict.Add("d3h", "");
            GRBLDict.Add("d4h", "");
            GRBLDict.Add("d5h", "");
            GRBLDict.Add("d6h", "");
            GRBLDict.Add("d7h", "");
            GRBLDict.Add("d8h", "");

            // E COLS

            GRBLDict.Add("e1", "");
            GRBLDict.Add("e2", "");
            GRBLDict.Add("e3", "");
            GRBLDict.Add("e4", "");
            GRBLDict.Add("e5", "");
            GRBLDict.Add("e6", "");
            GRBLDict.Add("e7", "");
            GRBLDict.Add("e8", "");

            GRBLDict.Add("e1h", "");
            GRBLDict.Add("e2h", "");
            GRBLDict.Add("e3h", "");
            GRBLDict.Add("e4h", "");
            GRBLDict.Add("e5h", "");
            GRBLDict.Add("e6h", "");
            GRBLDict.Add("e7h", "");
            GRBLDict.Add("e8h", "");

            // F COLS

            GRBLDict.Add("f1", "");
            GRBLDict.Add("f2", "");
            GRBLDict.Add("f3", "");
            GRBLDict.Add("f4", "");
            GRBLDict.Add("f5", "");
            GRBLDict.Add("f6", "");
            GRBLDict.Add("f7", "");
            GRBLDict.Add("f8", "");

            GRBLDict.Add("f1h", "");
            GRBLDict.Add("f2h", "");
            GRBLDict.Add("f3h", "");
            GRBLDict.Add("f4h", "");
            GRBLDict.Add("f5h", "");
            GRBLDict.Add("f6h", "");
            GRBLDict.Add("f7h", "");
            GRBLDict.Add("f8h", "");

            // G COLS

            GRBLDict.Add("g1", "");
            GRBLDict.Add("g2", "");
            GRBLDict.Add("g3", "");
            GRBLDict.Add("g4", "");
            GRBLDict.Add("g5", "");
            GRBLDict.Add("g6", "");
            GRBLDict.Add("g7", "");
            GRBLDict.Add("g8", "");

            GRBLDict.Add("MAGON", "");
            GRBLDict.Add("MAGOFF", "");

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

        public void performtandardMove(string square1, string square2)
        {
            moveCoreXY(square1);
            activateMagnet(true);
            moveCoreXY(square2);
            activateMagnet(false);
        }

        public void performKnightMove(string square1, string square2)
        {

            string intermediarySquare;

            if(knightDirection(square1, square2) == true)
            {
                
                moveCoreXY(square1);
                activateMagnet(true);

                // if the knight is moving in the right direction
                // redefine square1 to be a half square to the right
                // move to intermediary square
                // move to square2

                square1 += "h";
                intermediarySquare = square1[0].ToString();
                intermediarySquare += square2[1].ToString() + "h";
            } 
            else
            {
                char row = square1[1];
                char col = square1[0];
                square1 = ((char)((int)col - 1)).ToString() + row.ToString() + "h";
                intermediarySquare = square1[0].ToString();
                intermediarySquare += square2[1].ToString() + "h";
            }


            Debug.Log(square1);
            Debug.Log(intermediarySquare);
            Debug.Log(square2);

            moveCoreXY(square1);
            moveCoreXY(intermediarySquare);
            moveCoreXY(square2);
            activateMagnet(false);

        }

        // returns true if square2 is to the right of square1, false if to the left
        private bool knightDirection(string square1, string square2)
        {
            if (square1[0] < square2[0])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void moveCoreXY(string square)
        {
            //sp.WriteLine(GRBLDict[square]);
        }

        public void activateMagnet(bool activated)
        {
            /*if(activated == true)
            {
                sp.WriteLine(GRBLDict["MAGON"]);
            }else
            {
                sp.WriteLine(GRBLDict["MAGOFF"]);
            }*/
        }

        /*
         
        performMove(square1, square2){
            move(square1)
            activateMagnet(true)
            move(square2)
            activateMagnet(false)
        }
          
        */

    }
    

}







































