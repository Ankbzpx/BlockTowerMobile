using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RoomItemGroup : Photon.PunBehaviour
{
    [SerializeField]
    GameObject roomItemPrefab;

    [SerializeField]
    Text instructionText;

    List<RoomItem> roomItemList = new List<RoomItem>();

    public override void OnReceivedRoomListUpdate()
    {
        RoomInfo[] roomInfoList = PhotonNetwork.GetRoomList();

        if (roomInfoList.Length == 0)
            instructionText.text = "No room found";
        else
            instructionText.text = "";

        foreach (RoomInfo roomInfo in roomInfoList)
        {
            RoomReceived(roomInfo);
        }

        RemoveOldRooms();
    }



    public void Refresh()
    {
        RoomInfo[] roomInfoList = PhotonNetwork.GetRoomList();

        if (roomInfoList.Length == 0)
            instructionText.text = "No room found";
        else
            instructionText.text = "";

        foreach (RoomInfo roomInfo in roomInfoList)
        {
            RoomReceived(roomInfo);
        }

        RemoveOldRooms();
    }

    public static void OnClickJoinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {

        }
        else
        {
            print("Join room failed.");
        }
    }

    private void RoomReceived(RoomInfo roomInfo)
    {
        Debug.Log("Room received");

        int index = roomItemList.FindIndex(x => x.roomName == roomInfo.Name);

        if (index == -1)
        {
            if (roomInfo.IsVisible && roomInfo.PlayerCount < roomInfo.MaxPlayers)
            {
                GameObject roomItemObj = Instantiate(roomItemPrefab);
                roomItemObj.transform.SetParent(transform, false);
                Debug.Log("Room object instantiated");

                RoomItem roomItem = roomItemObj.GetComponent<RoomItem>();
                roomItemList.Add(roomItem);

                index = (roomItemList.Count - 1);
            }
        }

        if (index != -1)
        {
            RoomItem roomItem = roomItemList[index];
            roomItem.SetRoomNameText(roomInfo.Name);
            roomItem.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomItem> removeRooms = new List<RoomItem>();

        foreach (RoomItem roomItem in roomItemList)
        {
            if (!roomItem.Updated)
                removeRooms.Add(roomItem);
            else
                roomItem.Updated = false;
        }

        //
        foreach (RoomItem roomItem in removeRooms)
        {
            GameObject roomItemObj = roomItem.gameObject;
            roomItemList.Remove(roomItem);
            Destroy(roomItemObj);
        }
    }
}
