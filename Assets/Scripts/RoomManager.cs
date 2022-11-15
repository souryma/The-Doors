using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject RoomPrefab;

    public int NumberofRoomsActives = 0;
    public Room CurrentRoom => _currentRoom;
    public List<Room> Rooms;
    [Range(0, 0.01f)]
    public float RoomSpeed = 0.001f;

    private Room _currentRoom;
    private int _roomId;
    void Start()
    {
        _roomId = 0;
        Rooms = new List<Room>();
        
        for(int i = 0; i < NumberofRoomsActives; i ++)
        {
            CreateRoom();
        }
    }
    
    /// <summary>
    /// Creates a room behind the last one in the list
    /// </summary>
    private void CreateRoom()
    {
        GameObject roomGO = Instantiate(RoomPrefab);
        Room room = roomGO.AddComponent<Room>();
        roomGO.name = "Room" + _roomId;
        _roomId += 1;

        if (Rooms.Count > 0)
        {
            // Get last door frame Z position in list and add 10 for new position
            Vector3 position = Rooms[^1].transform.position;
            position.z += 10;

            room.gameObject.transform.position = position;
        }

        Rooms.Add(room);
    }
    
    private void DeleteFirstRoomInList()
    {
        GameObject door = Rooms[0].gameObject;
        Rooms.RemoveAt(0);
        Destroy(door);
    }

    // Update is called once per frame
    void Update()
    {
        // Makes the doors move to the player
        foreach (var door in Rooms)
        {
            MoveOnZ(door.transform);
        }
        
        // Detect if the first element in the list is still see by the player
        if (Rooms[0].transform.position.z <= -10)
        {
            DeleteFirstRoomInList();
            
            // If a door is removed, add a new one after the remaining ones
            CreateRoom();
        }
    }
    
    private void MoveOnZ(Transform tf)
    {
        tf.position -= tf.forward * RoomSpeed;
    }

    public void OpenCurrentDoor()
    {
        
    }
}
