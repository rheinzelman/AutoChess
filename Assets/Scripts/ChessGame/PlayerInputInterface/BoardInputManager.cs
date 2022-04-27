using System.IO.Ports;
using UnityEngine;
using Utils;

namespace ChessGame.PlayerInputInterface
{
    public class BoardInputManager : BaseInputHandler
    {
        private IODriver _ioDriver;

        // Start is called before the first frame update
        private void Start()
        {
            _ioDriver = _ioDriver ? _ioDriver : GetComponent<IODriver>() ?? gameObject.AddComponent<IODriver>();

            IODriver.SerialPort.DataReceived += CheckInput;
        }

        private void CheckInput(object sender, SerialDataReceivedEventArgs e)
        {
            print("Data received!");
            
            NotationsHandler.Print2DArray(_ioDriver.BoardToArray());
        }

        // Update is called once per frame
        public void Update()
        {
            //print("IO Driver Open: " + _ioDriver.IsOpen());
            
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            
            print("Pressed space!\n");

            var arr = _ioDriver.BoardToArray();
            
            NotationsHandler.Print2DArray(arr);
        }
    }
}
