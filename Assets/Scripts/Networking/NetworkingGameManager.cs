using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoChess;
using AutoChess.PlayerInput;
using ChessGame.PlayerInputInterface;
using System.ComponentModel;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;


namespace ChessGame
{
    public class NetworkingGameManager : MonoBehaviourPunCallbacks
    {

        #region Public Methods
        public char[] rows = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
        public enum PlayerColor
        { 
            White,
            Black,
            Unassigned
        }
        public PhotonView PV;
        public Text playername1;
        public Text playername2;
        public GameObject Board;
        public GameObject WaitText;
        public Text roomID;
        public List<GameObject> takenWhitePieces;
        public List<GameObject> takenBlackPieces;
        //public Text MoveHistory;
        public BoardManager boardManager;
        public GameManager gameManager;
        public bool gameActive;
        public GameObject blackInput;
        public GameObject whiteInput;
        public Vector2Int from;
        public Vector2Int to;
        public List<Vector2Int> WhitePieces = new List<Vector2Int>();
        public List<Vector2Int> BlackPieces = new List<Vector2Int>();


        public void Start()
        {
            PV = GetComponent<PhotonView>();
            gameManager = GameManager.Instance;
            boardManager = BoardManager.Instance;
            gameActive = false;
            roomID.text = PhotonNetwork.CurrentRoom.Name;
            Board.SetActive(false);
            WaitText.SetActive(true);

        }

        public void Update()
        {
            if(!gameActive)
                if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
                {
                    SetGameActive();
                }
            if (gameActive)
                if (pieceHasMoved())
                {
                    if (pieceHasBeenTaken())
                        FindTakenPiece(to);
                    Debug.Log("Piece has moved send RPC, Piece has moved from " + from + " to " + to);

                    int MoveData = from.x * 1000 + from.y * 100 + to.x * 10 + to.y;
                    Debug.Log("compressed move data " + MoveData);
                    this.PV.RPC("MovePiece", RpcTarget.Others, MoveData);
                    //MoveHistory.text = MoveHistory.text + (rows[from.x] + from.y.ToString() + rows[to.x] + to.y.ToString()); 
                    UpdateLists();
                }
        }

        [PunRPC]
        public void MovePiece(int moveData)
        {
            Debug.Log(moveData);
            int fromx = moveData/1000;
            int fromy = ((moveData)/100) % 10;
            int tox = ((moveData) / 10) % 10;
            int toy = moveData % 10;
            Debug.Log("Move piece " + fromx + fromy + " to " + tox + toy);
            boardManager.MovePiece(new Vector2Int(fromx, fromy), new Vector2Int(tox, toy), null);
            //swapTurns
            gameManager.SwapTurns();
            if (pieceHasBeenTaken())
                FindTakenPiece(new Vector2Int(tox,toy));
            //MoveHistory.text = MoveHistory.text + (rows[fromx] + fromy.ToString() + rows[tox] + toy.ToString());
            UpdateLists();
        }

        public bool pieceHasBeenTaken()
        {
            if (WhitePieces.Contains(to) || BlackPieces.Contains(to))
            {
                return true;
            }
            else
                return false;
        }

        public void FindTakenPiece(Vector2Int destination)
        {
            for (int i = 0; i < WhitePieces.Count; i++)
                if (WhitePieces[i] == destination)
                {
                    takenWhitePieces[i].SetActive(true);
                    takenWhitePieces.RemoveAt(i);
                    //GameObject pos = takenWhitePieces[i];
                    //takenWhitePieces.Remove(pos);
                }
            for (int i = 0; i < BlackPieces.Count; i++)
                if (BlackPieces[i] == destination)
                {
                    takenBlackPieces[i].SetActive(true);
                    takenBlackPieces.RemoveAt(i);
                    //GameObject pos = takenBlackPieces[i];
                    //takenBlackPieces.Remove(pos);
                }
        }

        public bool pieceHasMoved()
        {
            if (WhitePieces.Count == boardManager.whitePieces.Count)
            {
                for (int i = 0; i < WhitePieces.Count; i++)
                    if (WhitePieces[i] != boardManager.whitePieces[i].currentPosition)
                    {
                        from = WhitePieces[i];
                        to = boardManager.whitePieces[i].currentPosition;
                        return true;
                    }
            }

            if (BlackPieces.Count == boardManager.blackPieces.Count)
            {
                for (int i = 0; i < BlackPieces.Count; i++)
                    if (BlackPieces[i] != boardManager.blackPieces[i].currentPosition)
                    {
                        from = BlackPieces[i];
                        to = boardManager.blackPieces[i].currentPosition;
                        return true;
                    }
            }
            return false;
        }

        public void UpdateLists()
        {
            WhitePieces.Clear();
            BlackPieces.Clear();
            foreach (ChessGame.ChessPieces.BaseChessPiece p in boardManager.whitePieces)
                WhitePieces.Add(p.currentPosition);
            foreach (ChessGame.ChessPieces.BaseChessPiece p in boardManager.blackPieces)
                BlackPieces.Add(p.currentPosition);

        }

        public void SetGameActive()
        {
            gameActive = true;
            WaitText.SetActive(false);
            Board.SetActive(true);
            playername1.text = PhotonNetwork.PlayerList[0].NickName;
            playername2.text = PhotonNetwork.PlayerList[1].NickName;
            playername2.color = Color.black;
            if (PhotonNetwork.IsMasterClient)
            {
                BaseInputHandler blackInputHandler = blackInput.GetComponent<BaseInputHandler>();
                Destroy(blackInputHandler);
            }
            else
            {
                BaseInputHandler whiteInputHandler = whiteInput.GetComponent<BaseInputHandler>();
                Destroy(whiteInputHandler);
            }
            UpdateLists();

        }



        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods

        /*
        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        */

        #endregion

        #region Photon Callbacks

        public void quit()
        {
            Application.Quit();
        }

        public void Resign()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("Player Joined " + other.NickName);
            /*
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
            */

        }


        public override void OnPlayerLeftRoom(Player other)
        {
            
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("Launcher");

        }


        #endregion
    }
}