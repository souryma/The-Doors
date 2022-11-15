using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the door frames to the player and creates the new doorframes
/// </summary>
public class LevelManager : MonoBehaviour
{
    public GameObject DoorPrefab;
    public List<GameObject> DoorsInScene;
    public int NumberOfDoorsAtInitialisation = 3;
    [Range(0, 0.01f)]
    public float DoorsSpeed = 0.001f;

    private int _doorNumber;

    // Start is called before the first frame update
    void Start()
    {
        _doorNumber = 0;
        DoorsInScene = new List<GameObject>();
        
        for(int i = 0; i < NumberOfDoorsAtInitialisation; i ++)
        {
            CreateDoorFrame();
        }
    }

    /// <summary>
    /// Creates a door behind the last one in the list
    /// </summary>
    private void CreateDoorFrame()
    {
        GameObject door = (Instantiate(DoorPrefab));
        door.name = "Door" + _doorNumber;
        _doorNumber += 1;

        if (DoorsInScene.Count > 0)
        {
            // Get last door frame Z position in list and add 10 for new position
            Vector3 position = DoorsInScene[^1].transform.position;
            position.z += 10;

            door.transform.position = position;
        }

        DoorsInScene.Add(door);
    }

    private void DeleteFirstDoorFrameInList()
    {
        GameObject door = DoorsInScene[0];
        DoorsInScene.RemoveAt(0);
        Destroy(door);
    }

    // Update is called once per frame
    void Update()
    {
        // Makes the doors move to the player
        foreach (var door in DoorsInScene)
        {
            MoveOnZ(door.transform);
        }
        
        // Detect if the first element in the list is still see by the player
        if (DoorsInScene[0].transform.position.z <= -10)
        {
            DeleteFirstDoorFrameInList();
            
            // If a door is removed, add a new one after the remaining ones
            CreateDoorFrame();
        }
    }

    private void MoveOnZ(Transform tf)
    {
        tf.position -= tf.forward * DoorsSpeed;
    }
}