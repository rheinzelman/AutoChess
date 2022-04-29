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

            { 0, 8, 16, 24, 25, 17, 9, 1 },
            { 2, 10, 18, 26, 27, 19, 11 ,3 },
            { 4, 12, 20, 28, 29, 21, 13, 5 },
            { 6, 14, 22, 30, 31, 23, 15, 7 },
            { 38, 46, 54, 62, 63, 55, 47, 39 },
            { 36, 44, 52, 60, 61, 53, 45, 37 },
            { 34, 42, 50, 58, 59, 51, 43, 35 },
            { 32, 40, 48, 56, 57, 49, 41, 33 }
        };

        // physical board parameters
        int overShootAmount = 5;

        public Dictionary<string, string> GRBLDict = new Dictionary<string, string>();

        int takenCount = 0;

        string[] captureSquares =
        {
                "cap1",
                "cap2",
                "cap3",
                "cap4",
                "cap5",
                "cap6",
                "cap7",
                "cap8"
            };

        private void Start()
        {

            // GRBL COORDINATE DICTIONARY
            // h notates a half square position, used for knights
            // hh notates a half square position above the given square, used for castling

            // V COLS

            GRBLDict.Add("z1v", "X0Y241.5");
            GRBLDict.Add("a1v", "X29Y241.5");
            GRBLDict.Add("b1v", "X65Y241.5");
            GRBLDict.Add("c1v", "X101Y241.5");
            GRBLDict.Add("d1v", "X136.5Y241.5");
            GRBLDict.Add("e1v", "X173Y241.5");
            GRBLDict.Add("f1v", "X208.5Y241.5");
            GRBLDict.Add("g1v", "X244.5Y241.5");
            GRBLDict.Add("h1v", "X281.5Y241.5");

            GRBLDict.Add("z2v", "X0Y204");
            GRBLDict.Add("a2v", "X29Y204");
            GRBLDict.Add("b2v", "X65Y204");
            GRBLDict.Add("c2v", "X101Y204");
            GRBLDict.Add("d2v", "X138Y204");
            GRBLDict.Add("e2v", "X172.5Y204");
            GRBLDict.Add("f2v", "208.5XY204");
            GRBLDict.Add("g2v", "245.5XY204");
            GRBLDict.Add("h2v", "281.5XY204");

            GRBLDict.Add("z3v", "X0Y167.5");
            GRBLDict.Add("a3v", "X29Y167.5");
            GRBLDict.Add("b3v", "X65Y167.5");
            GRBLDict.Add("c3v", "X101Y167.5");
            GRBLDict.Add("d3v", "X136.5Y167.5");
            GRBLDict.Add("e3v", "X173Y167.5");
            GRBLDict.Add("f3v", "X208.5Y167.5");
            GRBLDict.Add("g3v", "X245.5Y167.5");
            GRBLDict.Add("h3v", "X281.5Y167.5");

            GRBLDict.Add("z4v", "X0Y131.5");
            GRBLDict.Add("a4v", "X29Y131.5");
            GRBLDict.Add("b4v", "X65Y131.5");
            GRBLDict.Add("c4v", "X101Y131.5");
            GRBLDict.Add("d4v", "X136.5Y131.5");
            GRBLDict.Add("e4v", "X173Y131.5");
            GRBLDict.Add("f4v", "X208.5Y131.5");
            GRBLDict.Add("g4v", "X245.5Y131.5");
            GRBLDict.Add("h4v", "X281.5Y131.5");

            GRBLDict.Add("z5v", "X0Y95.5");
            GRBLDict.Add("a5v", "X29Y95.5");
            GRBLDict.Add("b5v", "X65Y95.5");
            GRBLDict.Add("c5v", "X101Y95.5");
            GRBLDict.Add("d5v", "X136.5Y95.5");
            GRBLDict.Add("e5v", "X173Y95.5");
            GRBLDict.Add("f5v", "X208.5Y95.5");
            GRBLDict.Add("g5v", "X245.5Y95.5");
            GRBLDict.Add("h5v", "X281.5Y95.5");

            GRBLDict.Add("z6v", "X0Y59.5");
            GRBLDict.Add("a6v", "X29Y59.5");
            GRBLDict.Add("b6v", "X65Y59.5");
            GRBLDict.Add("c6v", "X101Y59.5");
            GRBLDict.Add("d6v", "X136.5Y59.5");
            GRBLDict.Add("e6v", "X173Y59.5");
            GRBLDict.Add("f6v", "X208.5Y59.5");
            GRBLDict.Add("g6v", "X245.5Y59.5");
            GRBLDict.Add("h6v", "X281.5Y59.5");

            GRBLDict.Add("z7v", "X0Y23.5");
            GRBLDict.Add("a7v", "X29Y23.5");
            GRBLDict.Add("b7v", "X65Y23.5");
            GRBLDict.Add("c7v", "X101Y23.5");
            GRBLDict.Add("d7v", "X136.5Y23.5");
            GRBLDict.Add("e7v", "X173Y23.5");
            GRBLDict.Add("f7v", "X208.5Y23.5");
            GRBLDict.Add("g7v", "X245.5Y23.5");
            GRBLDict.Add("h7v", "X281.5Y23.5");


           

            // A COLS

            GRBLDict.Add("a1", "X29Y260");
            GRBLDict.Add("a2", "X29Y224");
            GRBLDict.Add("a3", "X29Y185");
            GRBLDict.Add("a4", "X29Y150");
            GRBLDict.Add("a5", "X29Y113");
            GRBLDict.Add("a6", "X29Y75");
            GRBLDict.Add("a7", "X28Y42");
            GRBLDict.Add("a8", "X29Y5");


            GRBLDict.Add("a1h", "X46.5Y260");
            GRBLDict.Add("a2h", "X46.5Y224");
            GRBLDict.Add("a3h", "X46.5Y185");
            GRBLDict.Add("a4h", "X46.5Y150");
            GRBLDict.Add("a5h", "X46.5Y113");
            GRBLDict.Add("a6h", "X46.5Y75");
            GRBLDict.Add("a7h", "X46.5Y42");
            GRBLDict.Add("a8h", "X46.5Y5");

            // B COLS

            GRBLDict.Add("b1", "X65Y260");
            GRBLDict.Add("b2", "X65Y224");
            GRBLDict.Add("b3", "X65Y187");
            GRBLDict.Add("b4", "X65Y150");
            GRBLDict.Add("b5", "X65Y113");
            GRBLDict.Add("b6", "X65Y77");
            GRBLDict.Add("b7", "X65Y40");
            GRBLDict.Add("b8", "X65Y5");

            GRBLDict.Add("b1h", "X82.75Y260");
            GRBLDict.Add("b2h", "X82.75Y224");
            GRBLDict.Add("b3h", "X82.75Y187");
            GRBLDict.Add("b4h", "X82.75Y150");
            GRBLDict.Add("b5h", "X82.75Y113");
            GRBLDict.Add("b6h", "X82.75Y77");
            GRBLDict.Add("b7h", "X82.75Y40");
            GRBLDict.Add("b8h", "X82.75Y5");

            // C COLS

            GRBLDict.Add("c1", "X101Y259");
            GRBLDict.Add("c2", "X101Y222");
            GRBLDict.Add("c3", "X101Y185");
            GRBLDict.Add("c4", "X102Y148");
            GRBLDict.Add("c5", "X101Y111");
            GRBLDict.Add("c6", "X101.5Y75.5");
            GRBLDict.Add("c7", "X100.5Y39.5");
            GRBLDict.Add("c8", "X101Y3.5");

            GRBLDict.Add("c1h", "X118.5Y259");
            GRBLDict.Add("c2h", "X118.5Y222");
            GRBLDict.Add("c3h", "X118.5Y185");
            GRBLDict.Add("c4h", "X118.5Y148");
            GRBLDict.Add("c5h", "X118.5Y111");
            GRBLDict.Add("c6h", "X118.5Y75.5");
            GRBLDict.Add("c7h", "X118.5Y39.5");
            GRBLDict.Add("c8h", "X118.5Y3.5");

            // D COLS

            GRBLDict.Add("d1", "X136.5Y258.5");
            GRBLDict.Add("d2", "X138Y222.5");
            GRBLDict.Add("d3", "X137Y185.5");
            GRBLDict.Add("d4", "X137Y149.5");
            GRBLDict.Add("d5", "X138Y113.5");
            GRBLDict.Add("d6", "X136.5Y76.5");
            GRBLDict.Add("d7", "X136.5Y40.5");
            GRBLDict.Add("d8", "X136.5Y4.5");
           
            GRBLDict.Add("d1h", "X154Y258.5");
            GRBLDict.Add("d2h", "X154Y222.5");
            GRBLDict.Add("d3h", "X154Y185.5");
            GRBLDict.Add("d4h", "X154Y149.5");
            GRBLDict.Add("d5h", "X154Y113.5");
            GRBLDict.Add("d6h", "X154Y76.5");
            GRBLDict.Add("d7h", "X154Y39.5");
            GRBLDict.Add("d8h", "X154Y3.5");

            // E COLS

            GRBLDict.Add("e1", "X173Y258.5");
            GRBLDict.Add("e2", "X172.5Y221");
            GRBLDict.Add("e3", "X173.75Y186");
            GRBLDict.Add("e4", "X173Y149");
            GRBLDict.Add("e5", "X173Y113");
            GRBLDict.Add("e6", "X174Y77");
            GRBLDict.Add("e7", "X173Y41");
            GRBLDict.Add("e8", "X172.5Y5");

            GRBLDict.Add("e1h", "X190.75Y258.5");
            GRBLDict.Add("e2h", "X190.75Y221");
            GRBLDict.Add("e3h", "X190.75Y186");
            GRBLDict.Add("e4h", "X190.75Y149");
            GRBLDict.Add("e5h", "X190.75Y113");
            GRBLDict.Add("e6h", "X190.75Y77");
            GRBLDict.Add("e7h", "X190.75Y41");
            GRBLDict.Add("e8h", "X190.75Y5");

            // F COLS

            GRBLDict.Add("f1", "X208.5Y258.5");
            GRBLDict.Add("f2", "X208.5Y221");
            GRBLDict.Add("f3", "X208.5Y184.5");
            GRBLDict.Add("f4", "X208.5Y149");
            GRBLDict.Add("f5", "X209Y113");
            GRBLDict.Add("f6", "X209.5Y78.5");
            GRBLDict.Add("f7", "X209.5Y41");
            GRBLDict.Add("f8", "X208.5Y5");

            GRBLDict.Add("f1h", "X227Y258.5");
            GRBLDict.Add("f2h", "X227Y221");
            GRBLDict.Add("f3h", "X227Y184.5");
            GRBLDict.Add("f4h", "X227Y149");
            GRBLDict.Add("f5h", "X227Y113");
            GRBLDict.Add("f6h", "X227Y78.5");
            GRBLDict.Add("f7h", "X227Y41");
            GRBLDict.Add("f8h", "X227Y5");

            // G COLS

            GRBLDict.Add("g1", "X244.5Y258.5");
            GRBLDict.Add("g2", "X245.5Y222");
            GRBLDict.Add("g3", "X245.5Y185.5");
            GRBLDict.Add("g4", "X245Y148");
            GRBLDict.Add("g5", "X245Y112.5");
            GRBLDict.Add("g6", "X245.5Y78.5");
            GRBLDict.Add("g7", "X245.5Y42.25");
            GRBLDict.Add("g8", "X246Y5");

            GRBLDict.Add("g1h", "X262.5Y258.5");
            GRBLDict.Add("g2h", "X262.5Y222");
            GRBLDict.Add("g3h", "X262.5Y185.5");
            GRBLDict.Add("g4h", "X262.5Y148");
            GRBLDict.Add("g5h", "X262.5Y112.5");
            GRBLDict.Add("g6h", "X262.5Y78.5");
            GRBLDict.Add("g7h", "X262.5Y42.25");
            GRBLDict.Add("g8h", "X262.5Y5");

            // H COLS

            GRBLDict.Add("h1", "X281.5Y258.5");
            GRBLDict.Add("h2", "X281.5Y222");
            GRBLDict.Add("h3", "X281Y185");
            GRBLDict.Add("h4", "X280.5Y148.5");
            GRBLDict.Add("h5", "X280.5Y112.5");
            GRBLDict.Add("h6", "X281Y75.5");
            GRBLDict.Add("h7", "X281.25Y41");
            GRBLDict.Add("h8", "X282Y4.5");

            GRBLDict.Add("MAGON", "M62P 0");
            GRBLDict.Add("MAGOFF", "M63P 0");

            // CASTLING HALF ROWS

            GRBLDict.Add("e1hh", "X172Y240");
            GRBLDict.Add("g1hh", "X244Y239.25");
            GRBLDict.Add("c1hh", "X100.5Y240");
            GRBLDict.Add("e8hh", "X172Y22.5");
            GRBLDict.Add("g8hh", "X245.5Y22.5");
            GRBLDict.Add("c8hh", "X100Y22.5");

            // TAKEN PIECES

            GRBLDict.Add("cap1", "X0Y74");
            GRBLDict.Add("cap2", "X0Y110");
            GRBLDict.Add("cap3", "X0Y146");
            GRBLDict.Add("cap4", "X0Y182");

        }

        public int[,] boardToArray()
        {

            string board_state = ReadArray();

            int[,] returnArray = new int[8, 8];

            int arrayIndex = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
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
            moveCoreXYCoords(overShoot(square2, moveCardinalDirection(square1, square2)));
            activateMagnet(false);
        }

        public void performCastling(string square1, string square2)
        {

            string intermediarySquare;
            string rookSquare;
            string rookDestination;
            string kingDestination;

            if (moveDirection(square1, square2))
            {

                rookSquare = ((char)((int)square1[0] + 3)).ToString() + square1[1];
                rookDestination = ((char)((int)square1[0] + 1)).ToString() + square2[1];

                intermediarySquare = ((char)((int)square2[0] + 1)).ToString();
                intermediarySquare += square1[1].ToString() + "hh";

                kingDestination = ((char)((int)square2[0] + 1)).ToString() + square2[1].ToString();

            }
            else
            {

                rookSquare = ((char)((int)square1[0] - 4)).ToString() + square1[1];
                rookDestination = ((char)((int)square1[0] - 1)).ToString() + square2[1];

                kingDestination = ((char)((int)square2[0])).ToString() + square2[1].ToString();

                intermediarySquare = ((char)((int)kingDestination[0])).ToString() + square1[1].ToString() + "hh";


            }

            // move the rook
            moveCoreXY(rookSquare);
            activateMagnet(true);
            moveCoreXY(rookDestination);
            moveCoreXYCoords(overShoot(rookDestination, moveCardinalDirection(rookSquare, rookDestination)));
            activateMagnet(false);

            // move the king
            moveCoreXY(square1);
            activateMagnet(true);

            square1 += "hh";

            moveCoreXY(square1);
            moveCoreXY(intermediarySquare);
            moveCoreXY(kingDestination);
            moveCoreXYCoords(overShoot(kingDestination, moveCardinalDirection(intermediarySquare, kingDestination)));
            activateMagnet(false);

        }

        public void performKnightMove(string square1, string square2)
        {

            string intermediarySquare;

            int overShootDirection;

            moveCoreXY(square1);
            activateMagnet(true);

            if (moveDirection(square1, square2) == true)
            {

                // if the knight is moving in the right direction
                // redefine square1 to be a half square to the right
                // move to intermediary square
                // move to square2

                if(knightTallMove(square1, square2) == true)
                {
                    square1 += "h";
                    intermediarySquare = square1[0].ToString();
                    intermediarySquare += square2[1].ToString() + "h";
                    overShootDirection = 2;
                }
                else
                {
                    if(knightUpwardsMove(square1, square2) == true)
                    {   
                        square1 += "v";
                        intermediarySquare = square2[0].ToString();
                        intermediarySquare += square1[1].ToString() + "v";
                        overShootDirection = 1;
                    }
                    else
                    {

                        char row = square2[1];
                        char col = square1[0];
                        square1 = col.ToString() + row.ToString() + "v";
                        intermediarySquare = square2 + "v";
                        overShootDirection = 3;

                    }


                }

            } 
            else
            {
            

                if(knightTallMove(square1, square2) == true)
                {
                    char row = square1[1];
                    char col = square1[0];
                    square1 = ((char)((int)col - 1)).ToString() + row.ToString() + "h";
                    intermediarySquare = square1[0].ToString();
                    intermediarySquare += square2[1].ToString() + "h";
                    overShootDirection = 4;
                }
                else
                {

                    if(knightUpwardsMove(square1, square2) == true)
                    {


                        square1 += "v";
                        intermediarySquare = square2[0].ToString();
                        intermediarySquare += square1[1].ToString() + "v";
                        overShootDirection = 1;

                    }
                    else
                    {

                        char row = square2[1];
                        char col = square1[0];
                        square1 = col.ToString() + row.ToString() + "v";
                        intermediarySquare = square2 + "v";
                        overShootDirection = 3;
                    }
                    
                }

            }

            moveCoreXY(square1);
            moveCoreXY(intermediarySquare);
            moveCoreXY(square2);
            moveCoreXYCoords(overShoot(square2, overShootDirection));
            activateMagnet(false);

        }

        public void performCapture(string square1)
        {

            string intermediarySquare1;
            string intermediarySquare2;

            moveCoreXY(square1);
            activateMagnet(true);

            if(square1[1] <= '7')
            {
                intermediarySquare1 = square1.ToString() + "v";
            }
            else
            {
                intermediarySquare1 = (square1[0] - 1).ToString() + "v";
            }

            intermediarySquare2 = "z" + square1[1].ToString() + "v";

            print("intermediary1: " + intermediarySquare1 + ", intermediary2: " + intermediarySquare2);

            moveCoreXY(intermediarySquare1);
            moveCoreXY(intermediarySquare2);

            moveCoreXY(captureSquares[takenCount]);
            activateMagnet(false);
            takenCount++;
            if(takenCount == 4)
            {
                print("remove captured pieces");
                takenCount = 0;
            }
        }

        // move the piece a little extra in a given direction to account for board friction
        public string overShoot(string square, int direction)
        {
            string coords = GRBLDict[square];

            string[] newCoords = new string[2];

            int x = 0;

            foreach (char c in coords)
            {
                if (Char.IsNumber(c) || c == '.')
                {
                    newCoords[x] += c;
                } else if(c == 'Y')
                {
                    x++;
                }
            }

            float xInt = float.Parse(newCoords[0]);
            float yInt = float.Parse(newCoords[1]);

            if(direction == 1)
            {
                yInt -= overShootAmount;
            } else if(direction == 2)
            {
                xInt += overShootAmount;
            } else if(direction == 3)
            {
                yInt += overShootAmount;
            } else if (direction == 4)
            {
                xInt -= overShootAmount;
            } else if (direction == -1)
            {
                print("overShoot error");
            }   

            string returnCoords = "X" + xInt.ToString() + "Y" + yInt.ToString();

            return returnCoords;

        }
        
        private bool knightUpwardsMove(string square1, string square2)
        {
            if(square1[1] - square2[1] < 0)
            {
                return true;
            } else
            {
                return false;
            }
        }

        // returns true if the move moves the night longer vertically
        private bool knightTallMove(string square1, string square2)
        {

            if(Math.Abs(square1[1] - square2[1]) == 2)
            {
                return true;
            }
            else
            {
                return false;
            }

            
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

        // f6h f6
        // horsey bad
        private int moveCardinalDirection(string square1, string square2)
        {
            char col1 = square1[0];
            char col2 = square2[0];
            char row1 = square1[1];
            char row2 = square2[1];

            // right
            if ((int)col1 - (int)col2 < 0)
            {
                return 2;
            }
            // left
            else if ((int)col1 - (int)col2 > 0)
            {
                return 4;
            }
            // up
            else if (row1 - row2 < 0)
            {
                return 1;
            }
            // down
            else if (row1 - row2 > 0)
            {
                return 3;
            }
            else
            {
                return -1;
            }



        }

        public void moveCoreXY(string square)
        {
            sp.Open();
            sp.WriteLine(GRBLDict[square]);
            sp.Close();
        }

        public void moveCoreXYCoords(string coords)
        {
            sp.Open();
            sp.WriteLine(coords);
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
