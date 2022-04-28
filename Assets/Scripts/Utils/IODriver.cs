using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using ChessGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
    public class IODriver : MonoBehaviour
    {
        public static IODriver Instance;
        
        public static SerialPort SerialPort = new SerialPort("COM3", 115200);

        private string _initialState;
        private string _currentState;

        // Mapping that provides translation from physical board readout to 
        private readonly int[,] _keyArray =
        {
            {0, 8, 16, 24, 25, 17, 9, 1},
            {2, 10, 18, 26, 27, 19, 11, 3},
            {4, 12, 20, 28, 29, 21, 13, 5},
            {6, 14, 22, 30, 31, 23, 15, 7},
            {38, 46, 54, 62, 63, 55, 47, 39},
            {36, 44, 52, 60, 61, 53, 45, 37},
            {34, 42, 50, 58, 59, 51, 43, 35},
            {32, 40, 48, 56, 57, 49, 41, 33}
        };

        // Physical board parameters
        private const int OverShootAmount = 5;

        // GRBL COORDINATE DICTIONARY
        // "h" notates a half square position, used for knights
        // "hh" notates a half square position above the given square, used for castling
        private readonly Dictionary<string, string> _grblDict = new Dictionary<string, string>
        {
            // V COLS

            {"z1v", "X0Y241.5"},
            {"a1v", "X29Y241.5"},
            {"b1v", "X65Y241.5"},
            {"c1v", "X101Y241.5"},
            {"d1v", "X136.5Y241.5"},
            {"e1v", "X173Y241.5"},
            {"f1v", "X208.5Y241.5"},
            {"g1v", "X244.5Y241.5"},
            {"h1v", "X281.5Y241.5"},

            {"z2v", "X0Y204"},
            {"a2v", "X29Y204"},
            {"b2v", "X65Y204"},
            {"c2v", "X101Y204"},
            {"d2v", "X138Y204"},
            {"e2v", "X172.5Y204"},
            {"f2v", "208.5XY204"},
            {"g2v", "245.5XY204"},
            {"h2v", "281.5XY204"},

            {"z3v", "X0Y167.5"},
            {"a3v", "X29Y167.5"},
            {"b3v", "X65Y167.5"},
            {"c3v", "X101Y167.5"},
            {"d3v", "X136.5Y167.5"},
            {"e3v", "X173Y167.5"},
            {"f3v", "X208.5Y167.5"},
            {"g3v", "X245.5Y167.5"},
            {"h3v", "X281.5Y167.5"},

            {"z4v", "X0Y131.5"},
            {"a4v", "X29Y131.5"},
            {"b4v", "X65Y131.5"},
            {"c4v", "X101Y131.5"},
            {"d4v", "X136.5Y131.5"},
            {"e4v", "X173Y131.5"},
            {"f4v", "X208.5Y131.5"},
            {"g4v", "X245.5Y131.5"},
            {"h4v", "X281.5Y131.5"},

            {"z5v", "X0Y95.5"},
            {"a5v", "X29Y95.5"},
            {"b5v", "X65Y95.5"},
            {"c5v", "X101Y95.5"},
            {"d5v", "X136.5Y95.5"},
            {"e5v", "X173Y95.5"},
            {"f5v", "X208.5Y95.5"},
            {"g5v", "X245.5Y95.5"},
            {"h5v", "X281.5Y95.5"},

            {"z6v", "X0Y59.5"},
            {"a6v", "X29Y59.5"},
            {"b6v", "X65Y59.5"},
            {"c6v", "X101Y59.5"},
            {"d6v", "X136.5Y59.5"},
            {"e6v", "X173Y59.5"},
            {"f6v", "X208.5Y59.5"},
            {"g6v", "X245.5Y59.5"},
            {"h6v", "X281.5Y59.5"},

            {"z7v", "X0Y23.5"},
            {"a7v", "X29Y23.5"},
            {"b7v", "X65Y23.5"},
            {"c7v", "X101Y23.5"},
            {"d7v", "X136.5Y23.5"},
            {"e7v", "X173Y23.5"},
            {"f7v", "X208.5Y23.5"},
            {"g7v", "X245.5Y23.5"},
            {"h7v", "X281.5Y23.5"},


            // A COLS

            {"a1", "X29Y260"},
            {"a2", "X29Y224"},
            {"a3", "X29Y185"},
            {"a4", "X29Y150"},
            {"a5", "X29Y113"},
            {"a6", "X29Y75"},
            {"a7", "X28Y42"},
            {"a8", "X29Y5"},


            {"a1h", "X46.5Y260"},
            {"a2h", "X46.5Y224"},
            {"a3h", "X46.5Y185"},
            {"a4h", "X46.5Y150"},
            {"a5h", "X46.5Y113"},
            {"a6h", "X46.5Y75"},
            {"a7h", "X46.5Y42"},
            {"a8h", "X46.5Y5"},

            // B COLS

            {"b1", "X65Y260"},
            {"b2", "X65Y224"},
            {"b3", "X65Y187"},
            {"b4", "X65Y150"},
            {"b5", "X65Y113"},
            {"b6", "X65Y77"},
            {"b7", "X65Y40"},
            {"b8", "X65Y5"},

            {"b1h", "X82.75Y260"},
            {"b2h", "X82.75Y224"},
            {"b3h", "X82.75Y187"},
            {"b4h", "X82.75Y150"},
            {"b5h", "X82.75Y113"},
            {"b6h", "X82.75Y77"},
            {"b7h", "X82.75Y40"},
            {"b8h", "X82.75Y5"},

            // C COLS

            {"c1", "X101Y259"},
            {"c2", "X101Y222"},
            {"c3", "X101Y185"},
            {"c4", "X102Y148"},
            {"c5", "X101Y111"},
            {"c6", "X101.5Y75.5"},
            {"c7", "X100.5Y39.5"},
            {"c8", "X101Y3.5"},

            {"c1h", "X118.5Y259"},
            {"c2h", "X118.5Y222"},
            {"c3h", "X118.5Y185"},
            {"c4h", "X118.5Y148"},
            {"c5h", "X118.5Y111"},
            {"c6h", "X118.5Y75.5"},
            {"c7h", "X118.5Y39.5"},
            {"c8h", "X118.5Y3.5"},

            // D COLS

            {"d1", "X136.5Y258.5"},
            {"d2", "X138Y222.5"},
            {"d3", "X137Y185.5"},
            {"d4", "X137Y149.5"},
            {"d5", "X138Y113.5"},
            {"d6", "X136.5Y76.5"},
            {"d7", "X136.5Y40.5"},
            {"d8", "X136.5Y4.5"},

            {"d1h", "X154Y258.5"},
            {"d2h", "X154Y222.5"},
            {"d3h", "X154Y185.5"},
            {"d4h", "X154Y149.5"},
            {"d5h", "X154Y113.5"},
            {"d6h", "X154Y76.5"},
            {"d7h", "X154Y39.5"},
            {"d8h", "X154Y3.5"},

            // E COLS

            {"e1", "X173Y258.5"},
            {"e2", "X172.5Y221"},
            {"e3", "X173.75Y186"},
            {"e4", "X173Y149"},
            {"e5", "X173Y113"},
            {"e6", "X174Y77"},
            {"e7", "X173Y41"},
            {"e8", "X172.5Y5"},

            {"e1h", "X190.75Y258.5"},
            {"e2h", "X190.75Y221"},
            {"e3h", "X190.75Y186"},
            {"e4h", "X190.75Y149"},
            {"e5h", "X190.75Y113"},
            {"e6h", "X190.75Y77"},
            {"e7h", "X190.75Y41"},
            {"e8h", "X190.75Y5"},

            // F COLS

            {"f1", "X208.5Y258.5"},
            {"f2", "X208.5Y221"},
            {"f3", "X208.5Y184.5"},
            {"f4", "X208.5Y149"},
            {"f5", "X209Y113"},
            {"f6", "X209.5Y78.5"},
            {"f7", "X209.5Y41"},
            {"f8", "X208.5Y5"},

            {"f1h", "X227Y258.5"},
            {"f2h", "X227Y221"},
            {"f3h", "X227Y184.5"},
            {"f4h", "X227Y149"},
            {"f5h", "X227Y113"},
            {"f6h", "X227Y78.5"},
            {"f7h", "X227Y41"},
            {"f8h", "X227Y5"},

            // G COLS

            {"g1", "X244.5Y258.5"},
            {"g2", "X245.5Y222"},
            {"g3", "X245.5Y185.5"},
            {"g4", "X245Y148"},
            {"g5", "X245Y112.5"},
            {"g6", "X245.5Y78.5"},
            {"g7", "X245.5Y42.25"},
            {"g8", "X246Y5"},

            {"g1h", "X262.5Y258.5"},
            {"g2h", "X262.5Y222"},
            {"g3h", "X262.5Y185.5"},
            {"g4h", "X262.5Y148"},
            {"g5h", "X262.5Y112.5"},
            {"g6h", "X262.5Y78.5"},
            {"g7h", "X262.5Y42.25"},
            {"g8h", "X262.5Y5"},

            // H COLS

            {"h1", "X281.5Y258.5"},
            {"h2", "X281.5Y222"},
            {"h3", "X281Y185"},
            {"h4", "X280.5Y148.5"},
            {"h5", "X280.5Y112.5"},
            {"h6", "X281Y75.5"},
            {"h7", "X281.25Y41"},
            {"h8", "X282Y4.5"},

            // CASTLING HALF ROWS

            {"e1hh", "X172Y240"},
            {"g1hh", "X244Y239.25"},
            {"c1hh", "X100.5Y240"},
            {"e8hh", "X172Y22.5"},
            {"g8hh", "X245.5Y22.5"},
            {"c8hh", "X100Y22.5"},

            // TAKEN PIECES

            {"cap1", "X0Y74"},
            {"cap2", "X0Y110"},
            {"cap3", "X0Y146"},
            {"cap4", "X0Y182"},

            // MAGNET

            {"MAGON", "M62P 0"},
            {"MAGOFF", "M63P 0"}
        };

        private int _takenCount;

        private readonly string[] _captureSquares =
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

        private bool _isConnected;

        private void Start()
        {
            Instance = this;
            
            //HomeCoreXY();
        }

        private void Update()
        {
            // if (_isConnected) return;
            //
            // try
            // {
            //     SerialPort = new SerialPort("COM3", 115200);
            //     _isConnected = true;
            // }
            // catch (IOException e)
            // {
            //     Debug.LogError(e);
            // }
        }

        // [Button]
        // private void SendStandardMove(string from, string to)
        // {
        //     PerformStandardMove(from, to);
        // }

        public int[,] BoardToArray()
        {
            var boardState = ReadArray();

            var returnArray = new int[8, 8];

            var arrayIndex = 0;

            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                var arrayRow = _keyArray[i, j] % 8;
                //var arrayCol = (int) Math.Floor((double) (_keyArray[i, j] / 8));
                var arrayCol = _keyArray[i, j] / 8;

                returnArray[arrayRow, arrayCol] = boardState[arrayIndex] - '0';
                arrayIndex++;
            }

            return returnArray;
        }

        // if there is a piece on the capture square, return true
        public bool CapturedPiece()
        {
            var boardState = ReadArray();

            return boardState[64] - '0' == 1;
        }

        public int CheckDifference(int[,] arr)
        {
            var sum = 0;

            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
                if (arr[i, j] != 0)
                    sum += 1;

            return sum switch
            {
                2 => 1,
                1 => 2,
                _ => 0
            };
        }

        public int[,] GetDifferenceArray(int[,] initial, int[,] final)
        {
            var returnArray = new int[8, 8];

            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
                //subtract initial and final board states
                returnArray[i, j] = final[i, j] - initial[i, j];

            return returnArray;
        }

        public (Vector2Int, Vector2Int) GetMoveFromDifferenceArray(int[,] initial, int[,] final)
        {
            return GetMoveFromDifferenceArray(GetDifferenceArray(initial, final));
        }

        public (Vector2Int, Vector2Int) GetMoveFromDifferenceArray(int[,] differenceArray) //List<Vector2Int>
        {
            // var move = new List<Vector2Int>
            // {
            //     new Vector2Int(),
            //     new Vector2Int()
            // };

            var initial = Constants.ErrorValue;
            var final = Constants.ErrorValue;

            for (var i = 0; i < 8; i++)
            for (var j = 0; j < 8; j++)
            {
                if (differenceArray[i, j] == -1) initial = new Vector2Int(i, j);
                if (differenceArray[i, j] == 1) final = new Vector2Int(i, j);
            }

            return (initial, final);
        }

        public static string ReadArray()
        {
            SerialPort.Open();
            var sensor = SerialPort.ReadLine();
            SerialPort.Close();

            return sensor;
        }

        [Button]
        public void PerformStandardMove(string square1, string square2)
        {
            MoveCoreXY(square1);
            ActivateMagnet(true);
            MoveCoreXY(square2);
            MoveCoreXYCoords(OverShoot(square2, MoveCardinalDirection(square1, square2)));
            ActivateMagnet(false);
        }

        public void PerformCastling(string square1, string square2)
        {
            string intermediarySquare;
            string rookSquare;
            string rookDestination;
            string kingDestination;

            if (MoveDirection(square1, square2))
            {
                rookSquare = ((char) (square1[0] + 3)).ToString() + square1[1];
                rookDestination = ((char) (square1[0] + 1)).ToString() + square2[1];

                intermediarySquare = ((char) (square2[0])).ToString();
                intermediarySquare += square1[1] + "hh";

                kingDestination = (char) (square2[0]) + square2[1].ToString();
            }
            else
            {
                rookSquare = ((char) (square1[0] - 4)).ToString() + square1[1];
                rookDestination = ((char) (square1[0] - 1)).ToString() + square2[1];

                kingDestination = square2[0] + square2[1].ToString();

                intermediarySquare = kingDestination[0] + square1[1].ToString() + "hh";
            }

            // move the rook
            MoveCoreXY(rookSquare);
            ActivateMagnet(true);
            MoveCoreXY(rookDestination);
            MoveCoreXYCoords(OverShoot(rookDestination, MoveCardinalDirection(rookSquare, rookDestination)));
            ActivateMagnet(false);

            // move the king
            MoveCoreXY(square1);
            ActivateMagnet(true);

            square1 += "hh";

            MoveCoreXY(square1);
            MoveCoreXY(intermediarySquare);
            MoveCoreXY(kingDestination);
            MoveCoreXYCoords(OverShoot(kingDestination, MoveCardinalDirection(intermediarySquare, kingDestination)));
            ActivateMagnet(false);
        }

        [Button]
        public void PerformKnightMove(string square1, string square2)
        {
            string intermediarySquare;

            int overShootDirection;

            MoveCoreXY(square1);
            ActivateMagnet(true);

            if (MoveDirection(square1, square2))
            {
                // if the knight is moving in the right direction
                // redefine square1 to be a half square to the right
                // move to intermediary square
                // move to square2

                if (KnightTallMove(square1, square2))
                {
                    square1 += "h";
                    intermediarySquare = square1[0].ToString();
                    intermediarySquare += square2[1] + "h";
                    overShootDirection = 2;
                }
                else
                {
                    if (KnightUpwardsMove(square1, square2))
                    {
                        square1 += "v";
                        intermediarySquare = square2[0].ToString();
                        intermediarySquare += square1[1] + "v";
                        overShootDirection = 1;
                    }
                    else
                    {
                        var row = square2[1];
                        var col = square1[0];
                        square1 = col + row.ToString() + "v";
                        intermediarySquare = square2 + "v";
                        overShootDirection = 3;
                    }
                }
            }
            else
            {
                if (KnightTallMove(square1, square2))
                {
                    var row = square1[1];
                    var col = square1[0];
                    square1 = (char) (col - 1) + row.ToString() + "h";
                    intermediarySquare = square1[0].ToString();
                    intermediarySquare += square2[1] + "h";
                    overShootDirection = 4;
                }
                else
                {
                    if (KnightUpwardsMove(square1, square2))
                    {
                        square1 += "v";
                        intermediarySquare = square2[0].ToString();
                        intermediarySquare += square1[1] + "v";
                        overShootDirection = 1;
                    }
                    else
                    {
                        var row = square2[1];
                        var col = square1[0];
                        square1 = col + row.ToString() + "v";
                        intermediarySquare = square2 + "v";
                        overShootDirection = 3;
                    }
                }
            }

            MoveCoreXY(square1);
            MoveCoreXY(intermediarySquare);
            MoveCoreXY(square2);
            MoveCoreXYCoords(OverShoot(square2, overShootDirection));
            ActivateMagnet(false);
        }

        public void PerformCapture(string square1)
        {
            string intermediarySquare1;
            string intermediarySquare2;

            MoveCoreXY(square1);
            ActivateMagnet(true);

            if (square1[1] <= '7')
                intermediarySquare1 = square1 + "v";
            else
                intermediarySquare1 = square1[0] - 1 + "v";

            intermediarySquare2 = "z" + square1[1] + "v";

            print("intermediary1: " + intermediarySquare1 + ", intermediary2: " + intermediarySquare2);

            MoveCoreXY(intermediarySquare1);
            MoveCoreXY(intermediarySquare2);

            MoveCoreXY(_captureSquares[_takenCount]);
            ActivateMagnet(false);
            _takenCount++;

            if (_takenCount != 4) return;

            print("remove captured pieces");
            _takenCount = 0;
        }

        // move the piece a little extra in a given direction to account for board friction
        public string OverShoot(string square, int direction)
        {
            var coords = _grblDict[square];

            var newCoords = new string[2];

            var x = 0;

            foreach (var c in coords)
                if (char.IsNumber(c) || c == '.')
                    newCoords[x] += c;
                else if (c == 'Y') x++;

            //Regex.Split()

            var xInt = float.Parse(newCoords[0]);
            var yInt = float.Parse(newCoords[1]);

            switch (direction)
            {
                case 1:
                    yInt -= OverShootAmount;
                    break;
                case 2:
                    xInt += OverShootAmount;
                    break;
                case 3:
                    yInt += OverShootAmount;
                    break;
                case 4:
                    xInt -= OverShootAmount;
                    break;
                case -1:
                    print("overShoot error");
                    break;
            }

            var returnCoords = "X" + xInt + "Y" + yInt;

            return returnCoords;
        }

        private bool KnightUpwardsMove(string square1, string square2)
        {
            return square1[1] - square2[1] < 0;
        }

        // returns true if the move moves the night longer vertically
        private bool KnightTallMove(string square1, string square2)
        {
            return Math.Abs(square1[1] - square2[1]) == 2;
        }

        // returns true if the move moves the piece to the right
        private bool MoveDirection(string square1, string square2)
        {
            return square1[0] < square2[0];
        }

        // f6h f6
        // horsey bad
        private int MoveCardinalDirection(string square1, string square2)
        {
            var col1 = square1[0];
            var col2 = square2[0];
            var row1 = square1[1];
            var row2 = square2[1];

            if (col1 - col2 < 0) return 2; // right
            if (col1 - col2 > 0) return 4; // left
            if (row1 - row2 < 0) return 1; // up
            if (row1 - row2 > 0) return 3; // down

            return -1;
        }

        [Button]
        public void MoveCoreXY(string square)
        {
            print("_grblDict at " + square + ": " + _grblDict[square]);
            
            SerialPort.Open();
            SerialPort.WriteLine(_grblDict[square]);
            SerialPort.Close();
        }
        
        [Button]
        public void MoveCoreXYDirect(string square)
        {
            //print("_grblDict at " + square + ": " + _grblDict[square]);
            
            SerialPort.Open();
            SerialPort.WriteLine(square);
            SerialPort.Close();
        }
        
        [Button]
        public void E2(string square)
        {
            //print("_grblDict at " + square + ": " + _grblDict[square]);
            
            SerialPort.Open();
            SerialPort.WriteLine("X172.5Y221");
            SerialPort.Close();
        }

        public void MoveCoreXYCoords(string coords)
        {
            SerialPort.Open();
            SerialPort.WriteLine(coords);
            SerialPort.Close();
        }

        public void ActivateMagnet(bool activated)
        {
            SerialPort.Open();
            SerialPort.WriteLine(activated ? _grblDict["MAGON"] : _grblDict["MAGOFF"]);
            SerialPort.Close();
        }

        [Button]
        public void HomeCoreXY()
        {
            SerialPort.Open();
            SerialPort.WriteLine("$H");
            SerialPort.Close();
        }
    }
}