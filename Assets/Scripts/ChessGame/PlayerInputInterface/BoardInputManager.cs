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
