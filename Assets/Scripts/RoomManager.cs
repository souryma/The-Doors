using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // Static instance of the RoomManager
    private static RoomManager _instance;
    [SerializeField] private GameObject _roomPrefab;
    [SerializeField] private TextMeshProUGUI _roomNumber;
    // Number of rooms at the same time
    [SerializeField] private int _numberOfRoomsActives = 0;
    private Room _currentRoom = null;
    // Incrementing value used to give an id to each doors
    private int _roomId = 1;

    public List<Room> Rooms;
    public static RoomManager Instance => _instance;
    public Room CurrentRoom => _currentRoom;
    
    
    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        Rooms = new List<Room>();

        for (int i = 0; i < _numberOfRoomsActives; i++)
        {
            CreateRoom();
        }

        _currentRoom = Rooms[0];
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    /// <summary>
    /// Creates a room behind the last one in the list
    /// </summary>
    private void CreateRoom()
    {
        GameObject roomGO = Instantiate(_roomPrefab);
        Room room = roomGO.AddComponent<Room>();
        roomGO.name = "Room" + _roomId;
        room.DoorId = _roomId;
        
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

        _currentRoom = Rooms[0];
    }

    // Update is called once per frame
    private void Update()
    {
        // Makes the doors move to the player
        foreach (var door in Rooms)
        {
            MoveOnZ(door.transform);
        }

        // Remove behind room and add a new one 
        if (Rooms[0].transform.position.z <= -10)
        {
            DeleteFirstRoomInList();

            // If a door is removed, add a new one after the remaining ones
            CreateRoom();
        }

        // Check current room
        if (CheckCurrentRoom() == false)
        {
            _currentRoom = Rooms[1];
        }
        
        UpdateUIRoomNumber();
        
        //  DEBUG : Force Open door
        if (Input.GetKeyDown(KeyCode.T))
        {
            OpenCurrentDoor();
        }
    }

    private void UpdateUIRoomNumber()
    {
        _roomNumber.text = "Room number " + _currentRoom.DoorId;
    }

    private bool CheckCurrentRoom()
    {
        return Rooms[0].transform.position.z > -5;
    }

    private void MoveOnZ(Transform tf)
    {
        tf.position -= tf.forward * GameManager.Instance.GameSpeed;
    }

    public void OpenCurrentDoor()
    {
        CurrentRoom.OpenDoor();
    }
}