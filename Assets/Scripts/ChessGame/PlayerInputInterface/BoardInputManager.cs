using System.IO.Ports;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace ChessGame.PlayerInputInterface
{
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

        // Start is called before the first frame update
        // private void Start()
        // {
        //     //_ioDriver = _ioDriver ? _ioDriver : GetComponent<IODriver>() ?? gameObject.AddComponent<IODriver>();
        //
        //     //IODriver.SerialPort.DataReceived += CheckInput;
        // }
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
            
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            if (_ioDriver is null) return;

            print("Pressed space!\n");

            _finalState = _ioDriver.BoardToArray();
            
            NotationsHandler.Print2DArray(_initialState);
            NotationsHandler.Print2DArray(_finalState);
            NotationsHandler.Print2DArray(_ioDriver.GetDifferenceArray(_initialState, _finalState));

            var move = _ioDriver.GetMoveFromDifferenceArray(_initialState, _finalState);
            
            print("Item 1: " + move.Item1 + ", Item 2: " + move.Item2);

            SendMove(move.Item1, move.Item2);
        }

        // private int[,] CharToBoardState(char[,] charArr)
        // {
        //     
        // }

        public override bool SendMove(Vector2Int from, Vector2Int to)
        {
            if (!base.SendMove(from, to)) return false;

            _initialState = _finalState;

            return true;
        }

        [Button]
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
            
            //_ioDriver.PerformStandardMove(NotationsHandler.CoordinateToUCI(from), NotationsHandler.CoordinateToUCI(to));
            
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
