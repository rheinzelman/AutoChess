using System.IO.Ports;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace ChessGame.PlayerInputInterface
{
    public class BoardStateEvent : UnityEvent<int[,]> {};

    public class BoardInputManager : BaseInputHandler
    {
        private int[,] _initialState =
        {
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 },
            { 1, 1, 0, 0, 0, 0, 1, 1 }
        };

        private int[,] _finalState;
        
        private IODriver _ioDriver;

        public BoardStateEvent stateChange = new BoardStateEvent();

        // Start is called before the first frame update
        private void Start()
        {
            _ioDriver = _ioDriver ? _ioDriver : GetComponent<IODriver>() ?? gameObject.AddComponent<IODriver>();
            //IODriver.SerialPort.DataReceived += CheckInput;
            var board = FindObjectOfType<Board2D>();
            if (board) stateChange.AddListener(board.HighlightTilesFromArray);
        }
        //
        // private void CheckInput(object sender, SerialDataReceivedEventArgs e)
        // {
        //     print("Data received!");
        //     
        //     if (_ioDriver is null) return;
        //     
        //     NotationsHandler.Print2DArray(_ioDriver.BoardToArray());
        // }

        // Update is called once per frame
        public void Update()
        {
            //print("IO Driver Open: " + _ioDriver.IsOpen());
            
           //if (!Input.GetKeyDown(KeyCode.Space)) return;

           

            if (_ioDriver is null || !TurnActive) return;

            if (Input.GetKeyDown(KeyCode.Space) || _ioDriver.CapturedPiece()) //_ioDriver.CapturedPiece() || 
            {
                _initialState = GetCurrentStateInBoard();
                print("New board state!");
                NotationsHandler.Print2DArray(_initialState);
            }

            //print("Pressed space!\n");
            
            print("Initial");
            NotationsHandler.Print2DArray(_initialState);

            _finalState = GetCurrentStateInBoard();
            
            print("Final");
            NotationsHandler.Print2DArray(_finalState);
            
            var move = _ioDriver.GetMoveFromDifferenceArray(_initialState, _finalState);
            
            // NotationsHandler.Print2DArray(_initialState);
            // NotationsHandler.Print2DArray(_finalState);
            // NotationsHandler.Print2DArray(_ioDriver.GetDifferenceArray(_initialState, _finalState));
            //
            // print("Item 1: " + move.Item1 + ", Item 2: " + move.Item2);

            SendMove(move.Item1, move.Item2);
        }

        public override bool SendMove(Vector2Int from, Vector2Int to)
        {
            if (!base.SendMove(from, to)) return false;

            _initialState = _finalState;

            return true;
        }
        
        [Button]
        public void TestReceiveMove(Vector2Int from, Vector2Int to, PieceColor color = PieceColor.Unassigned, string args = "", string fen = "")
        {
            var moveData = new MoveEventData(null, color, args, "", BoardManager.Instance.BoardState);
            ReceiveMove(from, to, moveData);
        }

        private int[,] GetCurrentStateInBoard()
        {
            var bs = _initialState;
            if (_ioDriver) bs = _ioDriver.BoardToArray();
            //NotationsHandler.Print2DArray(bs);
            stateChange.Invoke(bs);
            return bs;
        }

        public override void ReceiveMove(Vector2Int from, Vector2Int to, MoveEventData moveData)
        {
            // _initialState[to.y, to.x] = 1;
            // _initialState[from.y, from.x] = 0;

            _initialState = CharArrayToInt(moveData.BoardState);

            var moveTuple = (NotationsHandler.CoordinateToUCI(from), NotationsHandler.CoordinateToUCI(to));
            
            print("Move received to board!");
            
            print("Move: " + from + " -> " + to);
            
            NotationsHandler.Print2DArray(moveData.BoardState);
            
            print("UCI Move: " + moveTuple.Item1 + moveTuple.Item2);

            print("MoveData args: " + moveData.Args);
            
            print("New int array: ");
            
            NotationsHandler.Print2DArray(CharArrayToInt(moveData.BoardState));
            
            if (_ioDriver == null) return;

            if (moveData.Args.Contains("t"))
            {
                if (moveData.Args.Contains("e")) 
                    _ioDriver.PerformCapture(NotationsHandler.CoordinateToUCI(to + Vector2Int.down));
                else if (moveData.Args.Contains("E"))
                    _ioDriver.PerformCapture(NotationsHandler.CoordinateToUCI(to + Vector2Int.up));
                else
                    _ioDriver.PerformCapture(moveTuple.Item2);
            }

            if (moveData.Args.Contains("n"))
                _ioDriver.PerformKnightMove(moveTuple.Item1, moveTuple.Item2);
            else if (moveData.Args.Contains("c"))
                _ioDriver.PerformCastling(moveTuple.Item1, moveTuple.Item2);
            else
                _ioDriver.PerformStandardMove(moveTuple.Item1, moveTuple.Item2);

            NotationsHandler.Print2DArray(_initialState);
        }

        private int[,] CharArrayToInt(char[,] charArr)
        {
            var intArr = new int[8,8];
            
            for (var y = 0; y < 8; y++)
                for (var x = 0; x < 8; x++)
                    intArr[x, y] = charArr[x, y] == '-' ? 0 : 1;

            return intArr;
        }
    }
}
