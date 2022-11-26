using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using VDT.FaceRecognition.SDK;

public class RoomManager : MonoBehaviour
{
    // Static instance of the RoomManager
    private static RoomManager _instance;
    [SerializeField] private GameObject _roomPrefab;

    [SerializeField] private TextMeshProUGUI _roomNumber;

    // Number of rooms at the same time
    [SerializeField] private int _numberOfRoomsActives = 0;

    // The room where the player is
    [SerializeField] private Room _currentRoom = null;

    // Incrementing value used to give an id to each doors
    private int _roomId = 1;

    // Used to keep the previous current room before it is deleted
    [SerializeField] private Room _previousRoom;
    private bool _previousRoomExists = true;

    // List of the next room to come
    public List<Room> Rooms;
    public static RoomManager Instance => _instance;

    public Room CurrentRoom => _currentRoom;

    // private bool _isAbleCutLowPass = true;

    // Invoked when the play is in a new room
    public static Action OnNewRoom;


    private void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        Rooms = new List<Room>();

        StartCoroutine("createStartingRooms");
    }

    private IEnumerator createStartingRooms()
    {
        // Wait for game manager to be instanciated
        yield return new WaitForSeconds(1);
        
        // Wait until all emotions are ready
        yield return new WaitUntil(() => GameManager.Instance.AreAllEmotionsReady() == true);
        
        for (int i = 0; i < _numberOfRoomsActives; i++)
        {
            CreateRoom();
        }

        _currentRoom = Rooms[0];
        _previousRoom = CurrentRoom;
    }

    public void RestartRoom()
    {
        _roomId = 1;
        
        Destroy(_currentRoom.gameObject);
        foreach (var room in Rooms)
        {
            Destroy(room.gameObject);
        }
        if (_previousRoomExists)
        {
            Destroy(_previousRoom.gameObject);
        }
        
        
        Rooms = new List<Room>();

        for (int i = 0; i < _numberOfRoomsActives; i++)
        {
            CreateRoom();
        }

        _currentRoom = Rooms[0];
        _previousRoom = CurrentRoom;
        
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

    private void DeleteOldRoom()
    {
        GameObject door = _previousRoom.gameObject;
        Destroy(door);
        _previousRoomExists = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.Instance.IsVerificationDone)
            return;
        
        // Makes the doors move to the player
        MoveOnZ(_currentRoom.transform);
        foreach (var door in Rooms)
        {
            MoveOnZ(door.transform);
        }
        if (_previousRoomExists)
        {
            MoveOnZ(_previousRoom.transform);
        }


        // Remove behind room and add a new one 
        if (_previousRoomExists && _previousRoom.transform.position.z <= -10)
        {
            DeleteOldRoom();

            // If a door is removed, add a new one after the remaining ones
            CreateRoom();
        }

        // Check current room
        if (CheckCurrentRoom() == false)
        {
            _previousRoom = _currentRoom;
            _previousRoomExists = true;
            _currentRoom = Rooms[0];
            Rooms.RemoveAt(0);
            
            OnNewRoom?.Invoke();
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
        return _currentRoom.transform.position.z > -5;
    }

    private void MoveOnZ(Transform tf)
    {
        tf.position -= tf.forward * GameManager.Instance.GameSpeed;
    }

    public void OpenCurrentDoor()
    {
        CurrentRoom.OpenDoor();
        startNextRoomMusic();
    }

    private void startNextRoomMusic()
    {
        string musicToStart = Rooms[0].getRoomMusic();
        string musicToEnd = _currentRoom.getRoomMusic();
        // Sound applause = AudioManager.instance.GetSound("applause");

        AudioManager.instance.Play("applause", 3f);
        if (musicToEnd != musicToStart)
        {
            StartCoroutine(AudioManager.instance.StopSoundAfterTime(musicToEnd, 1f, 0.5f));
            StartCoroutine(AudioManager.instance.PlayAfterTime(musicToStart, 2f));
            // if(IsAbleToCutLowPass())
                // StartCoroutine(AudioManager.instance.ChangeFloatMixerAfterTime("CutoffLowPass", 5000f, 1f));

        }
       
        
        
    }
    
}