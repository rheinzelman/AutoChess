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

        public Dictionary<string, string> GRBLDict = new Dictionary<string, string>();

        private void Start()
        {

            // GRBL COORDINATE DICTIONARY
            // h notates a half square position, used for knights
            // hh notates a half square position above the given square, used for castling

            // A COLS

            GRBLDict.Add("a1","X25Y265");
            GRBLDict.Add("a2", "X25Y227");
            GRBLDict.Add("a3", "X25Y192");
            GRBLDict.Add("a4", "X25Y152");
            GRBLDict.Add("a5", "X25Y115");
            GRBLDict.Add("a6", "X25Y80");
            GRBLDict.Add("a7", "X25Y47");
            GRBLDict.Add("a8", "X25Y10");

            GRBLDict.Add("a1h", "X44Y265");
            GRBLDict.Add("a2h", "X44Y227");
            GRBLDict.Add("a3h", "X44Y192");
            GRBLDict.Add("a4h", "X44Y152");
            GRBLDict.Add("a5h", "X44Y115");
            GRBLDict.Add("a6h", "X44Y80");
            GRBLDict.Add("a7h", "X44Y47");
            GRBLDict.Add("a8h", "X44Y10");
            
            // B COLS

            GRBLDict.Add("b1", "X60Y268");
            GRBLDict.Add("b2", "X62Y230");
            GRBLDict.Add("b3", "X62Y192");
            GRBLDict.Add("b4", "X62Y154");
            GRBLDict.Add("b5", "X62Y121");
            GRBLDict.Add("b6", "X62Y83");
            GRBLDict.Add("b7", "X62Y45");
            GRBLDict.Add("b8", "X62Y7");

            GRBLDict.Add("b1h", "X80Y268");
            GRBLDict.Add("b2h", "X80Y230");
            GRBLDict.Add("b3h", "X80Y192");
            GRBLDict.Add("b4h", "X80Y154");
            GRBLDict.Add("b5h", "X80Y121");
            GRBLDict.Add("b6h", "X80Y83");
            GRBLDict.Add("b7h", "X80Y45");
            GRBLDict.Add("b8h", "X80Y7");

            // C COLS

            GRBLDict.Add("c1", "X96Y262");
            GRBLDict.Add("c2", "X96Y224");
            GRBLDict.Add("c3", "X96Y190");
            GRBLDict.Add("c4", "X96Y154");
            GRBLDict.Add("c5", "X96Y117");
            GRBLDict.Add("c6", "X97Y81");
            GRBLDict.Add("c7", "X96Y43");
            GRBLDict.Add("c8", "X97Y7");

            GRBLDict.Add("c1h", "X116Y262");
            GRBLDict.Add("c2h", "X116Y224");
            GRBLDict.Add("c3h", "X116Y190");
            GRBLDict.Add("c4h", "X116Y154");
            GRBLDict.Add("c5h", "X116Y117");
            GRBLDict.Add("c6h", "X116Y81");
            GRBLDict.Add("c7h", "X116Y43");
            GRBLDict.Add("c8h", "X116Y7");

            // D COLS

            GRBLDict.Add("d1", "X131Y261");
            GRBLDict.Add("d2", "X131Y226");
            GRBLDict.Add("d3", "X131Y189");
            GRBLDict.Add("d4", "X133Y152");
            GRBLDict.Add("d5", "X133Y116");
            GRBLDict.Add("d6", "X132Y81");
            GRBLDict.Add("d7", "X133.5Y43");
            GRBLDict.Add("d8", "X133Y7");
           
            GRBLDict.Add("d1h", "X151Y261");
            GRBLDict.Add("d2h", "X151Y226");
            GRBLDict.Add("d3h", "X151Y189");
            GRBLDict.Add("d4h", "X151Y152");
            GRBLDict.Add("d5h", "X151Y116");
            GRBLDict.Add("d6h", "X151Y81");
            GRBLDict.Add("d7h", "X151Y43");
            GRBLDict.Add("d8h", "X151Y7");

            // E COLS

            GRBLDict.Add("e1", "X167Y261");
            GRBLDict.Add("e2", "X167Y226");
            GRBLDict.Add("e3", "X169Y190");
            GRBLDict.Add("e4", "X168Y154");
            GRBLDict.Add("e5", "X168Y117");
            GRBLDict.Add("e6", "X169Y80");
            GRBLDict.Add("e7", "X168Y45");
            GRBLDict.Add("e8", "X168Y8");

            GRBLDict.Add("e1h", "X188Y261");
            GRBLDict.Add("e2h", "X188Y226");
            GRBLDict.Add("e3h", "X188Y190");
            GRBLDict.Add("e4h", "X188Y154");
            GRBLDict.Add("e5h", "X188Y117");
            GRBLDict.Add("e6h", "X188Y80");
            GRBLDict.Add("e7h", "X188Y45");
            GRBLDict.Add("e8h", "X188Y8");

            // F COLS

            GRBLDict.Add("f1", "X204Y262");
            GRBLDict.Add("f2", "X204Y226");
            GRBLDict.Add("f3", "X204Y189");
            GRBLDict.Add("f4", "X204Y154");
            GRBLDict.Add("f5", "X204Y119");
            GRBLDict.Add("f6", "X204Y82");
            GRBLDict.Add("f7", "X205Y45");
            GRBLDict.Add("f8", "X205Y8");

            GRBLDict.Add("f1h", "X224Y262");
            GRBLDict.Add("f2h", "X224Y226");
            GRBLDict.Add("f3h", "X224Y189");
            GRBLDict.Add("f4h", "X224Y154");
            GRBLDict.Add("f5h", "X224Y119");
            GRBLDict.Add("f6h", "X224Y82");
            GRBLDict.Add("f7h", "X224Y45");
            GRBLDict.Add("f8h", "X224Y8");

            // G COLS

            GRBLDict.Add("g1", "X241Y262");
            GRBLDict.Add("g2", "X241Y224");
            GRBLDict.Add("g3", "X240Y188");
            GRBLDict.Add("g4", "X240Y151");
            GRBLDict.Add("g5", "X240Y114");
            GRBLDict.Add("g6", "X240Y78");
            GRBLDict.Add("g7", "X240Y43");
            GRBLDict.Add("g8", "X240Y6");

            GRBLDict.Add("g1h", "X261Y262");
            GRBLDict.Add("g2h", "X261Y224");
            GRBLDict.Add("g3h", "X261Y188");
            GRBLDict.Add("g4h", "X261Y151");
            GRBLDict.Add("g5h", "X261Y114");
            GRBLDict.Add("g6h", "X261Y78");
            GRBLDict.Add("g7h", "X261Y43");
            GRBLDict.Add("g8h", "X261Y6");

            GRBLDict.Add("h1", "X280Y262");
            GRBLDict.Add("h2", "X280Y227");
            GRBLDict.Add("h3", "X280Y192");
            GRBLDict.Add("h4", "X280Y155");
            GRBLDict.Add("h5", "X280Y118");
            GRBLDict.Add("h6", "X280Y83");
            GRBLDict.Add("h7", "X280Y46");
            GRBLDict.Add("h8", "X282Y9");

            GRBLDict.Add("MAGON", "M62P 0");
            GRBLDict.Add("MAGOFF", "M63P 0");

            // CASTLING HALF ROWS
            GRBLDict.Add("e1hh", "X167Y245");
            GRBLDict.Add("g1hh", "X241Y243");
            GRBLDict.Add("c1hh", "X96Y243");
            GRBLDict.Add("e8hh", "X168Y25");
            GRBLDict.Add("g8hh", "X240Y25");
            GRBLDict.Add("c8hh","");

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

        // if there is a piece on the capture square, return true
        public bool capturedPiece()
        {
            string board_state = ReadArray();

            if((int)board_state[64] - '0' == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int checkDifference(int[,] arr)
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
                return 1;
            }
            else if(sum == 1){
                return 2;
            }
            else
            {
                return 0;
            }

        }

        public int[,] getDifferenceArray(int[,] initial, int[,] final)
        {

            int[,] returnArray = new int[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //subtract initial and final board states
                    returnArray[i, j] = final[i, j] - initial[i, j];
                }
            }

            return returnArray;
            
        }

        public List<Vector2Int> getMoveFromDifferenceArray(int[,] differenceArray)
        {

            List<Vector2Int> move = new List<Vector2Int>();

            move.Add(new Vector2Int());
            move.Add(new Vector2Int());

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (differenceArray[i, j] == -1)
                    {
                        move[0] = new Vector2Int(i, j);
                    }
                    if (differenceArray[i, j] == 1)
                    {
                        move[1] = new Vector2Int(i, j);
                    }
                }
            }

            return move;

        }
            
        public static string ReadArray()
        {
            sp.Open();
            string Sensor = sp.ReadLine();
            sp.Close();

            return Sensor;
        }

        public void performStandardMove(string square1, string square2)
        {
            moveCoreXY(square1);
            activateMagnet(true);
            moveCoreXY(square2);
            activateMagnet(false);
        }

        public void performCastling(string square1, string square2)
        {

            string intermediarySquare;
            string rookSquare;
            string rookDestination;

            if (moveDirection(square1, square2))
            {

                rookSquare = ((char)((int)square1[0] + 3)).ToString() + square1[1];
                rookDestination = ((char)((int)square2[0] - 1)).ToString() + square2[1];
                
            }
            else
            {

                rookSquare = ((char)((int)square1[0] - 4)).ToString() + square1[1];
                rookDestination = ((char)((int)square2[0] + 1)).ToString() + square2[1];

            }

            // move the rook
            moveCoreXY(rookSquare);
            activateMagnet(true);
            moveCoreXY(rookDestination);
            activateMagnet(false);

            // move the king
            moveCoreXY(square1);
            activateMagnet(true);

            square1 += "hh";
            intermediarySquare = square2[0].ToString();
            intermediarySquare += square1[1].ToString() + "hh";

            moveCoreXY(square1);
            moveCoreXY(intermediarySquare);
            moveCoreXY(square2);
            activateMagnet(false);

        }

        public void performLongCastling()
        {

        }

        public void performKnightMove(string square1, string square2)
        {

            string intermediarySquare;

            if(moveDirection(square1, square2) == true)
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

        // returns true if the move moves the piece to the right
        private bool moveDirection(string square1, string square2)
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
            sp.Open();
            sp.WriteLine(GRBLDict[square]);
            sp.Close();
        }

        public void activateMagnet(bool activated)
        {
            if(activated == true)
            {
                sp.Open();
                sp.WriteLine(GRBLDict["MAGON"]);
                sp.Close();
            }
            else
            {
                sp.Open();
                sp.WriteLine(GRBLDict["MAGOFF"]);
                sp.Close();
            }
        }

        public void homeCoreXY()
        {
            sp.Open();
            sp.WriteLine("$H");
            sp.Close();
        }

    }
    

}







































