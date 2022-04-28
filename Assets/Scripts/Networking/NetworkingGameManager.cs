using System;
using System.Collections;
using AutoChess.ManagerComponents;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;


namespace Com.MyCompany.MyGame
{
    public class NetworkingGameManager : MonoBehaviourPunCallbacks
    {

        #region Public Methods

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
        public BoardManager boardManager;
        public bool gameActive;
        public Vector2Int from;
        public Vector2Int to;
        public List<Vector2Int> WhitePieces = new List<Vector2Int>();
        public List<Vector2Int> BlackPieces = new List<Vector2Int>();


        public void Start()
        {
            PV = GetComponent<PhotonView>();
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
                    Debug.Log("Piece has moved send RPC, Piece has moved from " + from + " to " + to);

                    int MoveData = from.x*1000 + from.y*100 + to.x*10 + to.y;
                    Debug.Log("compressed move data " + MoveData);
                    this.PV.RPC("MovePiece", RpcTarget.Others, MoveData);
                    //UpdateLists
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
            boardManager.MovePiece(new Vector2Int(fromx,fromy), new Vector2Int(tox,toy));
        }

        public bool pieceHasMoved()
        {
            if (WhitePieces.Count == boardManager.WhitePieces.Count)
            {
                for (int i = 0; i < WhitePieces.Count; i++)
                    if (WhitePieces[i] != boardManager.WhitePieces[i].currentPosition)
                    {
                        from = WhitePieces[i];
                        to = boardManager.WhitePieces[i].currentPosition;
                        return true;
                    }
            }

            if (BlackPieces.Count == boardManager.BlackPieces.Count)
            {
                for (int i = 0; i < BlackPieces.Count; i++)
                    if (BlackPieces[i] != boardManager.BlackPieces[i].currentPosition)
                    {
                        from = BlackPieces[i];
                        to = boardManager.BlackPieces[i].currentPosition;
                        return true;
                    }
            }
            return false;
        }

        public void UpdateLists()
        {
            WhitePieces.Clear();
            BlackPieces.Clear();
            foreach (AutoChess.ChessPieces.ChessPiece p in boardManager.WhitePieces)
                WhitePieces.Add(p.currentPosition);
            foreach (AutoChess.ChessPieces.ChessPiece p in boardManager.BlackPieces)
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