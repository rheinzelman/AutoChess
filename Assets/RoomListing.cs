using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomData roomListing;
    [SerializeField]
    private Transform content;
    private List<RoomData> RoomList = new List<RoomData>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = RoomList.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(RoomList[index].gameObject);
                    RoomList.RemoveAt(index);
                }
            }
            else
            {
                RoomData listing = Instantiate(roomListing, content);
                if (listing != null)
                {
                    listing.SetRoomInfo(info);
                    RoomList.Add(listing);
                }
            }
        }
    }
}
